using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using System.Text;
using System.Runtime.Serialization;


namespace Pumgrana
{
    public class PumgranaWebClient
    {
        string baseurl;
        enum TypeOfRequest
        {
            GET_DETAIL = 0,
            GET_DETAIL_FROM_LINK,
            GET_CONTENTS,
            INSERT_CONTENT,
            UPDATE_CONTENT,
            DELETE_CONTENT,
            GET_TAGS_BY_TYPE,
            GET_TAGS_FROM_CONTENT,
            GET_TAGS_FROM_CONTENT_LINKS,
            INSERT_TAGS,
            DELETE_TAGS,
            GET_LINKS_FROM_CONTENT,
            GET_LINKS_FROM_CONTENT_TAGS,
            INSERT_LINK,
            UPDATE_LINK,
            DELETE_LINK
        }

        private class RequestInfo
        {
            public Type type { get; set; }
            public TypeOfRequest Which { get; set; }

            public RequestInfo(Type otherType, TypeOfRequest otherRequest)
            {
                type = otherType;
                Which = otherRequest;
            }
        }

        public  delegate void EventFinish(object sender, object output);

        public event EventFinish GetDetail;
        public event EventFinish GetDetailFromLink;

        public event EventFinish GetContents;
        public event EventFinish InsertContent;
        public event EventFinish UpdatedContent;
        public event EventFinish DeleteContents;

        public event EventFinish GetTagsByType;
        public event EventFinish GetTagsFromContent;
        public event EventFinish GetTagsFromContentLinks;
        public event EventFinish InsertTags;
        public event EventFinish DeleteTags;

        public event EventFinish GetLinksFromContent;
        public event EventFinish GetLinksFromContentTags;
        public event EventFinish InsertLinks;
        public event EventFinish UpdateLinks;
        public event EventFinish DeleteLinks;

        public PumgranaWebClient()
        {
            baseurl = "http://163.5.84.222/api/";
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string res = e.Result;
                object data = new object();
                RequestInfo info = e.UserState as RequestInfo;
                data = Newtonsoft.Json.JsonConvert.DeserializeObject(res, info.type);
                switch (info.Which)
                {
                    case (TypeOfRequest.INSERT_TAGS):
                        {
                            InsertTags(sender, data);
                            break;
                        }
                    case (TypeOfRequest.DELETE_TAGS):
                        {
                            DeleteTags(sender, data);
                            break;
                        }
                    case (TypeOfRequest.UPDATE_CONTENT):
                        {
                            UpdatedContent(sender, data);
                            break;
                        }
                    case (TypeOfRequest.INSERT_CONTENT):
                        {
                            InsertContent(sender, data);
                            break;
                        }
                    case (TypeOfRequest.DELETE_CONTENT):
                        {
                            DeleteContents(sender, data);
                            break;
                        }
                }
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Stream s = e.Result;

                Type type = (e.UserState as RequestInfo).type;

                TypeOfRequest WhichRequest = (e.UserState as RequestInfo).Which;

                DataContractJsonSerializer ser = new DataContractJsonSerializer(type);
                
                object outData = ser.ReadObject(s);
                switch (WhichRequest)
                {
                    case TypeOfRequest.GET_DETAIL:
                        {
                            GetDetail(this, outData);
                            break;
                        }
                    case TypeOfRequest.GET_CONTENTS:
                        {
                            GetContents(this, outData);
                            break;
                        }
                    case TypeOfRequest.GET_TAGS_FROM_CONTENT:
                        {
                            GetTagsFromContent(this, outData);
                            break;
                        }
                    case TypeOfRequest.GET_LINKS_FROM_CONTENT:
                        {
                            GetLinksFromContent(this, outData);
                            break;
                        }
                    case TypeOfRequest.GET_TAGS_BY_TYPE:
                        {
                            GetTagsByType(this, outData);
                            break;
                        }
                }
                s.Close();
            }
        }

