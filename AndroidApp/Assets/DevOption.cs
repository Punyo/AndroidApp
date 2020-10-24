using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class DevOption
    {
        private Android.Support.V4.App.FragmentManager fragment;
        private MainActivity a;
        public DevOption(Button jsonimport, Button jsonexport, Android.Support.V4.App.FragmentManager transaction, MainActivity activity)
        {
            jsonimport.Click += OpenImport;
            jsonexport.Click += OpenExport;
            fragment = transaction;
            a = activity;
        }

        private async void OpenExport(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(WordManager.SerializeWordStructArray(a.CurrentWordlist));
            View view = (View)sender;
            Snackbar.Make(view, "Jsonをクリップボードにコピーしました", Snackbar.LengthLong).Show();               
        }

        private void OpenImport(object sender, EventArgs e)
        {
            JsonFragment f = new JsonFragment();
            f.Show(fragment.BeginTransaction(), "json");
        }
    }
}