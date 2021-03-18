
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using System.IO.Compression;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    public class JsonFragment : AndroidX.Fragment.App.DialogFragment
    {
        private View view;
        private EditText text;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
            view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_directjson, null);
            text = view.FindViewById<EditText>(Resource.Id.jsonenter_textview);
            builder.SetView(view).SetTitle("データインポート")
                .SetNegativeButton(Resource.String.dialog_cancel, Cancel)
                .SetPositiveButton(Resource.String.devoption_import, Import);
            text.Text = DevOption.ZIPPATH.AbsolutePath;
            return builder.Create();
        }

        public void Import(object s, DialogClickEventArgs args)
        {
            //FileIO.WriteFileAsync(WordManager.GetInternalSavePath(MainActivity.SAVEDATANAME), view.FindViewById<EditText>(Resource.Id.jsonenter_textview).Text, System.IO.FileMode.Open);
            try
            {
                foreach (var item in Directory.GetDirectories(MainActivity.GENREFOLDERDIR))
                {
                    Directory.Delete(item, true);
                }
                ZipFile.ExtractToDirectory(text.Text, MainActivity.GENREFOLDERDIR);
                if (Directory.Exists(Path.Combine(MainActivity.GENREFOLDERDIR, ".__override__")))
                {
                    Directory.Delete(Path.Combine(MainActivity.GENREFOLDERDIR, ".__override__"), true);
                }
            }
            catch (System.Exception e)
            {
                throw e;
                DialogComponents.ShowWarning(e.Message, e.StackTrace, Platform.CurrentActivity);
            }
        }

        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
    }
}