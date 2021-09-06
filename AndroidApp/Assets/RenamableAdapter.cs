using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace AndroidApp.Assets
{
    class RenamableAdapter : SimpleAdapter
    {
        private MainActivity act;
        private int renameingindex;
        private DoubleTextBoxFragment doubleTextBox;
        private bool isdialogopen;
        public RenamableAdapter(DoublelineListStruct[] words, MainActivity activity) : base(words)
        {
            act = activity;
        }

        public RenamableAdapter(string[] title, string[] description) : base(title, description)
        {
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            base.OnBindViewHolder(viewHolder, position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            this.ItemLongClick += RenamableAdapter_ItemLongClick;
            return base.OnCreateViewHolder(parent, viewType);
        }

        private void RenamableAdapter_ItemLongClick(object sender, Adapter1ClickEventArgs e)
        {
            if (isdialogopen)
            {
                return;
            }
            isdialogopen = true;
            doubleTextBox = new DoubleTextBoxFragment(Rename);
            doubleTextBox.dismiss += () => { isdialogopen = false; };
            doubleTextBox.Show(act.SupportFragmentManager.BeginTransaction(), "rename");
            renameingindex = e.Position;
        }

        private void Rename(EditText title, EditText desc)
        {
            GenreStruct newgenre = act.Genrelist[act.Genreid];
            DoublelineListStruct newtext = new DoublelineListStruct();
            for (int i = 0; i < act.Genrelist[act.Genreid].Words.Count; i++)
            {
                if (act.Genrelist[act.Genreid].Words[i].Title == Word[renameingindex].Title)
                {
                    if (!string.IsNullOrWhiteSpace(title.Text))
                    {
                        newtext.Title = title.Text;
                    }
                    else
                    {
                        newtext.Title = newgenre.Words[i].Title;
                    }
                    if (!string.IsNullOrWhiteSpace(desc.Text))
                    {
                        newtext.Description = desc.Text;
                    }
                    else
                    {
                        newtext.Description = newgenre.Words[i].Description;
                    }
                    newgenre.Words[i] = newtext;
                }
            }
            WordManager.WriteWordlist(WordManager.GetInternalSavePath(Path.Combine(act.Genrelist[act.Genreid].GenreName + GenreFragment.TAG, MainActivity.SAVEDATANAME)), newgenre.Words.ToArray());
            act.EditGenre(act.Genreid, newgenre);
        }
    }
}