using System;
using Android.Widget;
using System.IO;
using AndroidX.Fragment.App;

namespace AndroidApp.Assets
{
    class DevOption
    {
        private FragmentManager fragment;
        private MainActivity a;
        public static readonly Java.IO.File ZIPPATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
        public DevOption(Button jsonimport, Button jsonexport, FragmentManager transaction, MainActivity activity)
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