using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public LinearLayout maincontentlayout { private set; get; }
        private FloatingActionButton fab;
        private int CurrentLayoutID;
        private bool isloadedwords = false;
        public DoublelineListStruct[] CurrentWordlist;
        public List<GenreStruct> genres;

        private List<string> Genredescriptions;
        private List<string> Genretitles;
        public int Genreid { private set; get; }


        public const string SAVEDATANAME = "content.json";
        private readonly string GENREFOLDERDIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
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

            if (Genretitles == null && Genredescriptions == null)
            {
                Genretitles = new List<string>();
                Genredescriptions = new List<string>();
            }
            else
            {
                Genretitles.Clear();
                Genredescriptions.Clear();
            }
            genres.Clear();
            foreach (var item in Directory.GetDirectories(GENREFOLDERDIR))
            {
                if (item.Contains(GenreFragment.TAG))
                {
                    string name = item.Remove(0, item.LastIndexOf("/") + 1).Replace(GenreFragment.TAG, string.Empty);
                    GenreStruct genre = new GenreStruct();
                    DateTime a = Directory.GetCreationTime(Path.Combine(GENREFOLDERDIR, item));
                    Genretitles.Add(name);
                    Genredescriptions.Add($"作成日時：{a.Year}/{a.Month}/{a.Day}");
                    string json = FileIO.ReadFile(Path.Combine(item, SAVEDATANAME));
                    genre.GenreName = name;
                    if (!string.IsNullOrEmpty(json))
                    {
                        genre.Words = WordManager.DeserializeWordStructArray(json);
                    }
                    else
                    {
                        genre.Words = new DoublelineListStruct[0];
                    }
                    genres.Add(genre);
                    Genretitles.Sort();
                    Genredescriptions.Sort();
                }
            }
            RecyclerViewComponents.CreateDoublelineList(Genretitles.ToArray(), Genredescriptions.ToArray(), this, maincontentlayout, (a) => { ApplyChangetoGenreList(a.ToList()); }, RecyclerView_OnClick);
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else if (isloadedwords)
            {
                //CreateDoublelineListWithSwipe(titles.ToArray(), descs.ToArray(), (a) => { ApplyChangetoGenreList(a.ToList()); });
                RecyclerViewComponents.CreateDoublelineList(Genretitles.ToArray(), Genredescriptions.ToArray(), this, maincontentlayout, (a) => { ApplyChangetoGenreList(a.ToList()); }, RecyclerView_OnClick);
                GenreStruct g = new GenreStruct();
                g.GenreName = genres[Genreid].GenreName;
                g.Words = CurrentWordlist;
                genres[Genreid] = g;
                isloadedwords = false;
            }
            else
            {
                base.OnBackPressed();
            }
        }

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
            if (isloadedwords)
            {
                WordEnterFragment word = new WordEnterFragment(this, genres[Genreid].GenreName);
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
                CreateGenreList();
                toolbar.InflateMenu(Resource.Menu.menu_main);
            }

            else if (id == Resource.Id.nav_quiz)
            {
                View v = LayoutInflater.Inflate(Resource.Layout.quiz, maincontentlayout);
                RunOnUiThread(() => new QuizManager(this, CurrentWordlist, v.FindViewById<EditText>(Resource.Id.quiz_answer),
                    v.FindViewById<TextView>(Resource.Id.quiz_question),
                    v.FindViewById<Button>(Resource.Id.quiz_checkanewer),
                    v.FindViewById<ImageView>(Resource.Id.quiz_marubatsu), true));
            }
            else if (id == Resource.Id.nav_test)
            {
                View v = LayoutInflater.Inflate(Resource.Layout.quiz, maincontentlayout);
                RunOnUiThread(() => new TestManager(this, CurrentWordlist, v.FindViewById<EditText>(Resource.Id.quiz_answer),
                    v.FindViewById<TextView>(Resource.Id.quiz_question),
                    v.FindViewById<Button>(Resource.Id.quiz_checkanewer),
                    v.FindViewById<ImageView>(Resource.Id.quiz_marubatsu),false));
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
            if (isloadedwords)
            {
                GenreStruct g = new GenreStruct();
                g.GenreName = genres[Genreid].GenreName;
                g.Words = CurrentWordlist;
                genres[Genreid] = g;
                isloadedwords = false;
            }
            return true;
        }
        public void RecyclerView_OnClick(object sender, Adapter1ClickEventArgs e)
        {
            if (!isloadedwords)
            {
                //CreateDoublelineListWithSwipe(genres[e.Position].Words, (words) => { ApplyChangetoWordList(words, e.Position); });
                RecyclerViewComponents.CreateDoublelineList(genres[e.Position].Words, this, maincontentlayout, (words) => { ApplyChangetoWordList(words, e.Position); }, RecyclerView_OnClick);
                CurrentWordlist = genres[e.Position].Words;
                Genreid = e.Position;
                isloadedwords = true;
            }
        }

        public void ApplyChangetoWordList(DoublelineListStruct[] words, int index)
        {
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(genres[index].GenreName + GenreFragment.TAG, MainActivity.SAVEDATANAME)), words);
            GenreStruct g = new GenreStruct();
            g.GenreName = genres[index].GenreName;
            g.Words = words;
            genres[index] = g;
            CurrentWordlist = genres[index].Words;
        }

        private void ApplyChangetoGenreList(List<DoublelineListStruct> changedgenres)
        {
            foreach (var itema in genres)
            {
                if (!changedgenres.Exists(word => word.Title == itema.GenreName))
                {
                    genres.Remove(itema);
                    Directory.Delete(Path.Combine(GENREFOLDERDIR, itema.GenreName + GenreFragment.TAG), true);
                    break;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}