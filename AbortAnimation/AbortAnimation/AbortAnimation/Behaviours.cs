﻿using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AbortAnimation
{
    [Preserve(AllMembers = true)]
    public class Behaviours : Behavior<SfListView>
    {
        private SfListView listView;
        private SearchBar searchBar = null;
        protected override void OnAttachedTo(BindableObject bindable)
        {
            listView = bindable as SfListView;
            listView.ItemGenerator = new ItemGenerator(listView);
            searchBar = (bindable as SfListView).FindByName<SearchBar>("filterText");
            searchBar.TextChanged += SearchBar_TextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(BindableObject bindable)
        {
            listView = null;
            searchBar = null;
            searchBar.TextChanged -= SearchBar_TextChanged;
            base.OnDetachingFrom(bindable);
        }
        #region Private Methods
        private bool FilterContacts(object obj)
        {
            if (searchBar == null || searchBar.Text == null)
                return true;

            var contacts = obj as Contacts;
            if (contacts.ContactName.ToLower().Contains(searchBar.Text.ToLower())
                || contacts.ContactName.ToLower().Contains(searchBar.Text.ToLower()))
                return true;
            else
                return false;
        }

        #endregion

        #region CallBacks


        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchBar = (sender as SearchBar);
            if (listView.DataSource != null)
            {
                this.listView.DataSource.Filter = FilterContacts;
                this.listView.DataSource.RefreshFilter();
            }
        }
        #endregion

    }

    public class ItemGeneratorExt : ItemGenerator
    {
        public SfListView ListView { get; set; }
        public ItemGeneratorExt(SfListView listview) : base(listview)
        {
            ListView = listview;
        }
        protected override ListViewItem OnCreateListViewItem(int itemIndex, ItemType type, object data = null)
        {
            if (type == ItemType.Record)
                return new ListViewItemExt(ListView);
            return base.OnCreateListViewItem(itemIndex, type, data);
        }

    }
    public class ListViewItemExt : ListViewItem
    {
        private SfListView _listView;

        public ListViewItemExt(SfListView listView)
        {
            _listView = listView;
            this.PropertyChanged += ListViewItemExt_PropertyChanged;
        }

        private void ListViewItemExt_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visibility")
            {
                if (!this.Visibility)
                    this.AbortAnimation("FadeTo");
            }
        }

        protected override void OnItemAppearing()
        {
            this.Opacity = 0;
            this.FadeTo(1, 400, Easing.SinInOut);
            base.OnItemAppearing();
        }

    }
}
