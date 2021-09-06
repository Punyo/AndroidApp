using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndroidApp.Assets
{
    class DebugCMD : AndroidX.Fragment.App.DialogFragment
    {
        private Activity activityinstance;
        private EditText title_field;
        private View view;
        private readonly string[] commandlist = new string[3] {"help","enablesync","disablesync" };

        public DebugCMD(Activity activity)
        {
            activityinstance = activity;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);          
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
            view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_genre, null);
            title_field = view.FindViewById<EditText>(Resource.Id.genre_textfield);
            builder.SetView(view).SetTitle("単語を登録")
                .SetNegativeButton(Resource.String.dialog_cancel, (a, b) => { Dismiss(); })
                .SetPositiveButton("実行", Excute);

            return builder.Create();
        }
        private void Excute(object s, DialogClickEventArgs args)
        {

        }

        private void SyncGenres()
        {

        }
    }
}