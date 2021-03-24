using System;
using Android.Widget;
using System.IO;
using AndroidX.Fragment.App;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class DevOption
    {
        private FragmentManager fragment;
        public static readonly Java.IO.File ZIPPATH = Platform.CurrentActivity.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
        public DevOption(Button jsonimport, Button jsonexport, FragmentManager transaction)
        {
            jsonimport.Click += OpenImport;
            jsonexport.Click += OpenExport;
            fragment = transaction;
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