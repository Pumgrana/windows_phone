using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace StartPumgrana
{
    public partial class Article : PhoneApplicationPage
    {
        public Article()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string guid = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("guid", out guid))
            {
                //guid exists therefore it's a reload, so delete the last entry
                //from the navigation stack
                if (NavigationService.CanGoBack)
                    NavigationService.RemoveBackEntry();
            }
            PContent P;
            if (PhoneApplicationService.Current.State.ContainsKey("ArticleFromLink") == true)
            {
                P = PhoneApplicationService.Current.State["ArticleFromLink"] as PContent;
                PhoneApplicationService.Current.State.Remove("ArticleFromLink");
            }
            else
            {
                P = PhoneApplicationService.Current.State["CurrentPage"] as PContent;
            }
            this.PivotContent.Title = P.Name;

            ObservableCollection<string> ToDisplay = P.ListText;
            foreach (string s in ToDisplay)
            {
                this.DisplayContent.Text += s;
            }

            ObservableCollection<Tag> Tags = P.ListTags;
            foreach (Tag T in Tags)
            {
                TextBlock DisplayTag = new TextBlock();
                DisplayTag.Text = T.Name;

                this.StackTags.Children.Add(DisplayTag);

            }

            List<PThem> DB = IsolatedStorageSettings.ApplicationSettings["DataBase"] as List<PThem>;
            ObservableCollection<PContent> ListLinks = new ObservableCollection<PContent>();

            foreach (PThem TmpThem in DB)
            {
                foreach (Tag DBTag in TmpThem.ListTags)
                {
                    foreach (Tag ContentTag in P.ListTags)
                    {
                        if (DBTag.Name == ContentTag.Name)
                        {
                            foreach (PContent TmpContent in TmpThem.ListContent)
                            {
                                ListLinks.Add(TmpContent);
                            }
                        }
                    }
                }
            }
            foreach (PContent TmpContent in ListLinks)
            {
                HyperlinkButton ToAdd = new HyperlinkButton();
                ToAdd.Content = TmpContent.Name;
                ToAdd.NavigateUri = new Uri(String.Format("/Article.xaml?guid={0}", System.Guid.NewGuid().ToString()), UriKind.Relative);
                ToAdd.Tag = TmpContent;
                ToAdd.Click += new RoutedEventHandler(B_Click);
                this.StackLinks.Children.Add(ToAdd);
            }
            base.OnNavigatedTo(e);
        }

        void B_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton B = sender as HyperlinkButton;
            NavigationService.NavigationFailed += new System.Windows.Navigation.NavigationFailedEventHandler(NavigationService_NavigationFailed);
            PhoneApplicationService.Current.State["ArticleFromLink"] = B.Tag;
            NavigationService.Navigate(B.NavigateUri);
        }

        void NavigationService_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            string s = e.Exception.Message;

            MessageBox.Show(s);
        }
    }
}