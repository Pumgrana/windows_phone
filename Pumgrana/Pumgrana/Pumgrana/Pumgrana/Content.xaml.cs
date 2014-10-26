using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Data;
using System.Threading;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Threading.Tasks;

namespace Pumgrana
{
    public partial class ShowContent : PhoneApplicationPage
    {
        DT_ShowContent DT = new DT_ShowContent();
        PumgranaWebClient wc = new PumgranaWebClient();
        ApplicationBarIconButton RefreshContentButton { get; set; }

        private bool IsRefreshing { get; set; }

        public ShowContent()
        {
            InitializeComponent();
            RefreshContentButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            wc.Error += wc_Error;

            DT_ShowContent dt_local = IsolatedStorageOperations.Load<DT_ShowContent>("Content.xml");
            if (dt_local.listContent.Count > 0)
            {
                this.DT = dt_local;
            }
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            this.ListContent.DataContext = this.DT;
            GetContentFromServer();
            GetAllTags();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DT.listContent.Clear();
            this.DT.listTag.Clear();

            this.RefreshContentButton.IsEnabled = false;
            base.OnNavigatedFrom(e);
        }

        void wc_Error(object sender, object output)
        {
            Exception e = output as Exception;
            MessageBox.Show(e.Message);

            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            this.RefreshContentButton.IsEnabled = true;
        }

        private void GetAllTags()
        {
            wc.GetTagsByType += wc_GetTagsByType;
            wc.getTagsByType();
        }

        void wc_GetTagsByType(object sender, object output)
        {
            wc.GetTagsByType -= wc_GetTagsByType;
            this.DT.listTag.Clear();
            ListTag lt = output as ListTag;

            foreach (Tag t in lt.tags)
            {
                TagToApply ToAdd = new TagToApply();
                ToAdd.IsChecked = true;
                ToAdd.Id = t.uri;
                ToAdd.title = t.subject;
                this.DT.listTag.Add(ToAdd);
            }
        }

        private void GetContentFromServer(List<string> tags_uri = null)
        {
            wc.GetContents += wc_GetContents;
            IsRefreshing = true;
            wc.getContents(tags_uri);
        }

        void wc_GetContents(object sender, object output)
        {
            wc.GetContents -= wc_GetContents;
            this.DT.listContent.Clear();
            ListContent lc = output as ListContent;

            foreach (Content c in lc.contents)
            {
                this.DT.NbOfContents += 1;
                this.DT.listContent.Add(c);
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCurrentPage), this.DT);
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            this.RefreshContentButton.IsEnabled = true;
            this.IsRefreshing = false;
        }

        private void SaveCurrentPage(object data)
        {
            IsolatedStorageOperations.Save<Pumgrana.DT_ShowContent>(data as Pumgrana.DT_ShowContent, "Content.xml");
        }

        private void ListContent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selector = sender as LongListSelector;
            Content c = selector.SelectedItem as Content;
            string id = c.uri;
            int index = selector.ItemsSource.IndexOf(selector.SelectedItem);
            this.NavigationService.Navigate(new Uri("/Article.xaml?id=" + id + "&Index=" + index.ToString(), UriKind.Relative));
        }

        private List<string> getSelectedTags()
        {
            List<string> tags_uri = new List<string>();
            foreach (TagToApply tag in this.DT.listTag.Where(tmp => tmp.IsChecked == true))
            {
                tags_uri.Add(tag.Id);
            }
            return (tags_uri);
        }

        private void RefreshContents()
        {
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            this.RefreshContentButton.IsEnabled = false;
            GetContentFromServer(getSelectedTags());
        }

        private void ClearCache_Event(object sender, System.EventArgs e)
        {
            IsolatedStorageOperations.ClearCache();
            this.DT.NbOfContents = 0;
            (this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).IsEnabled = false;
        }

        private void RefreshContent_Click(object sender, EventArgs e)
        {
            RefreshContents();
        }
    }
}