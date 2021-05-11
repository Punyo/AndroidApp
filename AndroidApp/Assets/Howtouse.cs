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
    class Howtouse
    {
        private List<ExpandableCardView> cardViews = new List<ExpandableCardView>();
        public Howtouse()
        {
            ExpandableCardView cardView1 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox1), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body1));
            ExpandableCardView cardView2 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox2), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body2));
            ExpandableCardView cardView3 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox3), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body3));
            ExpandableCardView cardView4 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox4), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body4));
            ExpandableCardView cardView5 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox5), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body5));
            //ExpandableCardView cardView6 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox6), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body6));
            //ExpandableCardView cardView7 = new ExpandableCardView(Platform.CurrentActivity.FindViewById<CheckBox>(Resource.Id.howtouse_checkBox7), Platform.CurrentActivity.FindViewById<TextView>(Resource.Id.howtouse_body7));
            cardViews.Add(cardView1);
            cardViews.Add(cardView2);
            cardViews.Add(cardView3);
            cardViews.Add(cardView4);
            cardViews.Add(cardView5);
            //cardViews.Add(cardView6);
            //cardViews.Add(cardView7);
            foreach (var item in cardViews)
            {
                item.CheckBox.CheckedChange += CheckBox_CheckedChange;
                item.CheckBox.Checked = false;
                ChangeBodyState(item.CheckBox, false);
            }
        }

        private void CheckBox_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            ChangeBodyState(sender as CheckBox, e.IsChecked);
        }

        private void ChangeBodyState(CheckBox sender, bool e)
        {
            if (e)
            {
                cardViews.Find((d) => d.CheckBox == sender).Body.Visibility = ViewStates.Gone;
            }
            else
            {
                cardViews.Find((d) => d.CheckBox == sender).Body.Visibility = ViewStates.Visible;
            }
        }
    }
    public class ExpandableCardView
    {
        public CheckBox CheckBox { get; }
        public TextView Body { get; }
        public ExpandableCardView(CheckBox checkbox, TextView body)
        {
            CheckBox = checkbox;
            Body = body;
        }
    }

}