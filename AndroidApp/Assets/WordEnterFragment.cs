using System;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.IO;
using AndroidX.Fragment.App;
using AndroidX.AppCompat.App;
using Android.App;

namespace AndroidApp.Assets
{
    public class WordEnterFragment : AndroidX.Fragment.App.DialogFragment
    {
        private MainActivity activityinstance;
        private EditText title_field;
        private EditText desc_field;
        private View view;
        private string name;

        public WordEnterFragment(MainActivity activity, string genrename)
        {
            activityinstance = activity;
            name = genrename;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            base.OnCreateDialog(savedInstanceState);
            AndroidX.AppCompat.App.AlertDialog.Builder builder = new AndroidX.AppCompat.App.AlertDialog.Builder(Activity);
            view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_wordenter, null);
            builder.SetView(view).SetTitle("単語を登録")
                .SetNegativeButton(Resource.String.dialog_cancel, Cancel)
                .SetPositiveButton(Resource.String.dialog_register, Register);

            return builder.Create();
        }

        public void Register(object s, DialogClickEventArgs args)
        {
            title_field = view.FindViewById<EditText>(Resource.Id.title_textfield);
            desc_field = view.FindViewById<EditText>(Resource.Id.description_textfield);

            DoublelineListStruct newword = new DoublelineListStruct();
            newword.Title = title_field.Text;
            newword.Description = desc_field.Text;

            Array.Resize(ref activityinstance.CurrentWordlist, activityinstance.CurrentWordlist.Length + 1);
            activityinstance.CurrentWordlist[activityinstance.CurrentWordlist.Length - 1] = newword;
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(name + GenreFragment.TAG, MainActivity.SAVEDATANAME)), activityinstance.CurrentWordlist);

            GenreStruct g = new GenreStruct(activityinstance.genres[activityinstance.Genreid].GenreName, activityinstance.CurrentWordlist);
            activityinstance.genres[activityinstance.Genreid] = g;

            RecyclerViewComponents.CreateDoublelineList(activityinstance.CurrentWordlist, activityinstance, activityinstance.maincontentlayout,
                (words) => { activityinstance.ApplyChangetoWordList(words, activityinstance.Genreid); }, activityinstance.RecyclerView_OnClick);
        }

        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
    }
}