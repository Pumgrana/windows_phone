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
        PumgranaDataContext DT = new PumgranaDataContext();
        PumgranaWebClient wc = new PumgranaWebClient();
        public ShowContent()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DT.listContent.Clear();
            this.Mode = ShowContentMode.SHOW;
            this.ListContent.DataContext = this.DT;
            this.DT.ToDisplay = "Show Mode";
            GetContentFromServer();
        }

        private void GetContentFromServer()
        {
            wc.GetContents += wc_GetContents;
            wc.getContents();
        }

        void wc_GetContents(object sender, object output)
        {
            this.DT.listContent.Clear();
            ListContent lc = output as ListContent;
            foreach (Content c in lc.contents)
            {
                this.DT.listContent.Add(c);
            }
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

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock box = sender as TextBlock;
            box.DataContext = this.DT;
        }
    }
}