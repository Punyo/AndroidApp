
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;

namespace AndroidApp.Assets
{
    public class JsonFragment : Android.Support.V4.App.DialogFragment
    {
        private View view;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_directjson, null);
            builder.SetView(view).SetTitle("Jsonインポート")
                .SetNegativeButton(Resource.String.dialog_cancel, Cancel)
                .SetPositiveButton(Resource.String.devoption_import, Import);

            return builder.Create();
        }

        public void Import(object s, DialogClickEventArgs args)
        {
            //FileIO.WriteFileAsync(WordManager.GetInternalSavePath(MainActivity.SAVEDATANAME), view.FindViewById<EditText>(Resource.Id.jsonenter_textview).Text, System.IO.FileMode.Open);
        }

        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
    }
}