using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidApp.Assets;

namespace AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private Android.Support.V7.Widget.Toolbar toolbar;
        private DrawerLayout drawer;
        private LinearLayout maincontentlayout;
        private RecyclerView recyclerView;
        private RecyclerView.LayoutManager manager;
        private Adapter1 adapter;
        private FloatingActionButton fab;
        private int CurrentLayoutID;
        private bool isloadwords = false;
        public WordStruct[] CurrentWordlist;
        public List<GenreStruct> genres;
        private List<string> descs;
        private List<string> titles;
        private int genreid;
        

        public const string FILENAME = "content.json";
        private readonly string FOLDERDIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);

            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);
            navigationView.Menu.GetItem(0).SetChecked(true);

            maincontentlayout = FindViewById<LinearLayout>(Resource.Id.main_content_Layout);

            //ReadWord();
            genres = new List<GenreStruct>();
            OnNavigationItemSelected(navigationView.Menu.GetItem(0));
            toolbar.InflateMenu(Resource.Menu.menu_main);
            CurrentLayoutID = Resource.Id.nav_wordlist;

        }

        public void CreateGenreList()
        {
            //List<string> titles = new List<string>();
            //List<string> descs = new List<string>();

            if (titles == null && descs == null)
            {
                titles = new List<string>();
                descs = new List<string>();
            }
            else
            {
                titles.Clear();
                descs.Clear();
            }
            genres.Clear();
            foreach (var item in Directory.GetDirectories(FOLDERDIR))
            {
                if (item.Contains(GenreFragment.TAG))
                {
                    string name = item.Remove(0, item.LastIndexOf("/") + 1).Replace(GenreFragment.TAG, string.Empty);
                    GenreStruct genre = new GenreStruct();
                    DateTime a = Directory.GetCreationTime(Path.Combine(FOLDERDIR, item));
                    titles.Add(name);
                    descs.Add($"作成日時：{a.Year}/{a.Month}/{a.Day}");
                    string json = FileIO.ReadFile(Path.Combine(item, FILENAME));
                    genre.GenreName = name;
                    if (!string.IsNullOrEmpty(json))
                    {
                        genre.Words = WordManager.DeserializeWordStructArray(json);
                    }
                    else
                    {
                        genre.Words = new WordStruct[0];
                    }
                    genres.Add(genre);
                    titles.Sort();
                    descs.Sort();
                }
            }
            CreateDoublelineList(titles.ToArray(), descs.ToArray());
        }

        public void CreateWordList(string genrename)
        {
            foreach (var item in genres)
            {
                if (item.GenreName == genrename)
                {
                    CreateDoublelineList(item.Words);
                }
            }
        }


        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else if (isloadwords)
            {
                CreateDoublelineList(titles.ToArray(), descs.ToArray());
                GenreStruct g = new GenreStruct();
                g.GenreName = genres[genreid].GenreName;
                g.Words = CurrentWordlist;
                genres[genreid] = g;
                isloadwords = false;
            }
            else
            {
                base.OnBackPressed();
            }
        }

        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            //View view = (View)sender;
            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            //WordEnterFragment f = new WordEnterFragment(this);
            //f.Show(SupportFragmentManager.BeginTransaction(), "register");
            //Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction();
            //transaction.SetTransition(Android.Support.V4.App.FragmentTransaction.TransitFragmentOpen);
            //transaction.Add(Resource.Id.content,f).AddToBackStack(null).Commit();            
            if (isloadwords)
            {
                WordEnterFragment word = new WordEnterFragment(this, genres[genreid].GenreName);
                word.Show(SupportFragmentManager.BeginTransaction(), "register");
            }
            else
            {
                GenreFragment genre = new GenreFragment(this);
                genre.Show(SupportFragmentManager.BeginTransaction(), "genre");
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            CurrentLayoutID = id;
            maincontentlayout.RemoveAllViews();
            if (id == Resource.Id.nav_wordlist)
            {
                //ReadWord();
                //CreateDoublelineList(CurrentWordlist);
                CreateGenreList();
                toolbar.InflateMenu(Resource.Menu.menu_main);
            }

            else if (id == Resource.Id.nav_quiz)
            {
                View v = LayoutInflater.Inflate(Resource.Layout.quiz, maincontentlayout);
                //ReadWord();
                new QuizManager(this, CurrentWordlist, v.FindViewById<EditText>(Resource.Id.quiz_answer),
                    v.FindViewById<TextView>(Resource.Id.quiz_question),
                    v.FindViewById<Button>(Resource.Id.quiz_checkanewer),
                    v.FindViewById<ImageView>(Resource.Id.quiz_marubatsu));
            }
            else if (id == Resource.Id.nav_slideshow)
            {
            }
            else if (id == Resource.Id.nav_devoption)
            {
                View v = LayoutInflater.Inflate(Resource.Layout.devoption, maincontentlayout);
                new DevOption(v.FindViewById<Button>(Resource.Id.button_jsonimport), v.FindViewById<Button>(Resource.Id.button_jsonexport), SupportFragmentManager, this);
            }
            else if (id == Resource.Id.nav_share)
            {
                
            }
            else if (id == Resource.Id.nav_send)
            {

            }

            if (id == Resource.Id.nav_wordlist)
            {
                fab.Show();
            }
            else
            {
                toolbar.Menu.Clear();
                fab.Hide();
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            if (isloadwords)
            {
                GenreStruct g = new GenreStruct();
                g.GenreName = genres[genreid].GenreName;
                g.Words = CurrentWordlist;
                genres[genreid] = g;
                isloadwords = false;
            }
            return true;
        }

        public void CreateDoublelineList(WordStruct[] words)
        {
            maincontentlayout.RemoveAllViews();
            LayoutInflater.Inflate(Resource.Layout.recycler_view, maincontentlayout);
            recyclerView = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.list_view);
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);
            adapter = new Adapter1(words);
            var item = new ItemTouchHelper(new RecyclerViewItemSwiper(ItemTouchHelper.Left, ItemTouchHelper.Left, adapter));
            item.AttachToRecyclerView(recyclerView);

            adapter.ItemClick += RecyclerView_OnClick;

            recyclerView.SetAdapter(adapter);
            RecyclerView.ItemDecoration deco = new DividerItemDecoration(this, DividerItemDecoration.Vertical);
            recyclerView.AddItemDecoration(deco);

        }

        public void CreateDoublelineList(string[] titles, string[] description)
        {
            maincontentlayout.RemoveAllViews();
            LayoutInflater.Inflate(Resource.Layout.recycler_view, maincontentlayout);
            recyclerView = (RecyclerView)FindViewById<RecyclerView>(Resource.Id.list_view);
            manager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(manager);
            adapter = new Adapter1(titles, description);
            var item = new ItemTouchHelper(new RecyclerViewItemSwiper(ItemTouchHelper.Left, ItemTouchHelper.Left, adapter));
            item.AttachToRecyclerView(recyclerView);

            adapter.ItemClick += RecyclerView_OnClick;

            recyclerView.SetAdapter(adapter);
            RecyclerView.ItemDecoration deco = new DividerItemDecoration(this, DividerItemDecoration.Vertical);
            recyclerView.AddItemDecoration(deco);
        }

        private void RecyclerView_OnClick(object sender, Adapter1ClickEventArgs e)
        {
            if (!isloadwords)
            {
                CreateDoublelineList(genres[e.Position].Words);
                CurrentWordlist = genres[e.Position].Words;
                genreid = e.Position;

                isloadwords = true;
            }          
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}