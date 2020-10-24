using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Assets
{
    class ArraySorter
    {
        public static string[] Sort(string[] sortarray)
        {
            Array.Sort(sortarray, new Comparator());
            return sortarray;
        }

        private class Comparator : IComparer<string>
        {

            public int Compare(string x, string y)
            {
                MatchCollection mc1 = Regex.Matches(x, "[/d]");
                MatchCollection mc2 = Regex.Matches(y, "[/d]");
                if (mc1 != null && mc2 != null)
                {
                    return int.Parse(mc1[0].Value) - int.Parse(mc2[0].Value);
                }
                return 1;
            }
        }
    }
}