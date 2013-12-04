using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace PumgranaDesign
{
    public class PumgranaContent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != title)
                {
                    title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }
        private ObservableCollection<string> listTags;
        public ObservableCollection<String> ListTags
        {
            get
            {
                return listTags;
            }
            set
            {
                if (value != listTags)
                {
                    listTags = value;
                    NotifyPropertyChanged("ListTags");
                }
            }
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
