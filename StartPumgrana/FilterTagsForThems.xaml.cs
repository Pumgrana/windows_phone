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

namespace StartPumgrana
{
    public partial class FilterTagsForThems : PhoneApplicationPage
    {
        private PThem Them
        {
            get;
            set;
        }

        public FilterTagsForThems()
        {
            InitializeComponent();

            Them = PhoneApplicationService.Current.State["CurrentPage"] as PThem;

            this.Loaded += new RoutedEventHandler(FilterTagsForThems_Loaded);
        }

        void FilterTagsForThems_Loaded(object sender, RoutedEventArgs e)
        {            
            for (int i = 0 ; i < Them.ListContent.Count; i++)
            {
                PContent P = Them.ListContent[i];

                for (int j = 0; j < P.ListTags.Count; j++)
                {
                    Tag T = P.ListTags[j];
                    bool ok = true;
                    CheckBox ToAdd = new CheckBox();
                    ToAdd.Name = T.Name;
                    ToAdd.IsChecked = T.Applicated;
                    ToAdd.Content = T.Name;

                    foreach (Tag TmpTag in Them.ListTags)
                    {
                        if (TmpTag.Name == T.Name)
                            ok = false;
                    }
                    if (ok == true)
                        this.ContentPanel.Children.Add(ToAdd);
                }
            }
            
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (PContent p in Them.ListContent)
            {
                foreach (Tag t in p.ListTags)
                {
                    CheckBox    Check = this.ContentPanel.FindName(t.Name) as CheckBox;
                    if (Check != null)
                        t.Applicated = Check.IsChecked.Value;
                }
            }
            PhoneApplicationService.Current.State["CurrentPage"] = Them;
            this.NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}