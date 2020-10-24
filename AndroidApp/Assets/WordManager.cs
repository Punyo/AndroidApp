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
        public static string SerializeWordStructArray(WordStruct[] word)
        {
            return JsonSerializer.Serialize(word);
        }

        public static WordStruct[] DeserializeWordStructArray(string json)
        {
            return JsonSerializer.Deserialize<WordStruct[]>(json);
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

        public static WordStruct[] ReadWordList(string path)
        {
            string json = FileIO.ReadFile(path);
            if (json!=null)
            {
                return DeserializeWordStructArray(json);
            }
            return null;
        }

        public static void WriteWordlist(string path, WordStruct[] words)
        {
            FileIO.WriteFileAsync(path, SerializeWordStructArray(words), FileMode.Open);
        }
    }

    [System.Serializable]
    public struct WordStruct
    {
        public string Title { get; set; }
        public string Description { get; set; }
    
    }

    public struct GenreStruct
    {
        public string GenreName { get; set; }
        public WordStruct[] Words { get; set; }
    }
}