using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Preference;
using System;
using System.IO;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    internal class SettingsFragment : PreferenceFragmentCompat
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
            common_era.PreferenceClick += Common_era_PreferenceClick;
            common_deletealldata.PreferenceClick += Common_deletealldata_PreferenceClick;
            preference = common_era.SharedPreferences;
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