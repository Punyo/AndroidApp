using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Snackbar;
using System;
using Xamarin.Essentials;
using static Android.Views.View;
using static AndroidApp.Assets.RecyclerViewItemSwiper;

namespace AndroidApp.Assets
{
    public class RecyclerViewItemSwiper : ItemTouchHelper.SimpleCallback
    {
        private SimpleAdapter viewadapter;
        private RemovalAdapter1 removaladapter;


        private ColorDrawable drawable;

        private Canvas canvas;

        public delegate void OnSwipedEvent(DoublelineListStruct[] words);
        public event OnSwipedEvent OnSwipe;
        public RecyclerViewItemSwiper(int dragDirs, int swipeDirs,/*ref*/ SimpleAdapter adapter) : base(dragDirs, swipeDirs)
        {
            RemovalAdapter1 ra = adapter as RemovalAdapter1;
            if (ra == null)
            {
                viewadapter = adapter;
            }
            else
            {
                removaladapter = ra;
            }
            drawable = new ColorDrawable();
            drawable.Color = Color.Red;
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            //throw new NotImplementedException();
            return false;
        }

        public override int GetSwipeDirs(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            if (removaladapter != null)
            {
                if (removaladapter.SelectedElements.Count != 0)
                {
                    return 0;
                }
                else
                {
                    return base.GetDragDirs(recyclerView, viewHolder);
                }
            }
            return base.GetDragDirs(recyclerView, viewHolder);
        }

        public override void OnChildDrawOver(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
        {
            DrawOver(c, recyclerView, viewHolder, dX, dY, actionState, isCurrentlyActive);
        }

        private void DrawOver(Canvas c, RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, float dX, float dY, int actionState, bool isCurrentlyActive)
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
            Swiped(viewHolder);
        }

        private void Swiped(RecyclerView.ViewHolder viewHolder)
        {
            int indexbackup = viewHolder.AdapterPosition;
            if (removaladapter == null)
            {
                DoublelineListStruct backup = viewadapter.Element[viewHolder.AdapterPosition];
                viewadapter.RemoveAt(viewHolder.AdapterPosition);
                Snackbar s = Snackbar.Make(viewHolder.ItemView.RootView, $"{backup.Title}を削除しました", Snackbar.LengthLong);
                s.AddCallback(new SnackBarCallBack(() => { OnSwipe?.Invoke(viewadapter.Element); }));
                s.SetAction("削除を取り消す", (v) => { viewadapter.Insert(indexbackup, backup); });
                s.Show();
            }
            else
            {
                DoublelineListStruct backup = removaladapter.Element[viewHolder.AdapterPosition];           
                removaladapter.RemoveAt(viewHolder.AdapterPosition);
                Snackbar s = Snackbar.Make(viewHolder.ItemView.RootView, $"{backup.Title}を削除しました", Snackbar.LengthLong);
                s.AddCallback(new SnackBarCallBack(() => { OnSwipe?.Invoke(removaladapter.Element); }));
                s.SetAction("削除を取り消す", (v) => { removaladapter.Insert(indexbackup, backup); });
                s.Show();
            }
        }

        private class SnackBarCallBack : BaseTransientBottomBar.BaseCallback
        {
            public delegate void OnDissmissedEvent();
            private event OnDissmissedEvent OnDissmissed_Event;
            public SnackBarCallBack(OnDissmissedEvent e = null)
            {
                if (e != null)
                {
                    OnDissmissed_Event += e;
                }
            }
            public override void OnDismissed(Java.Lang.Object transientBottomBar, int e)
            {
                base.OnDismissed(transientBottomBar, e);
                OnDissmissed_Event?.Invoke();
            }
        }
    }
}