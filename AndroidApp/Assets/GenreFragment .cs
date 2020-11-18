using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Assets
{
    public class GenreFragment : Android.Support.V4.App.DialogFragment
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
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(Activity);
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