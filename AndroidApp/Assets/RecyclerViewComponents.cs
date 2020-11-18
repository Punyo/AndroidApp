﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Assets
{
    class RecyclerViewComponents
    {
        public static void CreateDoublelineList(DoublelineListStruct[] words, Activity activity, LinearLayout view, RecyclerViewItemSwiper.OnSwipedEvent onswipe = null, EventHandler<Adapter1ClickEventArgs> clickevent = null)
        {
            Adapter1 adapter = new Adapter1(words);
            IntlList(ref activity, ref view, ref adapter, onswipe);
            if (clickevent.Target != null)
            {
                adapter.ItemClick += clickevent;
            }
        }

        public static void CreateDoublelineList(string[] titles, string[] description, Activity activity, LinearLayout view, RecyclerViewItemSwiper.OnSwipedEvent onswipe = null, EventHandler<Adapter1ClickEventArgs> clickevent = null)
        {
            Adapter1 adapter = new Adapter1(titles, description);
            IntlList(ref activity, ref view, ref adapter, onswipe);
            if (clickevent.Target != null)
            {
                adapter.ItemClick += clickevent;
            }
        }

        public static void CreateDoublelineList(DoublelineListStruct[] words, Activity activity, int recyclerviewid, RecyclerViewItemSwiper.OnSwipedEvent onswipe = null, EventHandler<Adapter1ClickEventArgs> clickevent = null)
        {
            Adapter1 adapter = new Adapter1(words);
            IntlList(ref activity,recyclerviewid, ref adapter, onswipe);
            if (clickevent != null)
            {
                adapter.ItemClick += clickevent;
            }
        }

        public static void CreateDoublelineList(string[] titles, string[] description, Activity activity,int recyclerviewid, RecyclerViewItemSwiper.OnSwipedEvent onswipe = null, EventHandler<Adapter1ClickEventArgs> clickevent = null)
        {
            Adapter1 adapter = new Adapter1(titles, description);
            IntlList(ref activity,recyclerviewid, ref adapter, onswipe);
            if (clickevent.Target != null)
            {
                adapter.ItemClick += clickevent;
            }
        }

        private static void IntlList(ref Activity activity, ref LinearLayout maincontentlayout, ref Adapter1 adapter, RecyclerViewItemSwiper.OnSwipedEvent onswipe = null)
        {
            maincontentlayout.RemoveAllViews();
            activity.LayoutInflater.Inflate(Resource.Layout.recycler_view, maincontentlayout);
            RecyclerView recyclerView = activity.FindViewById<RecyclerView>(Resource.Id.list_view);
            LinearLayoutManager manager = new LinearLayoutManager(activity);
            recyclerView.SetLayoutManager(manager);
            recyclerView.SetAdapter(adapter);
            RecyclerView.ItemDecoration deco = new DividerItemDecoration(activity, DividerItemDecoration.Vertical);
            recyclerView.AddItemDecoration(deco);
            if (onswipe != null)
            {
                RecyclerViewItemSwiper swiper = new RecyclerViewItemSwiper(ItemTouchHelper.Left, ItemTouchHelper.Left, ref adapter);
                swiper.OnSwipe += onswipe;
                var item = new ItemTouchHelper(swiper);
                item.AttachToRecyclerView(recyclerView);
            }
        }
        private static void IntlList(ref Activity activity, int recyclerviewId, ref Adapter1 adapter, RecyclerViewItemSwiper.OnSwipedEvent onswipe = null)
        {
            RecyclerView recyclerView = activity.FindViewById<RecyclerView>(recyclerviewId);
            LinearLayoutManager manager = new LinearLayoutManager(activity);
            recyclerView.SetLayoutManager(manager);
            recyclerView.SetAdapter(adapter);
            RecyclerView.ItemDecoration deco = new DividerItemDecoration(activity, DividerItemDecoration.Vertical);
            recyclerView.AddItemDecoration(deco);
            if (onswipe != null)
            {
                RecyclerViewItemSwiper swiper = new RecyclerViewItemSwiper(ItemTouchHelper.Left, ItemTouchHelper.Left, ref adapter);
                swiper.OnSwipe += onswipe;
                var item = new ItemTouchHelper(swiper);
                item.AttachToRecyclerView(recyclerView);
            }
        }

    }
}