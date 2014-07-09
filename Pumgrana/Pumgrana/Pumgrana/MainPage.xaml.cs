using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pumgrana.Resources;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;

namespace Pumgrana
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            /*Création de la liste de stockage des pages visitées (mis en cache des pages)*/
            if (IsolatedStorageSettings.ApplicationSettings.Contains(IsolatedStorageOperations.InputCacheName) == false)
            {
                IsolatedStorageSettings.ApplicationSettings[IsolatedStorageOperations.InputCacheName] = new List<string>();
            }
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Content.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void ClearCacheButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            /*Suppression de tout les fichiers mis en cache précedemment*/
            List<string> list = IsolatedStorageSettings.ApplicationSettings[IsolatedStorageOperations.InputCacheName] as List<string>;
            foreach (string file in list)
            {
                System.Diagnostics.Debug.WriteLine("Delete " + file);
                IsolatedStorageFile.GetUserStoreForApplication().DeleteFile(file);
            }
            list.Clear();
        }
    }
}