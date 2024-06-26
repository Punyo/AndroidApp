﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidApp.Assets;
using AndroidX.AppCompat.App;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Navigation;
using Microcharts.Droid;
using Xamarin.Essentials;

namespace AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AndroidX.AppCompat.Widget.Toolbar toolbar;
        private DrawerLayout drawer;
        public LinearLayout maincontentlayout { private set; get; }
        private FloatingActionButton fab;
        private List<DoublelineListStruct> GenreInfos = new List<DoublelineListStruct>();
        private List<GenreStruct> genres;
        public ReadOnlyCollection<GenreStruct> Genrelist
        {
            get
            {
                return genres.AsReadOnly();
            }
        }
        private RemovalAdapter1 currentadapter;
        public int Genreid { private set; get; }

        private BackButtonManager backButtonManager = new BackButtonManager();


        public const string SAVEDATANAME = "content.json";
        public const string SCOREDATANAME = "score.json";
        public static readonly string GENREFOLDERDIR = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Genreid = -1;
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
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

            genres = new List<GenreStruct>();
            OnNavigationItemSelected(navigationView.Menu.GetItem(0));
            toolbar.InflateMenu(Resource.Menu.menu_main);
            _ = RequestPermission();
        }

        public async Task CreateGenreList()
        {
            GenreInfos.Clear();
            genres.Clear();
            string[] dirs = Directory.GetDirectories(GENREFOLDERDIR);
            TestResultStruct[][] results = await LoadScoreList(dirs);
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].Contains(GenreFragment.TAG))
                {
                    string name = dirs[i].Remove(0, dirs[i].LastIndexOf("/") + 1).Replace(GenreFragment.TAG, string.Empty);
                    string description = "作成日時：" + EraConverter.ConvertToCalendar(Directory.GetCreationTime(Path.Combine(GENREFOLDERDIR, dirs[i])));
                    string json = FileIO.ReadFile(Path.Combine(dirs[i], SAVEDATANAME));
                    DoublelineListStruct genreinfo = new DoublelineListStruct(name, description);
                    GenreInfos.Add(genreinfo);
                    if (!string.IsNullOrEmpty(json))
                    {
                        GenreStruct genre = new GenreStruct(name, WordManager.DeserializeWordStructArray(json), results[i]);
                        genres.Add(genre);
                    }
                    else
                    {
                        GenreStruct genre = new GenreStruct(name, new DoublelineListStruct[0], results[i]);
                        genres.Add(genre);
                    }
                }
            }
            genres.Sort();
            GenreInfos.Sort();
            RecyclerViewComponents.CreateDoublelineList(GenreInfos.ToArray(), this, maincontentlayout, (a) => { ApplyChangetoGenreList(a.ToList()); }, RecyclerView_OnClick);
        }

        private async Task<TestResultStruct[][]> LoadScoreList(string[] dirnames)
        {
            TestResultStruct[][] results = new TestResultStruct[0][];
            foreach (var item in Directory.GetDirectories(GENREFOLDERDIR))
            {
                if (item.Contains(GenreFragment.TAG))
                {
                    string json = FileIO.ReadFile(Path.Combine(item, SCOREDATANAME));
                }
            }
            string[] dirs = dirnames;
            Array.Resize(ref results, dirnames.Length);
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].Contains(GenreFragment.TAG))
                {
                    string[] jsons = new string[0];
                    try
                    {
                        jsons = File.ReadAllLines(Path.Combine(dirs[i], SCOREDATANAME));
                    }
                    catch (FileNotFoundException)
                    {
                        await FileIO.WriteFileAsync(Path.Combine(dirs[i], SCOREDATANAME), string.Empty, FileMode.Open);
                    }
                    TestResultStruct[] res = new TestResultStruct[jsons.Length];
                    for (int ia = 0; ia < jsons.Length; ia++)
                    {
                        res[ia] = JsonSerializer.Deserialize<TestResultStruct>(jsons[ia]);
                    }
                    results[i] = res;
                }
            }
            return results;
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);

            }
            else
            if (Genreid != -1 && backButtonManager.GetCurrentID() == Resource.Id.nav_wordlist)
            {
                RecyclerViewComponents.CreateDoublelineList(GenreInfos.ToArray(), this, maincontentlayout, (a) => { ApplyChangetoGenreList(a.ToList()); }, RecyclerView_OnClick);
                Genreid = -1;
                //RemovalAdapterVisibleCheck();
                RemovalAdapterCheck();
                return;
            }
            int previousid = backButtonManager.GetProviousID();
            if (previousid == Resource.Id.nav_wordlist && Genreid != -1)
            {
                CreateWordlist(Genreid);
                fab.Show();
            }
            else
            {
                OnNavSelected(previousid, false);
            }
        }

        private void RemovalAdapterCheck()
        {
            if (currentadapter != null)
            {
                if (currentadapter.SelectedElements.Count > 0)
                {
                    toolbar.Menu.Clear();
                    currentadapter = null;
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.deleteword)
            {
                if (currentadapter != null)
                {
                    currentadapter.ExcuteRemove();
                }
                else
                {
                    Log.Error("ERROR", "currentadapterがnullです");
                }
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            if (Genreid != -1)
            {
                DoubleTextBoxFragment word = new DoubleTextBoxFragment(RegisiterWord);
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

            return OnNavSelected(id, true);
        }

        private bool OnNavSelected(int id, bool isrecordscreen)
        {
            if (id == Resource.Id.nav_wordlist)
            {
                maincontentlayout.RemoveAllViews();
                CreateGenreList();
            }

            else if (id == Resource.Id.nav_quiz)
            {
                if (Genreid != -1)
                {
                    if (genres[Genreid].Words.Count == 0)
                    {
                        DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_nowords_title), Resources.GetString(Resource.String.dialog_nowords_desc), this);
                        return false;
                    }
                    else
                    {
                        maincontentlayout.RemoveAllViews();
                        View v = LayoutInflater.Inflate(Resource.Layout.quiz, maincontentlayout);
                        RunOnUiThread(() => new QuizManager(this, genres[Genreid].Words.ToArray(), v.FindViewById<EditText>(Resource.Id.quiz_answer),
                            v.FindViewById<TextView>(Resource.Id.quiz_question),
                            v.FindViewById<Button>(Resource.Id.quiz_checkanewer),
                            v.FindViewById<ImageView>(Resource.Id.quiz_marubatsu), true));
                    }
                }
                else
                {
                    DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_notselect_title), Resources.GetString(Resource.String.dialog_notselect_desc), this);
                    return false;
                }

            }
            else if (id == Resource.Id.nav_test)
            {
                if (Genreid != -1)
                {
                    if (genres[Genreid].Words.Count == 0)
                    {
                        DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_nowords_title), Resources.GetString(Resource.String.dialog_nowords_desc), this);
                        return false;
                    }
                    else
                    {
                        maincontentlayout.RemoveAllViews();
                        View v = LayoutInflater.Inflate(Resource.Layout.test, maincontentlayout);
                        RunOnUiThread(() => new TestManager(this, genres[Genreid].Words.ToArray(), v.FindViewById<EditText>(Resource.Id.test_answer),
                            v.FindViewById<TextView>(Resource.Id.test_question),
                            v.FindViewById<Button>(Resource.Id.test_checkanewer),
                            v.FindViewById<ImageView>(Resource.Id.test_marubatsu),
                            WordManager.GetInternalSavePath(Path.Combine(genres[Genreid].GenreName + GenreFragment.TAG, MainActivity.SCOREDATANAME)), false, genres[Genreid].GenreName, Genreid));
                    }
                }
                else
                {
                    DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_notselect_title), Resources.GetString(Resource.String.dialog_notselect_desc), this);
                    return false;
                }
            }
            else if (id == Resource.Id.nav_howtouse)
            {
                maincontentlayout.RemoveAllViews();
                View v = LayoutInflater.Inflate(Resource.Layout.howtouse, maincontentlayout);
                new Howtouse();
            }
            else if (id == Resource.Id.nav_share)
            {
                if (Genreid != -1)
                {
                    if (genres[Genreid].Results.Count == 0)
                    {
                        DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_notestdata_title), Resources.GetString(Resource.String.dialog_notestdata_desc), this);
                        return false;
                    }
                    else
                    {
                        maincontentlayout.RemoveAllViews();
                        View v = LayoutInflater.Inflate(Resource.Layout.scoreanalytics, maincontentlayout);
                        RunOnUiThread(() => new ScoreAnalyticsManager(this, Resource.Id.scoreanalytics_recyclerView, v.FindViewById<TextView>(Resource.Id.scoreanalytics_title)
                            , v.FindViewById<ChartView>(Resource.Id.scoreanalytics_chartView)
                            , v.FindViewById<Button>(Resource.Id.scoreanalytics_nextbutton)
                            , v.FindViewById<Button>(Resource.Id.scoreanalytics_previousbutton)                           
                            , genres[Genreid].Results.ToArray()));
                    }
                }
                else
                {
                    DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_notselect_title), Resources.GetString(Resource.String.dialog_notselect_desc), this);
                    return false;
                }
            }
            else if (id == Resource.Id.nav_settings)
            {
                maincontentlayout.RemoveAllViews();
                SupportFragmentManager.BeginTransaction().Replace(Resource.Id.main_content_Layout, new SettingsFragment()).Commit();
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
            if (Genreid != -1)
            {
                //GenreStruct g = new GenreStruct(genres[Genreid].GenreName, genres[Genreid].Words.ToArray(), genres[Genreid].Results);
                //genres[Genreid] = g;
                //isloadedwords = false;
                //Genreid = -1;
                RemovalAdapterCheck();
            }
            if (isrecordscreen)
            {
                backButtonManager.RecordScreenID(id);
            }
            //CurrentWordlist = null;
            return true;
        }

        public void RecyclerView_OnClick(object sender, Adapter1ClickEventArgs e)
        {
            //if (Genreid == -1)
            //{
            CreateWordlist(e.Position);
            //}
        }

        private void CreateWordlist(int e)
        {
            //RemovalAdapter1 adapter1 = new RemovalAdapter1(genres[e].Words.ToArray());
            //adapter1.OnRemoveExcuted += (words) => { toolbar.Menu.Clear(); ApplyChangetoWordList(words, e); };
            //adapter1.OnRemoveModeEnter += () => { toolbar.InflateMenu(Resource.Menu.menu_deleteword); currentadapter = adapter1; };
            //adapter1.OnRemoveModeExit += () => { toolbar.Menu.Clear(); currentadapter = null; };
            RenamableAdapter adapter1 = new RenamableAdapter(genres[e].Words.ToArray(), this);
            RecyclerViewComponents.CreateCustomAdapterDoublelineList(adapter1, this, maincontentlayout, (words) => { ApplyChangetoWordList(words, e); });
            //CurrentWordlist = genres[e.Position].Words.ToArray();
            Genreid = e;
        }

        public void ApplyChangetoWordList(DoublelineListStruct[] words, int index)
        {
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(genres[index].GenreName + GenreFragment.TAG, MainActivity.SAVEDATANAME)), words);
            genres[index].Words = words.ToList();
            //CurrentWordlist = genres[index].Words.ToArray();
            //Genreid = index;
        }

        private void ApplyChangetoGenreList(List<DoublelineListStruct> changedgenres)
        {
            for (int i = 0; i < genres.Count; i++)
            {
                if (!changedgenres.Exists(word => word.Title == genres[i].GenreName))
                {
                    Directory.Delete(Path.Combine(GENREFOLDERDIR, genres[i].GenreName + GenreFragment.TAG), true);
                    //Genredescriptions.RemoveAt(i);
                    //Genretitles.Remove(genres[i].GenreName);
                    GenreInfos.RemoveAt(i);
                    genres.Remove(genres[i]);
                    break;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async Task RequestPermission()
        {
#if DEBUG
            var permissioncheck = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (permissioncheck != PermissionStatus.Granted)
            {
                var requestres = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (requestres != PermissionStatus.Granted)
                {
                    Platform.CurrentActivity.FinishAndRemoveTask();
                }
            }
#endif
        }

        public void SetTestResults(TestResultStruct[] result, int genreid)
        {
            genres[genreid].SetTestResult(result.ToList());
        }

        public void EditGenre(int genreid, GenreStruct editedgenre)
        {
            genres[genreid] = editedgenre;
        }
        public void RegisiterWord(EditText title_field, EditText desc_field)
        {

            DoublelineListStruct newword = new DoublelineListStruct();
            newword.Title = title_field.Text;
            newword.Description = desc_field.Text;

            //Array.Resize(ref CurrentWordlist, CurrentWordlist.Length + 1);
            //CurrentWordlist[CurrentWordlist.Length - 1] = newword;
            GenreStruct newgenre = Genrelist[Genreid];
            newgenre.Words.Add(newword);
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(Genrelist[Genreid].GenreName + GenreFragment.TAG, MainActivity.SAVEDATANAME)), newgenre.Words.ToArray());
            EditGenre(Genreid, newgenre);

            RecyclerViewComponents.CreateDoublelineList(newgenre.Words.ToArray(), this, maincontentlayout,
                (words) => { ApplyChangetoWordList(words, Genreid); }, RecyclerView_OnClick);
        }
    }
}