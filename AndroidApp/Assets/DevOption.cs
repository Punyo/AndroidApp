using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using System.IO;

namespace AndroidApp.Assets
{
    class DevOption
    {
        private Android.Support.V4.App.FragmentManager fragment;
        private MainActivity a;
        public static readonly Java.IO.File ZIPPATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
        public DevOption(Button jsonimport, Button jsonexport, Android.Support.V4.App.FragmentManager transaction, MainActivity activity)
        {
            jsonimport.Click += OpenImport;
            jsonexport.Click += OpenExport;
            fragment = transaction;
            a = activity;
        }

        private void OpenExport(object sender, EventArgs e)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(MainActivity.GENREFOLDERDIR, Path.Combine(ZIPPATH.AbsolutePath, $"SAVE({DateTime.Now.ToString().Replace("/",string.Empty)}).zip"));
        }

        private void OpenImport(object sender, EventArgs e)
        {
            JsonFragment f = new JsonFragment();
            f.Show(fragment.BeginTransaction(), "json");
        }
    }
}