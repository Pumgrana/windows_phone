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
using System.Collections.ObjectModel;
using System.Threading;

namespace Pumgrana
{
    public partial class Article : PhoneApplicationPage
    {
        DT_Article DT = new DT_Article();
        PumgranaWebClient wc = new PumgranaWebClient();
        string id = "";
        static private int IdArticle = 0;
        static private Dictionary<string, int> WebIdToLocalId = new Dictionary<string, int>();
        public Article()
        {
            InitializeComponent();
            wc.Error += wc_Error;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationContext.QueryString.TryGetValue("id", out id);
            
            if (WebIdToLocalId.ContainsKey(id) == true)
            {
                this.DT = IsolatedStorageOperations.Load<DT_Article>("Article_" + WebIdToLocalId.First(tmp => tmp.Key == id).Value.ToString() + ".xml");
            }
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;

            LoadArticle();

            base.OnNavigatedTo(e);
        }

        private void LoadArticle()
        {
            GetDetailsFromId();
            GetTagsFromId();
            GetLinkedArticlesFromId();
        }

        private void ArticlePivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            /*Pivot p = sender as Pivot;
            switch (p.SelectedIndex)
            {
                case (0):
                    {
                        GetDetailsFromId();
                        break;
                    }
                case (1):
                    {
                        GetTagsFromId();
                        break;
                    }
                case (2):
                    {
                        GetLinkedArticlesFromId();
                        break;
                    }
            }*/
        }

        void wc_Error(object sender, object output)
        {
            wc.Error -= wc_Error;
            Exception e = output as Exception;
            MessageBox.Show(e.Message);
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void GetDetailsFromId()
        {
            wc.GetDetail += wc_GetDetail;
            wc.getDetail(id);
        }

        void wc_GetDetail(object sender, object output)
        {
            wc.GetDetail -= wc_GetDetail;
            Content res = (output as ListContent).contents[0];
            this.ContentPivotItem.DataContext = res;
            PhoneApplicationService.Current.State["Article_Content"] = ContentPivotItem.DataContext;
            this.DT.content = res;
            string html = "<html><head><meta charset=\"utf-8\"/></head><body>" + res.body + "</body></html>";
            this.ArticleWebView.NavigateToString(html);
            //System.Diagnostics.Debug.WriteLine("WebBrowser actual size: " + this.ArticleWebView.ActualWidth.ToString() + ":" + this.ArticleWebView.ActualHeight.ToString());
        }

        private void GetTagsFromId()
        {
            wc.GetTagsFromContent += wc_GetTagsFromContent;
            wc.getTagsFromContent(id);
        }

        void wc_GetTagsFromContent(object sender, object output)
        {
            wc.GetTagsFromContent -= wc_GetTagsFromContent;
            ListTag lt = output as ListTag;
            if (lt.tags != null)
                DT.listTag.ToList().ForEach(lt.tags.Add);
            this.TagsPivotItem.DataContext = lt;
            PhoneApplicationService.Current.State["Article_Tag"] = TagsPivotItem.DataContext;
        }

        private void GetLinkedArticlesFromId()
        {
            wc.GetLinksFromContent += wc_GetLinksFromContent;
            wc.getLinksFromContent(id);
        }

        void wc_GetLinksFromContent(object sender, object output)
        {
            wc.GetLinksFromContent -= wc_GetLinksFromContent;
            ListLink ll = output as ListLink;
            this.LinkedPivotItem.DataContext = ll;
            PhoneApplicationService.Current.State["Article_Link"] = LinkedPivotItem.DataContext;
            this.DT.listLink = ll;

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCurrentPage), this.DT);
        }

        private void SaveCurrentPage(object state)
        {
            IsolatedStorageOperations.Save<DT_Article>(this.DT, "Article_" + IdArticle.ToString() + ".xml");
            if (WebIdToLocalId.ContainsKey(id) == false)
            {
                WebIdToLocalId.Add(id, IdArticle++);
            }
        }

        private void ApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditContent.xaml?Param=Edit", UriKind.Relative));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO : Implement
            MessageBox.Show("A implémenter !");
        }

        private void ApplicationBarIconButton_Click_1(object sender, System.EventArgs e)
        {
            (sender as ApplicationBarIconButton).IsEnabled = false;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            LoadArticle();
        }

        private void ArticleWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            (this.ApplicationBar.Buttons[1] as ApplicationBarIconButton).IsEnabled = true;
        }
    }
}