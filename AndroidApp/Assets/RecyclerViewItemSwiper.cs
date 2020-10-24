using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;

namespace AndroidApp.Assets
{
    public class RecyclerViewItemSwiper : ItemTouchHelper.SimpleCallback
    {
        private Adapter1 viewadapter;


        private ColorDrawable drawable;

        private Canvas canvas;

        public delegate void OnSwipedEvent();
        public event OnSwipedEvent OnSwipe;
        public RecyclerViewItemSwiper(int dragDirs, int swipeDirs, Adapter1 adapter) : base(dragDirs, swipeDirs)
        {
            viewadapter = adapter;
            drawable = new ColorDrawable();
            drawable.Color = Color.Red;
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            //throw new NotImplementedException();
            return false;
        }

        public override void OnChildDrawOver(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            base.OnChildDrawOver(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
            drawable.SetBounds(viewHolder.ItemView.Right + (int)dX, viewHolder.ItemView.Top, viewHolder.ItemView.Right, viewHolder.ItemView.Bottom);
            drawable.Draw(c);
            if (canvas != c)
            {
                canvas = c;
            }
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            //canvas.DrawRect(viewHolder.ItemView.Left, viewHolder.ItemView.Top,viewHolder.ItemView.Right, viewHolder.ItemView.Bottom,);
            viewadapter.RemoveAt(viewHolder.AdapterPosition);
            //OnSwipe?.Invoke();
        }
    }
}