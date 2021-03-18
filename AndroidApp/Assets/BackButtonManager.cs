using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class BackButtonManager
    {
        private List<int> loadedlayouts;
        private int currentpointer;
        public BackButtonManager()
        {
            loadedlayouts = new List<int>();
            loadedlayouts.Add(0);
            currentpointer = -1;
        }
        public void RecordScreenID(int id)
        {
            currentpointer++;
            if (loadedlayouts.Count < currentpointer + 1) 
            {
                loadedlayouts.Add(0);
            }
            loadedlayouts[currentpointer] = id;
        }
        public int GetCurrentID()
        {
            return loadedlayouts[currentpointer];
        }
        public int GetProviousID()
        {
            currentpointer--;
            if (currentpointer > 0)
            {
                return loadedlayouts[currentpointer];
            }
            else
            {
                Platform.CurrentActivity.FinishAndRemoveTask();
                return 0;
            }
        }
    }
}