        public void getContents()
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            RequestInfo info = new RequestInfo(typeof(ListContent), TypeOfRequest.GET_CONTENTS);
            string url = baseurl + "content/list_content/";
            wc.OpenReadAsync(new Uri(url), info);
        }
        public void getTagsFromContent(string id)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            RequestInfo info = new RequestInfo(typeof(ListTag), TypeOfRequest.GET_TAGS_FROM_CONTENT);
            string url = baseurl + "tag/list_from_content/" + id;
            wc.OpenReadAsync(new Uri(url), info);
        }
        public void getLinksFromContent(string id)
        {
           WebClient wc = new WebClient();
           wc.OpenReadCompleted += wc_OpenReadCompleted;
           RequestInfo info = new RequestInfo(typeof(ListLink), TypeOfRequest.GET_LINKS_FROM_CONTENT);
           string url = baseurl + "link/list_from_content/" + id;
           wc.OpenReadAsync(new Uri(url), info);
        }
        public void getDetail(string id)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            RequestInfo info = new RequestInfo(typeof(ListContent), TypeOfRequest.GET_DETAIL);
            string url = baseurl + "content/detail/" + id;
            wc.OpenReadAsync(new Uri(url), info);
        }
        public void getTagsByType()
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += wc_OpenReadCompleted;
            RequestInfo info = new RequestInfo(typeof(ListTag), TypeOfRequest.GET_TAGS_BY_TYPE);
            string url = baseurl + "tag/list_by_type/CONTENT";
            wc.OpenReadAsync(new Uri(url), info);
        }
        public void PostTag(List<string> tags, string id = "")
        {
                WebClient wc = new WebClient();
                wc.UploadStringCompleted += wc_UploadStringCompleted;
                string url = baseurl + "tag/insert";

                wc.Headers["Content-Type"] = "application/json";

                WriteTag tag = new WriteTag();
                tag.tags_subject = tags;
                tag.type_name = "CONTENT";

                RequestInfo info = new RequestInfo(typeof(WriteTagAnswer), TypeOfRequest.INSERT_TAGS);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(tag);
                wc.UploadStringAsync(new Uri(url), "POST", json, info);
        }
        public void PostTag(string tag)
        {
            List<string> list = new List<string>();
            list.Add(tag);
            PostTag(list);
        }
        public void DeleteTag(List<string> subjects)
        {
            WebClient wc = new WebClient();
            wc.UploadStringCompleted += wc_UploadStringCompleted;
            string url = baseurl + "tag/delete";

            wc.Headers["Content-Type"] = "application/json";

            WriteDeleteTag tag = new WriteDeleteTag();
            tag.tags_id = subjects;

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(tag);
            RequestInfo info = new RequestInfo(typeof(RequestObject), TypeOfRequest.DELETE_TAGS);
            wc.UploadStringAsync(new Uri(url), "POST", json, info);
        }
        public void DeleteTag(string subject)
        {
            List<string> list = new List<string>();
            list.Add(subject);
            DeleteTag(list);
        }

        public void     UpdateContent(Content c, List<string> Tags)
        {
            WebClient wc = new WebClient();
            wc.UploadStringCompleted += wc_UploadStringCompleted;
            WriteUpdateContent uc = new WriteUpdateContent(c);
            uc.tags_id = Tags;
            string url = baseurl + "content/update";

            wc.Headers["Content-Type"] = "application/json";
            RequestInfo info = new RequestInfo(typeof(RequestObject), TypeOfRequest.UPDATE_CONTENT);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(uc);
            wc.UploadStringAsync(new Uri(url), "POST", json, info);
        }
        public void CreateContent(Content c, List<string> tags)
        {
            WebClient wc = new WebClient();
            wc.UploadStringCompleted += wc_UploadStringCompleted;
            WriteCreateContent cc = new WriteCreateContent(c, tags);
            string url = baseurl + "content/insert";

            wc.Headers["Content-Type"] = "application/json";
            RequestInfo info = new RequestInfo(typeof(CreateContent), TypeOfRequest.INSERT_CONTENT);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(cc);
            wc.UploadStringAsync(new Uri(url), "POST", json, info);
        }
        public void DeleteContent(string id)
        {
            WebClient wc = new WebClient();
            wc.UploadStringCompleted += wc_UploadStringCompleted;
            PutDeleteContent c = new PutDeleteContent();
            c.contents_id.Add(id);
            string url = baseurl + "content/delete";

            wc.Headers["Content-Type"] = "application/json";
            RequestInfo info = new RequestInfo(typeof(RequestObject), TypeOfRequest.DELETE_CONTENT);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(c);
            wc.UploadStringAsync(new Uri(url), "POST", json, info);
        }
    }
}