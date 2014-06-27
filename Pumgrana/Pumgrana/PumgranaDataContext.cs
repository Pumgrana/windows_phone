using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pumgrana
{
    public abstract class PumgranaDataContext : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class DT_EditContent : PumgranaDataContext
    {
        public ObservableCollection<TagToApply> list_Tag {get;set;}
        public ObservableCollection<Link>       list_Link { get; set; }
        public Param_Article Param { get; set; }
        public string Param_Str { get; set; }
        public enum AddOrDeleteTag
        {
            ADD = 0,
            DELETE
        };
        public AddOrDeleteTag TagAction { get; set; }
        public DT_EditContent()
        {
            Param_Str = "";
            list_Link = new ObservableCollection<Link>();
            list_Tag = new ObservableCollection<TagToApply>();
        }
    }

    public class DT_Article : PumgranaDataContext
    {
        public Content content { get; set; }
        public ObservableCollection<Tag> listTag { get; set; }
        public ListLink listLink { get; set; }

        public DT_Article()
        {
            this.content = new Content();
            this.listTag = new ObservableCollection<Tag>();
            this.listLink = new ListLink();
        }
    }

    public class DT_ShowContent : PumgranaDataContext
    {
        private string todisplay { get; set; }
        public string ToDisplay
        {
            get { return todisplay; }
            set
            {
                todisplay = value;
                SetProperty("ToDisplay");
            }
        }
        public ObservableCollection<Content> listContent { get; set; }

        public DT_ShowContent()
        {
            this.listContent = new ObservableCollection<Content>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private int nbofcontents_ { get; set; }
        public int NbOfContents
        {
            get
            {
                return nbofcontents_;
            }
            set
            {
                nbofcontents_ = value;
                SetProperty("NbOfContents");
            }
        }
    }
}
