using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Preference;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class EraConverter
    {
        private const int JucheStart = 1912;
        private static JapaneseCalendar japanese;
        private static SettingsSaver saver;
        public static string ConvertToCalendar(DateTime date)
        {
            if (saver == null)
            {
                saver = new SettingsSaver(SettingsFragment.EraSaveFileName);
            }
            switch (saver.GetEnumSettings<Eras>())
            {
                case Eras.Gregorian:
                    {
                        return FromDateTimeToGregorian(date);
                    }
                case Eras.Japanese:
                    {
                        return FromDateTimeToJapaneseCalendar(date);
                    }
                case Eras.Juche:
                    {
                        return FromDateTimeToJucheCalendar(date);
                    }
            }
            return string.Empty;
        }

        private static string FromDateTimeToGregorian(DateTime date)
        {
            return $"{date.Year}/{date.Month}/{date.Day}";
        }

        private static string FromDateTimeToJapaneseCalendar(DateTime date)
        {
            if (japanese == null)
            {
                japanese = new JapaneseCalendar();
            }
            return $"令和{japanese.GetYear(date)}年{date.Month}月{date.Day}日";
        }

        private static string FromDateTimeToJucheCalendar(DateTime date)
        {
            return $"主体{date.Year - JucheStart + 1}年{date.Month}月{date.Day}日";
        }
    }
}