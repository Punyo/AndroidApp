using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Support.V4.App;
using Android.Widget;
using Android.Support.V7.App;
using Android.App;
using System.IO;

namespace AndroidApp.Assets
{
    public class WordEnterFragment : Android.Support.V4.App.DialogFragment
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
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(Activity);
            view = Activity.LayoutInflater.Inflate(Resource.Layout.dialog_wordenter, null);
            builder.SetView(view).SetTitle("単語を登録")
                .SetNegativeButton(Resource.String.dialog_cancel, Cancel)
                .SetPositiveButton(Resource.String.dialog_register, Register);

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

        public void Register(object s, DialogClickEventArgs args)
        {
            title_field = view.FindViewById<EditText>(Resource.Id.title_textfield);
            desc_field = view.FindViewById<EditText>(Resource.Id.description_textfield);
            WordStruct newword = new WordStruct();
            newword.Title = title_field.Text;
            newword.Description = desc_field.Text;
            Array.Resize(ref activityinstance.CurrentWordlist, activityinstance.CurrentWordlist.Length + 1);
            activityinstance.CurrentWordlist[activityinstance.CurrentWordlist.Length - 1] = newword;
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(name + GenreFragment.TAG, MainActivity.FILENAME)), activityinstance.CurrentWordlist);
            activityinstance.CreateDoublelineList(activityinstance.CurrentWordlist);
        }

        public void Cancel(object s, DialogClickEventArgs args)
        {
            Dismiss();
        }
    }
}