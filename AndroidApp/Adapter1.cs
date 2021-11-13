using System;

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace AndroidApp.Assets
{
    public class SimpleAdapter : RecyclerView.Adapter
    {
        public event EventHandler<Adapter1ClickEventArgs> ItemClick;
        public event EventHandler<Adapter1ClickEventArgs> ItemLongClick;
        public DoublelineListStruct[] Element;

        public SimpleAdapter(DoublelineListStruct[] words)
        {
            Element = words;
        }

        public SimpleAdapter(string[] title, string[] description)
        {
            DoublelineListStruct[] words = new DoublelineListStruct[title.Length];
            for (int i = 0; i < words.Length; i++)
            {
                words[i].Title = title[i];
                words[i].Description = description[i];
            }
            Element = words;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.recyclerview_element;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new Adapter1ViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Element[position].Title;

            // Replace the contents of the view with that element
            var holder = viewHolder as Adapter1ViewHolder;
            holder.Title.Text = Element[position].Title;
            holder.Description.Text = Element[position].Description;
        }

        public void RemoveAt(int index)
        {
            //Word[index] = new DoublelineListStruct();
            for (int i = index; i < Element.Length; i++)
            {
                if (Element.Length - 1 != i)
                {
                    Element[i] = Element[i + 1];

                }
                else
                {
                    Array.Resize(ref Element, Element.Length - 1);
                }
            }
            this.NotifyItemRemoved(index);
            this.NotifyItemRangeChanged(index, Element.Length);
        }
        public void Insert(int index, DoublelineListStruct item)
        {
            Array.Resize(ref Element, Element.Length + 1);
            for (int i = Element.Length - 2; i > index - 1; i--)
            {
                Element[i + 1] = Element[i];
            }
            Element[index] = item;
            //this.NotifyItemRemoved(index);
            this.NotifyItemRangeChanged(index, Element.Length);
        }
        public override int ItemCount => Element.Length;

        void OnClick(Adapter1ClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Adapter1ClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class Adapter1ViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Description { get; set; }


        public Adapter1ViewHolder(View itemView, Action<Adapter1ClickEventArgs> clickListener,
                            Action<Adapter1ClickEventArgs> longClickListener) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.maintitle);
            Description = itemView.FindViewById<TextView>(Resource.Id.description);
            itemView.Click += (sender, e) => clickListener(new Adapter1ClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new Adapter1ClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class Adapter1ClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }

    }
}