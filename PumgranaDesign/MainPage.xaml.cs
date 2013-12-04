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
using System.Collections.ObjectModel;

namespace PumgranaDesign
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<PumgranaDesign.PumgranaContent> Contents;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Contents = new List<PumgranaContent>();
            this.Contents.Add(new PumgranaContent
            {
                Title = "League of Legends",
                ListTags = new ObservableCollection<string>
                {
                    "Jeux vidéo",
                    "E sport"
                }
            });
            this.ViewContents.ItemsSource = this.Contents;
        }
    }
}