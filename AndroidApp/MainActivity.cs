using System;
using System.Collections.Generic;
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

namespace AndroidApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private AndroidX.AppCompat.Widget.Toolbar toolbar;
        private DrawerLayout drawer;
        public LinearLayout maincontentlayout { private set; get; }
        private FloatingActionButton fab;
        public DoublelineListStruct[] CurrentWordlist;
        public List<GenreStruct> genres;

        private List<string> Genredescriptions;
        private List<string> Genretitles;

        private RemovalAdapter1 currentadapter;
        public int Genreid { private set; get; }


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
        }

        public async void CreateGenreList()
        {
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
            string[] dirs = Directory.GetDirectories(GENREFOLDERDIR);
            TestResultStruct[][] results = await LoadScoreList(dirs);
            for (int i = 0; i < dirs.Length; i++)
            {
                if (dirs[i].Contains(GenreFragment.TAG))
                {
                    string name = dirs[i].Remove(0, dirs[i].LastIndexOf("/") + 1).Replace(GenreFragment.TAG, string.Empty);
                    DateTime creationtime = Directory.GetCreationTime(Path.Combine(GENREFOLDERDIR, dirs[i]));
                    Genretitles.Add(name);
                    Genredescriptions.Add($"作成日時：{creationtime.Year}/{creationtime.Month}/{creationtime.Day}");
                    string json = FileIO.ReadFile(Path.Combine(dirs[i], SAVEDATANAME));
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
            Genretitles.Sort();
            Genredescriptions.Sort();
            genres.Sort();
            RecyclerViewComponents.CreateDoublelineList(Genretitles.ToArray(), Genredescriptions.ToArray(), this, maincontentlayout, (a) => { ApplyChangetoGenreList(a.ToList()); }, RecyclerView_OnClick);
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
                    catch (FileNotFoundException e)
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
            else if (Genreid != -1)
            {
                RecyclerViewComponents.CreateDoublelineList(Genretitles.ToArray(), Genredescriptions.ToArray(), this, maincontentlayout, (a) => { ApplyChangetoGenreList(a.ToList()); }, RecyclerView_OnClick);
                Genreid = -1;
                CurrentWordlist = null;
                RemovalAdapterCheck();
            }
            else
            {
                base.OnBackPressed();
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

            if (id == Resource.Id.nav_wordlist)
            {
                maincontentlayout.RemoveAllViews();
                CreateGenreList();
            }

            else if (id == Resource.Id.nav_quiz)
            {
                if (CurrentWordlist != null)
                {
                    if (CurrentWordlist.Length == 0)
                    {
                        DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_nowords_title), Resources.GetString(Resource.String.dialog_nowords_desc), this);
                        return false;
                    }
                    else
                    {
                        maincontentlayout.RemoveAllViews();
                        View v = LayoutInflater.Inflate(Resource.Layout.quiz, maincontentlayout);
                        RunOnUiThread(() => new QuizManager(this, CurrentWordlist, v.FindViewById<EditText>(Resource.Id.quiz_answer),
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
                if (CurrentWordlist != null)
                {
                    if (CurrentWordlist.Length == 0)
                    {
                        DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_nowords_title), Resources.GetString(Resource.String.dialog_nowords_desc), this);
                        return false;
                    }
                    else
                    {
                        maincontentlayout.RemoveAllViews();
                        View v = LayoutInflater.Inflate(Resource.Layout.quiz, maincontentlayout);
                        RunOnUiThread(() => new TestManager(this, CurrentWordlist, v.FindViewById<EditText>(Resource.Id.quiz_answer),
                            v.FindViewById<TextView>(Resource.Id.quiz_question),
                            v.FindViewById<Button>(Resource.Id.quiz_checkanewer),
                            v.FindViewById<ImageView>(Resource.Id.quiz_marubatsu),
                            WordManager.GetInternalSavePath(Path.Combine(genres[Genreid].GenreName + GenreFragment.TAG, MainActivity.SCOREDATANAME)), false, genres[Genreid].GenreName, Genreid));
                    }
                }
                else
                {
                    DialogComponents.ShowWarning(Resources.GetString(Resource.String.dialog_notselect_title), Resources.GetString(Resource.String.dialog_notselect_desc), this);
                    return false;
                }
            }
            else if (id == Resource.Id.nav_devoption)
            {
                maincontentlayout.RemoveAllViews();
                View v = LayoutInflater.Inflate(Resource.Layout.devoption, maincontentlayout);
                new DevOption(v.FindViewById<Button>(Resource.Id.button_jsonimport), v.FindViewById<Button>(Resource.Id.button_jsonexport), SupportFragmentManager, this);
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
                //RecyclerViewComponents.CreateRemovalDoublelineList(this,maincontentlayout);
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
                GenreStruct g = new GenreStruct(genres[Genreid].GenreName, CurrentWordlist, genres[Genreid].Results);
                genres[Genreid] = g;
                //isloadedwords = false;
                Genreid = -1;
                RemovalAdapterCheck();
            }
            CurrentWordlist = null;
            return true;
        }


        public void RecyclerView_OnClick(object sender, Adapter1ClickEventArgs e)
        {
            if (Genreid == -1)
            {
                RemovalAdapter1 adapter1 = new RemovalAdapter1(genres[e.Position].Words.ToArray());
                adapter1.OnRemoveExcuted += (words) => { toolbar.Menu.Clear(); ApplyChangetoWordList(words, e.Position); };
                adapter1.OnRemoveModeEnter += () => { toolbar.InflateMenu(Resource.Menu.menu_deleteword); currentadapter = adapter1; };
                adapter1.OnRemoveModeExit += () => { toolbar.Menu.Clear(); currentadapter = null; };
                RecyclerViewComponents.CreateRemovalDoublelineList(adapter1, this, maincontentlayout, (words) => { ApplyChangetoWordList(words, e.Position); });
                CurrentWordlist = genres[e.Position].Words.ToArray();
                Genreid = e.Position;
            }
        }

        public void ApplyChangetoWordList(DoublelineListStruct[] words, int index)
        {
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(genres[index].GenreName + GenreFragment.TAG, MainActivity.SAVEDATANAME)), words);
            genres[index].Words = words.ToList();
            CurrentWordlist = genres[index].Words.ToArray();
        }

        private void ApplyChangetoGenreList(List<DoublelineListStruct> changedgenres)
        {
            for (int i = 0; i < genres.Count; i++)
            {
                if (!changedgenres.Exists(word => word.Title == genres[i].GenreName))
                {
                    Directory.Delete(Path.Combine(GENREFOLDERDIR, genres[i].GenreName + GenreFragment.TAG), true);
                    Genredescriptions.RemoveAt(i);
                    Genretitles.Remove(genres[i].GenreName);
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
        public void SetTestResults(TestResultStruct[] result, int genreid)
        {
            genres[genreid].SetTestResult(result.ToList());
        }
    }
}