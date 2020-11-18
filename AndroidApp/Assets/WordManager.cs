using System.IO;
using System.Text.Json;
using System;

using Android.Content;
//using Android.OS;
public enum WordLoadMode
{
    External,
    Internal
}
namespace AndroidApp.Assets
{
    public class WordManager
    {
        public static string SerializeWordStructArray(DoublelineListStruct[] word)
        {
            return JsonSerializer.Serialize(word);
        }

        public static DoublelineListStruct[] DeserializeWordStructArray(string json)
        {
            return JsonSerializer.Deserialize<DoublelineListStruct[]>(json);
        }

        public static string GetInternalSavePath(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);
        }

        public static string GetExternalSavePath(Context context, string filename)
        {
            Java.IO.File file = context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
            return Path.Combine(file.AbsolutePath, filename);
        }

        public static DoublelineListStruct[] ReadWordList(string path)
        {
            string json = FileIO.ReadFile(path);
            if (json != null)
            {
                return DeserializeWordStructArray(json);
            }
            return null;
        }

        public static void WriteWordlist(string path, DoublelineListStruct[] words)
        {
            string a = SerializeWordStructArray(words);
            //FileIO.WriteFileAsync(path, a, FileMode.Open);
            File.WriteAllText(path, a);
        }
    }

    [System.Serializable]
    public struct DoublelineListStruct
    {
        //上段に表示
        public string Title { get; set; }
        //下段に表示
        public string Description { get; set; }
        public static bool operator ==(DoublelineListStruct a, DoublelineListStruct b)
        {
            if (a.Title == b.Description && a.Description == b.Description)
            {
                return true;
            }
            return false;
        }
        public static bool operator !=(DoublelineListStruct a, DoublelineListStruct b)
        {
            if (a.Title != b.Description || a.Description != b.Description)
            {
                return true;
            }
            return false;
        }
        public override bool Equals(Object objA)
        {
            return base.Equals(objA);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct GenreStruct :  IComparable<GenreStruct>
    {
        public string GenreName { get; set; }
        public DoublelineListStruct[] Words { get; set; }

        public int CompareTo(GenreStruct other)
        {
            return GenreName.CompareTo(other.GenreName);
        }
    }
}