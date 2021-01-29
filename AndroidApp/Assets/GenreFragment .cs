using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace AndroidApp.Assets
{
    public class GenreFragment : AndroidX.Fragment.App.DialogFragment
    {
        private View view;
        private MainActivity main;
        public const string TAG = "[GENRE]";

        public GenreFragment(MainActivity activity)
        {
            main = activity;
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
            builder.SetView(view).SetTitle("項目を作成")
                .SetNegativeButton(Resource.String.dialog_cancel, Cancel)
                .SetPositiveButton(Resource.String.dialog_register, RegisterGenre);

            return builder.Create();
        }

        //public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        //{
        //    base.OnCreate(savedInstanceState);
        //   view = inflater.Inflate(Resource.Layout.dialog_wordenter, null);
        //    //title_field.TextChanged += Title_field_TextChanged;
        //    //desc_field.TextChanged += Desc_field_TextChanged;
        //    return view;
        //}

        public void RegisterGenre(object s, DialogClickEventArgs args)
        {
            string path = WordManager.GetInternalSavePath(view.FindViewById<EditText>(Resource.Id.genre_textfield).Text + TAG);
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, MainActivity.SAVEDATANAME), string.Empty);
            File.WriteAllText(Path.Combine(path, MainActivity.SCOREDATANAME), string.Empty);
            main.CreateGenreList();
        }

        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
    }
}