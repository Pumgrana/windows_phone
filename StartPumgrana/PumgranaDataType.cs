using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace StartPumgrana
{
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<Tag> List = value as ObservableCollection<Tag>;

            foreach (Tag T in List)
            {
                if (T.Applicated == true)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class PThem : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != null)
                    name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public ObservableCollection<PContent> ListContent
        {
            get;
            set;
        }
        public ObservableCollection<Tag> ListTags
        {
            get;
            set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }

    public class PContent : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != null)
                    name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public ObservableCollection<string> ListText
        {
            get;
            set;
        }
        public ObservableCollection<Tag> ListTags
        {
            get;
            set;
        }
        public ObservableCollection<PLink> ListLinks
        {
            get;
            set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        public override string  ToString()
        {
            return Name;
        }
    }

    public class Tag : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != null)
                {
                    name = value;
                }
                NotifyPropertyChanged("Name");
            }
        }
        private bool applicated;
        public bool Applicated
        {
            get
            {
                return applicated;
            }
            set
            {
                if (value != applicated)
                {
                    applicated = value;
                }
                NotifyPropertyChanged("Applicated");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }
    public class PLink : INotifyPropertyChanged
    {
        public PContent Content
        {
            get;
            set;
        }
        public ObservableCollection<Tag> ListTags
        {
            get;
            set;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }
}
