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

namespace Pumgrana
{
    public partial class ShowContent : PhoneApplicationPage
    {
        private enum ShowContentMode
        {
            SHOW = 0,
            DELETE
        }
        private ShowContentMode Mode { get; set; }
        DT_ShowContent DT = new DT_ShowContent();
        PumgranaWebClient wc = new PumgranaWebClient();
        public ShowContent()
        {
            InitializeComponent();
            wc.Error += wc_Error;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ListContent lc = IsolatedStorageOperations.Load<ListContent>("Content.xml");
            if (lc.contents.Count > 0)
            {
                foreach (Content c in lc.contents)
                {
                    this.DT.listContent.Add(c);
                }
            }
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            this.Mode = ShowContentMode.SHOW;
            this.ListContent.DataContext = this.DT;
            this.DT.ToDisplay = "Show Mode";
            GetContentFromServer();
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DT.listContent.Clear();
            base.OnNavigatedFrom(e);
        }

        void wc_Error(object sender, object output)
        {
            Exception e = output as Exception;
            MessageBox.Show(e.Message);

            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void GetContentFromServer()
        {
            wc.GetContents += wc_GetContents;
            wc.getContents();
        }

        void wc_GetContents(object sender, object output)
        {
            wc.GetContents -= wc_GetContents;
            this.DT.listContent.Clear();
            ListContent lc = output as ListContent;

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCurrentPage), lc);

            foreach (Content c in lc.contents)
            {
                this.DT.NbOfContents += 1;
                this.DT.listContent.Add(c);
            }

            (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SaveCurrentPage(object lc)
        {
            IsolatedStorageOperations.Save<ListContent>(lc as ListContent, "Content.xml");
        }

        private void ListContent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Content c = (sender as LongListSelector).SelectedItem as Content;
            string id = c._id;
            if (this.Mode == ShowContentMode.SHOW)
            {
                this.NavigationService.Navigate(new Uri("/Article.xaml?id=" + id, UriKind.Relative));
            }
            else
            {
                wc.DeleteContents += wc_DeleteContents;
                wc.DeleteContent(id);
            }
        }

        void wc_DeleteContents(object sender, object output)
        {
            RequestObject info = output as RequestObject;
            if (info.status == 200)
            {
                this.DT.listContent.Remove(this.ListContent.SelectedItem as Content);
                MessageBox.Show("Content deleted.");
            }
            else
            {
                MessageBox.Show("Failed to delete the content. Error : " + info.error);
            }
            wc.DeleteContents -= wc_DeleteContents;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditContent.xaml?Param=Add", UriKind.Relative));
        }

        private void ApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            if (this.Mode == ShowContentMode.SHOW)
            {
                this.Mode = ShowContentMode.DELETE;
                DT.ToDisplay = "Delete Mode";
            }
            else
            {
                this.Mode = ShowContentMode.SHOW;
                DT.ToDisplay = "Show Mode";
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, System.EventArgs e)
        {
            ApplicationBarIconButton button = sender as ApplicationBarIconButton;
            button.IsEnabled = false;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            GetContentFromServer();
        }
    }
}