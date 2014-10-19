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
using System.ComponentModel;

namespace Pumgrana
{
    public enum Param_Article
    {
        EDIT = 0,
        ADD
    }

    public class RequestObject
    {
        public int status { get; set; }
        public string error { get; set; }

        public RequestObject()
        {

        }
    }

    public class DeleteContent : INotifyPropertyChanged
    {
        private bool _ischecked { get; set; }
        public  bool IsChecked
        {
            get { return _ischecked; }
            set
            {
                _ischecked = value;
                setProperty("IsChecked");
            }
        }

        public string title { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void setProperty(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public DeleteContent()
        {

        }

        public DeleteContent(string title)
        {
            this.title = title;
            this.IsChecked = false;
        }
    }

    public class Content : INotifyPropertyChanged
    {
        public string   uri { get; set; }
        private string title_ { get; set; }
        public string title
        {
            get
            {
                return title_;
            }
            set
            {
                title_ = value;
                setProperty("title");
            }
        }
        public string   summary { get; set; }
        public string   body { get; set; }

        public Content()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void setProperty(string name)
        {
            if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class ListContent : RequestObject
    {
        public List<Content> contents { get; set; }
        public ListContent()
        {
            contents = new List<Content>();
        }
    }

    public class TagToApply : INotifyPropertyChanged
    {
        private bool _checked;
        public bool IsChecked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                OnPropertyChaned("IsChecked");
            }
        }

        public string title { get; set; }
        public string Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChaned(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public TagToApply()
        {
            
        }

        /*public TagToApply(string _title)
        {
            title = _title;
        }*/
    }

    public class Tag
    {
        public string   uri { get; set; }
        public string   subject { get; set; }

        public Tag()
        {

        }
    }

    public class ListTag : RequestObject
    {
        public List<Tag> tags {get;set;}

        public ListTag()
        {
            tags = new List<Tag>();
        }

    }

    public class Link
    {
        public string link_uri { get; set; }
        public string content_uri { get; set; }
        public string content_title { get; set; }
        public string content_summary { get; set; }

        public Link()
        {

        }
    }

    public class FullLink
    {
        public string link_uri { get; set; }
        public string origin_uri { get; set; }
        public string target_uri { get; set; }
        public ListTag tags { get; set; }

        public FullLink()
        {

        }
    }

    public class FullLinkList : RequestObject
    {
        public List<FullLink> links { get; set; }
    }

    public class ListLink : RequestObject
    {
        public List<Link> links { get; set; }

        public ListLink()
        {
            links = new List<Link>();
        }
    }

    public class ReturnWriteTag
    {
        public int status { get; set; }
        public List<string> tags_uri { get; set; }

        ReturnWriteTag()
        {
            tags_uri = new List<string>();
        }
    }

    public class WriteTag
    {
        public string type_name { get; set; }
        public List<string> tags_subject { get; set; }
        public WriteTag()
        {
            tags_subject = new List<string>();
        }
    }

    public class Tags_Id_Tag
    {
        public string _id { get; set; }
    }

    public class WriteTagAnswer : RequestObject
    {
        public List<Tags_Id_Tag> tags_uri { get; set; }
        public WriteTagAnswer()
        {
            tags_uri = new List<Tags_Id_Tag>();
        }
    }

    public class WriteDeleteTag
    {
        public List<string> tags_uri { get; set; }
        public WriteDeleteTag()
        {
            tags_uri = new List<string>();
        }
    }
    public class DeleteTagAnswer : RequestObject
    {
        public DeleteTagAnswer()
        {

        }
    }
    public class WriteUpdateContent : Content
    {
        public List<string> tags_uri { get; set; }
        public string content_uri { get; set; }
        public WriteUpdateContent()
        {
            tags_uri = new List<string>();
        }
        public WriteUpdateContent(Content c)
        {
            this.content_uri = c.uri;
            this.summary = c.summary;
            this.body = c.body;
            this.title = c.title;
        }
    }
    public class CreateContent : RequestObject
    {
        public List<string> content_uri { get; set; }

        public CreateContent()
        {
            content_uri = new List<string>();
        }
    }
    public class WriteCreateContent
    {
        public string title { get; set; }
        public string summary { get; set; }
        public string body { get; set; }
        public List<string> tags_uri { get; set; }
        public WriteCreateContent()
        {
            tags_uri = new List<string>();
        }
        public WriteCreateContent(Content c, List<string> tags)
        {
            this.title = c.title;
            this.summary = c.summary;
            this.body = c.body;
            this.tags_uri = tags;
        }
    }
    public class PutDeleteContent
    {
        public List<string> contents_uri { get; set; }
        public PutDeleteContent()
        {
            contents_uri = new List<string>();
        }
    }
}