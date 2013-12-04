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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

namespace StartPumgrana
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private List<PThem> listthems;
        public List<PThem> ListThems
        {
            get
            {
                return listthems;
            }
            set
            {
                NotifyPropertyChanged(ref listthems, value, "ListThems");
            }
        }
        static int refreshCount;
        public   ObservableCollection<Tag>       ListTagsForThem;
        // Constructor
        public MainPage()
        {
            this.Resources.Add("ConverterFromBool2Visibility", new StartPumgrana.BoolVisibilityConverter());
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            ListTagsForThem = new ObservableCollection<Tag>
            {
                new Tag{
                    Name = "Jeux Vidéo",
                    Applicated = false
                },
                new Tag{
                    Name = "Mangas",
                    Applicated = false
                },
                new Tag{
                    Name = "Musique",
                    Applicated = false
                },
                new Tag{
                    Name = "Film",
                    Applicated = false
                }
            };

            if (refreshCount++ > 0)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("DataBase") == true)
                    this.ListThems = IsolatedStorageSettings.ApplicationSettings["DataBase"] as List<PThem>;
                DataContext = this; 
                return;
            }
            ListThems = new List<PThem>()
            {
                new PThem{
                    Name = "Musique",
                    ListContent = new ObservableCollection<PContent>{
                        new PContent{
                        Name = "Eminem",
                        ListText = new ObservableCollection<string>
                        {
                            "Chansons",
                            "Biographie"
                        },
                        ListTags = new ObservableCollection<Tag>
                        {
                            new Tag
                            {
                                Name = "Rap",
                                Applicated = true
                            },
                            ListTagsForThem[2]
                        },
                        },
                        new PContent{
                            Name = "David Guetta",
                            ListText = new ObservableCollection<string>
                            {
                                "Un DJ francais",
                                "Ses duo"
                            },
                            ListTags = new ObservableCollection<Tag>
                            {
                                new Tag
                                {
                                    Name = "Electronique",
                                    Applicated = true
                                },
                                ListTagsForThem[2]
                            }
                        }
                    },
                    ListTags = new ObservableCollection<Tag>
                    {
                        new Tag
                        {
                            Name = "Musique",
                            Applicated = true
                        }
                    }
                },
                new PThem{
                    Name = "Film",
                    ListContent = new ObservableCollection<PContent>{
                                new PContent{
                                    Name = "Prisoners",
                                    ListText = new ObservableCollection<string>
                                    {
                                        "Casting",
                                        "Critique"
                                    },
                                    ListTags = new ObservableCollection<Tag>
                                    {
                                        new Tag
                                        {
                                            Name = "Thriller",
                                            Applicated = true
                                        },
                                        ListTagsForThem[3]
                                    }
                                },
                                new PContent{
                                    Name = "Very Bad Trip",
                                    ListText = new ObservableCollection<string>
                                    {
                                        "Une trilogie"
                                    },
                                    ListTags = new ObservableCollection<Tag>
                                    {
                                        new Tag{
                                            Name = "Comédie",
                                            Applicated = true
                                        },
                                        ListTagsForThem[3]
                                    }
                                }
                    },
                    ListTags = new ObservableCollection<Tag>{
                        new Tag
                        {
                            Name = "Film",
                            Applicated = true
                        }
                    }
                },
                new PThem{
                    Name = "Mangas",
                    ListContent = new ObservableCollection<PContent>{
                         new PContent{
                             Name = "Claymore",
                            ListText = new ObservableCollection<string>
                            {
                                "Personnages : Clare et Theresa"
                            },
                            ListTags = new ObservableCollection<Tag>
                            {
                               new Tag
                               {
                                   Name = "Seinen",
                                   Applicated = true
                               },
                               ListTagsForThem[1]
                            }
                        },
                        new PContent{
                            Name = "Bleach",
                            ListText = new ObservableCollection<string>
                            {
                                "Plus de 300 chapitres"
                            },
                            ListTags = new ObservableCollection<Tag>
                            {
                                new Tag
                                {
                                    Name = "Shojo",
                                    Applicated = true
                                },
                                ListTagsForThem[1]
                            }
                        }
                    },
                    ListTags = new ObservableCollection<Tag>{
                        new Tag
                        {
                            Name = "Mangas",
                            Applicated = true
                        }
                    }
                },
                new PThem{
                    Name = "Jeux Vidéo",
                    ListContent = new ObservableCollection<PContent>{
                        new PContent{
                                        Name = "League Of Legends",
                                        ListText = new ObservableCollection<string>
                                        {
                                            "Championnat du monde de la saison 3"
                                        },
                                        ListTags = new ObservableCollection<Tag>
                                        {
                                            new Tag
                                            {
                                                Name = "MOBA",
                                                Applicated = true
                                            },
                                            ListTagsForThem[0]
                                        }
                                    },
                        new PContent{
                                        Name = "Starcarft 2",
                                        ListText = new ObservableCollection<string>
                                        {
                                            "Un jeu qui a lancé l'Esport"
                                        },
                                        ListTags = new ObservableCollection<Tag>
                                        {
                                            new Tag
                                            {
                                                Name = "Blizzard",
                                                Applicated = true
                                            },
                                            ListTagsForThem[0]
                                        }
                        }
                    },
                    ListTags = new ObservableCollection<Tag>{
                        new Tag
                        {
                            Name = "Jeux Vidéo",
                            Applicated = true
                        }
                    }
                }
            };
            IsolatedStorageSettings.ApplicationSettings["DataBase"] = this.ListThems;
            DataContext = this;            
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PhoneApplicationService.Current.State.ContainsKey("CurrentPage") == true)
                this.ThemsToDisplay.DefaultItem = PhoneApplicationService.Current.State["CurrentPage"];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool NotifyPropertyChanged<T>(ref T variable, T valeur, string nomPropriete)
        {
            if (object.Equals(variable, valeur)) return false;

            variable = valeur;
            NotifyPropertyChanged(nomPropriete);
            return true;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox Listbox = sender as ListBox;
            if (Listbox.SelectedItem == null)
                return;
            PContent P = Listbox.SelectedItem as PContent;
            PhoneApplicationService.Current.State["CurrentPage"] = P;
            Listbox.SelectedItem = null;
            this.NavigationService.Navigate(new Uri("/Article.xaml", UriKind.Relative));
        }

        private void FilterTags_Click(object sender, RoutedEventArgs e)
        {
            PhoneApplicationService.Current.State["CurrentPage"] = ThemsToDisplay.SelectedItem;
            this.NavigationService.Navigate(new Uri("/FilterTagsForThems.xaml", UriKind.Relative));
        }
    }
}