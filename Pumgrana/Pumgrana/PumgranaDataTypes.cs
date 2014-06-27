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
    }


    public class Content
    {
        public string   _id { get; set; }
        public string   title { get; set; }
        public string   summary { get; set; }
        public string   text { get; set; }
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
        public bool Checked
        {
            get
            {
                return _checked;
            }
            set
            {
                _checked = value;
                OnPropertyChaned("Checked");
            }
        }

        public string Subject { get; set; }
        public string Id { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChaned(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    public class Tag
    {
        public string   _id { get; set; }
        public string   subject { get; set; }
    }

    public class ListTag : RequestObject
    {
        public List<Tag> tags {get;set;}
    }

    public class Link
    {
        public string link_id { get; set; }
        public string content_id { get; set; }
        public string content_title { get; set; }
        public string content_summary { get; set; }
    }

    public class ListLink : RequestObject
    {
        public List<Link> links { get; set; }
    }

    public class ReturnWriteTag
    {
        public int status { get; set; }
        public List<string> tags_id { get; set; }

        ReturnWriteTag()
        {
            tags_id = new List<string>();
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
        public List<Tags_Id_Tag> tags_id { get; set; }
        public WriteTagAnswer()
        {
            tags_id = new List<Tags_Id_Tag>();
        }
    }

    public class WriteDeleteTag
    {
        public List<string> tags_id { get; set; }
        public WriteDeleteTag()
        {
            tags_id = new List<string>();
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
        public List<string> tags_id { get; set; }
        public string content_id { get; set; }
        public WriteUpdateContent()
        {
            tags_id = new List<string>();
        }
        public WriteUpdateContent(Content c)
        {
            this.content_id = c._id;
            this.summary = c.summary;
            this.text = c.text;
            this.title = c.title;
        }
    }
    public class CreateContent : RequestObject
    {
        public List<string> content_id { get; set; }

        public CreateContent()
        {
            content_id = new List<string>();
        }
    }
    public class WriteCreateContent
    {
        public string title { get; set; }
        public string summary { get; set; }
        public string text { get; set; }
        public List<string> tags_id { get; set; }
        public WriteCreateContent()
        {
            tags_id = new List<string>();
        }
        public WriteCreateContent(Content c, List<string> tags)
        {
            this.title = c.title;
            this.summary = c.summary;
            this.text = c.text;
            this.tags_id = tags;
        }
    }
    public class PutDeleteContent
    {
        public List<string> contents_id { get; set; }
        public PutDeleteContent()
        {
            contents_id = new List<string>();
        }
    }
}