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
using System.Threading.Tasks;

namespace Pumgrana
{
    public partial class Article : PhoneApplicationPage
    {
        PumgranaDataContext DT = new PumgranaDataContext();
        PumgranaWebClient wc = new PumgranaWebClient();
        EventWaitHandle WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
        string id = "";
        public Article()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetDetailsFromId();
            GetTagsFromId();
            GetLinkedArticlesFromId();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationContext.QueryString.TryGetValue("id", out id);
            base.OnNavigatedTo(e);
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

        private void GetDetailsFromId()
        {
            wc.GetDetail += wc_GetDetail;
            wc.getDetail(id);
        }

        void wc_GetDetail(object sender, object output)
        {
            Content res = (output as ListContent).contents[0];
            this.ContentPivotItem.DataContext = res;
            PhoneApplicationService.Current.State["Article_Content"] = ContentPivotItem.DataContext;
        }


        private void GetTagsFromId()
        {
            wc.GetTagsFromContent += wc_GetTagsFromContent;
            wc.getTagsFromContent(id);
        }

        void wc_GetTagsFromContent(object sender, object output)
        {
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
            ListLink ll = output as ListLink;
            this.LinkedPivotItem.DataContext = ll;
            PhoneApplicationService.Current.State["Article_Link"] = LinkedPivotItem.DataContext;
        }

        private void ApplicationBarIconButton_Click(object sender, System.EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditContent.xaml?Param=Edit", UriKind.Relative));
        }
    }
}