using System;

using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Xamarin.Essentials;
using Android.App;
using System.Collections.ObjectModel;
using AndroidX.RecyclerView.Widget;

namespace AndroidApp.Assets
{
    public class RemovalAdapter1 : SimpleAdapter
    {
        public new event EventHandler<RemovalAdapterClickEventArgs> ItemClick;
        public new event EventHandler<RemovalAdapterClickEventArgs> ItemLongClick;
        public delegate void OnRemoveExcutedEvent(DoublelineListStruct[] words);
        public delegate void OnRemoveMode();
        public delegate void OnSelectedElementsChanged();
        public event OnRemoveExcutedEvent OnRemoveExcuted;
        public event OnRemoveMode OnRemoveModeEnter;
        public event OnRemoveMode OnRemoveModeExit;
        public event OnSelectedElementsChanged OnSelectChanged;

        private List<int> selectingelements;
        private List<RadioButton> selectingradios;
        public ReadOnlyCollection<int> SelectedElements
        {
            get
            {
                return selectingelements.AsReadOnly();
            }
        }
        //private List<Removal1AdapterViewHolder> holders;

        public RemovalAdapter1(DoublelineListStruct[] words) : base(words)
        {
            Element = words;
            selectingelements = new List<int>();
            selectingradios = new List<RadioButton>();
            //holders = new List<Removal1AdapterViewHolder>();
        }

        public RemovalAdapter1(string[] title, string[] description) : base(title, description)
        {
            DoublelineListStruct[] words = new DoublelineListStruct[title.Length];
            for (int i = 0; i < words.Length; i++)
            {
                words[i].Title = title[i];
                words[i].Description = description[i];
            }
            Element = words;
            selectingelements = new List<int>();
            //holders = new List<Removal1AdapterViewHolder>();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.recyclerview_element_removal;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);
            var vh = new Removal1AdapterViewHolder(itemView, OnClick, OnLongClick);
            //vh.Radio.Checked = false;
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Element[position].Title;

            // Replace the contents of the view with that element
            var holder = viewHolder as Removal1AdapterViewHolder;
            holder.Title.Text = Element[position].Title;
            holder.Description.Text = Element[position].Description;
            if (!selectingelements.Contains(position))
            {
                holder.Radio.Visibility = ViewStates.Gone;
            }
            else
            {
                holder.Radio.Visibility = ViewStates.Visible;
                holder.Radio.Checked = true;
            }
            //holders.Add(holder);
        }

        public override int ItemCount => Element.Length;

        void OnClick(RemovalAdapterClickEventArgs args)
        {
            if (selectingelements.Count > 0)
            {
                if (args.Radio.Checked == false)
                {
                    args.Radio.Visibility = ViewStates.Visible;
                    selectingelements.Add(args.Position);
                    args.Radio.Checked = true;
                }
                else
                {
                    selectingelements.Remove(args.Position);
                    args.Radio.Visibility = ViewStates.Gone;
                    if (selectingelements.Count == 0)
                    {
                        //foreach (var item in holders)
                        //{
                        //    item.Radio.Visibility = ViewStates.Gone;
                        //}
                        OnRemoveModeExit?.Invoke();
                    }
                    args.Radio.Checked = false;
                }
            }
            else
            {
                ItemClick?.Invoke(this, args);
            }
        }
        void OnLongClick(RemovalAdapterClickEventArgs args)
        {
            if (args.Radio.Checked == false)
            {
                selectingelements.Add(args.Position);
                if (selectingelements.Count == 1)
                {
                    //foreach (var item in holders)
                    //{
                    //    item.Radio.Visibility = ViewStates.Visible;
                    //}
                    OnRemoveModeEnter?.Invoke();
                }
                args.Radio.Visibility = ViewStates.Visible;
                args.Radio.Checked = true;

            }
            ItemLongClick?.Invoke(this, args);
        }
   
        public void ExcuteRemove()
        {
            Activity main = Platform.CurrentActivity;
            DialogComponents.ShowWarning("警告",
                main.Resources.GetString(Resource.String.dialog_deleteelements).Replace("[SELECTINGELEMENTSCOUNT]", selectingelements.Count.ToString()), main, (a, v) =>
                {
                    selectingelements.Sort();
                    for (int i = 0; i < selectingelements.Count; i++)
                    {
                        RemoveAt(selectingelements[i] - i);
                    }
                    foreach (var item in selectingradios)
                    {
                        item.Checked = false;
                        item.Visibility = ViewStates.Gone;
                    }
                    selectingelements.Clear();
                    OnRemoveExcuted?.Invoke(Element);
                }, (a, v) => { }, main.Resources.GetString(Resource.String.dialog_delele), main.Resources.GetString(Resource.String.dialog_cancel));
        }
    }

    public class Removal1AdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Description { get; set; }
        public RadioButton Radio { get; set; }


        public Removal1AdapterViewHolder(View itemView, Action<RemovalAdapterClickEventArgs> clickListener,
                            Action<RemovalAdapterClickEventArgs> longClickListener) : base(itemView)
        {

            Title = itemView.FindViewById<TextView>(Resource.Id.maintitle_removal);
            Description = itemView.FindViewById<TextView>(Resource.Id.description_removal);
            Radio = itemView.FindViewById<RadioButton>(Resource.Id.radioButton_removal);

            itemView.Click += (sender, e) => clickListener(new RemovalAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Radio = Radio }); ;
            itemView.LongClick += (sender, e) => longClickListener(new RemovalAdapterClickEventArgs { View = itemView, Position = AdapterPosition, Radio = Radio });
        }
    }

    public class RemovalAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public RadioButton Radio { get; set; }
    }
}