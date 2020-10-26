using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace AndroidApp.Assets
{


    class FileIO
    {
        public static string ReadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fs = File.Open(filePath, FileMode.Open))
                {
                    byte[] content = new byte[fs.Length];
                    fs.Read(content, 0, (int)fs.Length);
                    fs.Dispose();
                    return System.Text.UTF8Encoding.UTF8.GetString(content);
                }
            }
            else
            {
                Log.Error(Application.Context.ApplicationContext.PackageName, $"File Not Found. Path:{filePath}");
                return null;
            }
        }

        public static async void WriteFileAsync(string filePath, string content, FileMode mode)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fs = File.Open(filePath, mode))
                {
                    byte[] contentbyte = UTF8Encoding.UTF8.GetBytes(content);
                    await fs.WriteAsync(contentbyte, 0, content.Length);
                    Log.Debug(Application.Context.ApplicationContext.PackageName, $"Wrote {content} in {filePath}.");
                    fs.Close();
                }
            }
            else
            {
                using (FileStream fs = File.Create(filePath))
                {
                    byte[] contentbyte = UTF8Encoding.UTF8.GetBytes(content);
                    await fs.WriteAsync(contentbyte, 0, content.Length);
                    Log.Debug(Application.Context.ApplicationContext.PackageName, $"Create File and wrote {content} in {filePath}.");
                    fs.Close();
                }
            }
        }  
    }
}