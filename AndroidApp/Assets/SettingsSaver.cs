using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AndroidApp.Assets
{
    class SettingsSaver
    {
        private const string SettingsDirname = "Settings";
        private readonly string filename;
        private readonly string filepath;
        private static readonly string SettingsDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), SettingsDirname);

        public SettingsSaver(string settingsfilename)
        {
            Initialize();
            filename = settingsfilename;
            filepath = Path.Combine(SettingsDir, filename);
        }
        private static void Initialize()
        {
            if (!Directory.Exists(SettingsDir))
            {
                Directory.CreateDirectory(SettingsDir);
            }
        }

        public void SaveEnumSettings<T>(T value) where T : Enum
        {
            File.WriteAllTextAsync(filepath, Convert.ToInt32(value).ToString());
        }
        public T GetEnumSettings<T>() where T : notnull, Enum
        {
            if (File.Exists(filepath))
            {
                return (T)Enum.Parse(typeof(T), File.ReadAllText(filepath));
            }
            return default(T);
        }
        //public void SaveSettings<T>(T value)
        //{
        //    if (typeof(T) == typeof(string) || typeof(T) == typeof(int))
        //    {
        //        File.WriteAllTextAsync(filepath,Convert.ToString(value));
        //    }   else if (typeof(T) == typeof(Enum))
        //    {
        //        File.WriteAllTextAsync(filepath, Convert.ToInt32(value).ToString());
        //    }
        //    else
        //    {
        //        throw new NotSupportedException();
        //    }     
        //}
        //public T GetSettings<T>()
        //{
        //    if (typeof(T) == typeof(string))
        //    {

        //    }
        //    else if(typeof(T) == typeof(int))           
        //    {
        //       return (T)Convert.ToInt32(File.ReadAllText(filepath)); 
        //    }
        //    else if (typeof(T) == typeof(Enum))
        //    {
        //        File.ReadAllText(filepath, );
        //    }
        //    else
        //    {
        //        throw new NotSupportedException();
        //    }
        //}
    }
}