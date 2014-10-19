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
using System.Windows.Documents;

namespace Pumgrana
{
    public partial class Article : PhoneApplicationPage
    {
        DT_Article DT = new DT_Article();
        PumgranaWebClient wc = new PumgranaWebClient();
        List<int> IdArticles = new List<int>();
        string id = "";
        int IndexArticle = 0;
        public Article()
        {
            InitializeComponent();
            wc.Error += wc_Error;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string index = "";
            NavigationContext.QueryString.TryGetValue("id", out id);
            NavigationContext.QueryString.TryGetValue("Index", out index);
            IndexArticle = System.Convert.ToInt32(index);
            if (IdArticles.Contains(IndexArticle) == true)
            {
                this.DT = (IsolatedStorageOperations.Load<DT_Article>(id + ".xml"));
            }
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;

            LoadArticle();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            wc.Error -= wc_Error;
            //wc.AbortConnection();
            base.OnNavigatedFrom(e);
        }

        private void LoadArticle()
        {
            GetDetailsFromId();
            GetTagsFromId();
            GetLinkedArticlesFromId();
        }

        void wc_Error(object sender, object output)
        {
            Exception e = output as Exception;
            MessageBox.Show(e.Message);
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
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
            this.ContentPanoramaItem.DataContext = res;
            PhoneApplicationService.Current.State["Article_Content"] = ContentPanoramaItem.DataContext;
            this.DT.content = res;
            this.TitleOfArticle.DataContext = this.DT.content;

            string html = "<html><head><meta charset=\"utf-8\"/></head><body>" + res.body + "</body></html>";
            this.ArticleWebView.NavigateToString(html);
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
            this.TagsPanoramaItem.DataContext = lt;
            PhoneApplicationService.Current.State["Article_Tag"] = TagsPanoramaItem.DataContext;
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
            this.LinkedPanoramaItem.DataContext = ll;
            PhoneApplicationService.Current.State["Article_Link"] = LinkedPanoramaItem.DataContext;
            this.DT.listLink = ll;

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCurrentPage), this.DT);
        }

        private void SaveCurrentPage(object state)
        {
            IsolatedStorageOperations.Save<DT_Article>(this.DT, IndexArticle.ToString() + ".xml");
            if (IdArticles.Contains(IndexArticle) == false)
            {
                IdArticles.Add(IndexArticle);
            }
        }

        private void ApplicationBarIconButton_Click_1(object sender, System.EventArgs e)
        {
            (sender as ApplicationBarIconButton).IsEnabled = false;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            LoadArticle();
        }

        private void ArticleWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            (this.ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;
            Link l = list.SelectedItem as Link;

            wc.GetLinkDetailFromUri += wc_GetLinkDetailFromUri;
            wc.getLinkDetailFromUri(l.link_uri);
        }

        void wc_GetLinkDetailFromUri(object sender, object output)
        {
            wc.GetLinkDetailFromUri -= wc_GetLinkDetailFromUri;
            FullLinkList data = output as FullLinkList;
            NavigationService.Navigate(new Uri("/Article.xaml?id=" + data.links[0].target_uri, UriKind.Relative));
        }
    }
}