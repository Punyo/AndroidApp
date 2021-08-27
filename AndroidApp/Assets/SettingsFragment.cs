using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class SettingsFragment : PreferenceFragmentCompat
    {
        private Eras currentera;
        private SettingsSaver saver;
        private ISharedPreferences preference;
        public const string EraSaveFileName = "eras.txt";
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            saver = new SettingsSaver(EraSaveFileName);
            SetPreferencesFromResource(Resource.Xml.settingsprefs, rootKey);
            Preference common_era = FindPreference("common_era");
            Preference common_deletealldata = FindPreference("common_deletealldata");
            FindPreference("debug_dump").PreferenceClick += Dump_PreferenceClick;
            common_era.PreferenceClick += Common_era_PreferenceClick;
            common_deletealldata.PreferenceClick += Common_deletealldata_PreferenceClick;
            preference = common_era.SharedPreferences;

        }

        private void Dump_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            string[] dirs = Directory.GetDirectories(MainActivity.GENREFOLDERDIR);
            Array.ForEach(dirs, (a) =>
               {
                   if (!a.Contains(GenreFragment.TAG))
                   {
                       a = string.Empty;
                   }
               });
            foreach (var item in dirs)
            {
                string dirpath = Path.Combine(Context.GetExternalFilesDir(Android.OS.Environment.DirectoryNotifications).AbsolutePath, "Backups");
                Directory.CreateDirectory(dirpath);
                if (item != string.Empty)
                {
                    string newdir = item.Replace(MainActivity.GENREFOLDERDIR, dirpath);
                    Directory.CreateDirectory(newdir);
                    FileInfo[] files = new DirectoryInfo(item).GetFiles();
                    foreach (var it in files)
                    {
                        File.Copy(it.FullName,Path.Combine(newdir,it.Name),true);
                    }
                }
            }
        }

        private void Common_deletealldata_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            Random r = new Random();
            int rint = r.Next(1, 100);
            if (rint == 3 && rint == 21)
            {
                HALDelete();
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(Platform.CurrentActivity);
                builder.SetTitle(Resource.String.dialog_deletealldata_title)
                    .SetMessage(Resource.String.dialog_deletealldata)
                    .SetPositiveButton(Platform.CurrentActivity.GetString(Resource.String.dialog_delele), (a, e) =>
                    {
                        DeleteData();
                    }).SetNegativeButton(Platform.CurrentActivity.GetString(Resource.String.dialog_cancel), (a, e) => { })
                     .Show();
            }
        }

        private void DeleteData()
        {
            foreach (var item in Directory.GetDirectories(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)))
            {
                Directory.Delete(item, true);
            }
            preference.Edit().Clear().Commit();
            Platform.CurrentActivity.FinishAndRemoveTask();
        }

        private void Common_era_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Platform.CurrentActivity);
            builder.SetTitle(Resource.String.dialog_era_title)
                .SetSingleChoiceItems(Resource.Array.setting_common_eras, Convert.ToInt32(saver.GetEnumSettings<Eras>()), (a, e) => { currentera = (Eras)e.Which; })
                .SetPositiveButton(Platform.CurrentActivity.GetString(Resource.String.dialog_ok), (a, e) =>
                 {
                     saver.SaveEnumSettings(currentera);
                 }).Show();
        }
        private void HALDelete()
        {

        }
    }
    public enum Eras
    {
        Gregorian = 0,
        Japanese = 1,
        Juche = 2
    }
}