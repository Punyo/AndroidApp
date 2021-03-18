using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;

namespace AndroidApp.Assets
{
    public class WordEnterFragment : AndroidX.Fragment.App.DialogFragment
    {
        private readonly MainActivity activityinstance;
        private EditText title_field;
        private EditText desc_field;
        private View view;
        private readonly string name;

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

            List<DoublelineListStruct> newwordlist = new List<DoublelineListStruct>();
            newwordlist.AddRange(activityinstance.LoadedGenreList[activityinstance.Genreid].Words);
            newwordlist.Add(newword);
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(name + GenreFragment.TAG, MainActivity.SAVEDATANAME)), newwordlist.ToArray());

            GenreStruct g = new GenreStruct(activityinstance.LoadedGenreList[activityinstance.Genreid].GenreName, newwordlist.ToArray());
            activityinstance.EditGenre(activityinstance.Genreid,g);

            RecyclerViewComponents.CreateDoublelineList(activityinstance.LoadedGenreList[activityinstance.Genreid].Words.ToArray(), activityinstance, activityinstance.maincontentlayout,
                (words) => { activityinstance.ApplyChangetoWordList(words, activityinstance.Genreid); }, activityinstance.OnClick_GenreList);
        }

        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
    }
}