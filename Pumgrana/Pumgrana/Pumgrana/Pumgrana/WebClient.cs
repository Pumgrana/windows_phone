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
using System.Threading;


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
            GET_LINK_DETAIL_FROM_URI,
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

        public event EventFinish Error;

        public event EventFinish GetDetail;
        public event EventFinish GetDetailFromLink;
        public event EventFinish GetLinkDetailFromUri;

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

        private WebClient[] PumgranaWebClientPoolDownload{ get; set; }
        private WebClient[] PumgranaWebClientPoolUpload { get; set; }
        private enum SIZE_POOL
        {
            SIZE_POOL_DOWNLOAD = 3,
            SIZE_POOL_UPLOAD = 1
        }

        public PumgranaWebClient()
        {

            baseurl = "http://163.5.84.222/api/";
            PumgranaWebClientPoolDownload = new WebClient[(int)SIZE_POOL.SIZE_POOL_DOWNLOAD];
            for (int i = 0 ; i < (int)SIZE_POOL.SIZE_POOL_DOWNLOAD ; i++)
            {
                PumgranaWebClientPoolDownload[i] = new WebClient();
                PumgranaWebClientPoolDownload[i].DownloadStringCompleted += wc_OpenReadCompleted;
            }

            PumgranaWebClientPoolUpload = new WebClient[(int)SIZE_POOL.SIZE_POOL_UPLOAD];
            for (int i = 0 ; i < (int)SIZE_POOL.SIZE_POOL_UPLOAD ; i++)
            {
                PumgranaWebClientPoolUpload[i] = new WebClient();
                PumgranaWebClientPoolUpload[i].Headers["Content-Type"] = "application/json";
                PumgranaWebClientPoolUpload[i].UploadStringCompleted += wc_UploadStringCompleted;
            }
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
                Error(this, e.Error);
            }
        }

        void wc_OpenReadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                string s = e.Result;

                RequestInfo info = e.UserState as RequestInfo;

                Type type_ = info.type;
                TypeOfRequest WhichRequest = info.Which;

                object outData = Newtonsoft.Json.JsonConvert.DeserializeObject(s, type_);

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
                    case (TypeOfRequest.GET_LINK_DETAIL_FROM_URI):
                        {
                            GetLinkDetailFromUri(sender, outData);
                            break;
                        }
                }
            }
            else
            {
                Error(this, e.Error);
            }
        }

        private WebClient GetFreeWebClientDownLoad()
        {
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (PumgranaWebClientPoolDownload[i].IsBusy == false)
                        return PumgranaWebClientPoolDownload[i];
                }
            }
        }

        private WebClient GetFreeWebClientUpload()
        {
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (PumgranaWebClientPoolUpload[i].IsBusy == false)
                        return PumgranaWebClientPoolUpload[i];
                }
            }
        }

        private string EncodeUrl(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        public void getContents(List<string> tags_uri = null)
        {
            WebClient wc = GetFreeWebClientDownLoad();
            RequestInfo info = new RequestInfo(typeof(ListContent), TypeOfRequest.GET_CONTENTS);
            string url = baseurl + "content/list_content/";
            if (tags_uri != null && tags_uri.Count > 0)
            {
                url += "/";
                foreach (string s in tags_uri)
                {
                    url += EncodeUrl(s) + "/";
                }
            }
            wc.DownloadStringAsync(new Uri(url), info);
        }

        public void getTagsFromContent(string id)
        {
            WebClient wc = GetFreeWebClientDownLoad();
            RequestInfo info = new RequestInfo(typeof(ListTag), TypeOfRequest.GET_TAGS_FROM_CONTENT);
            string url = baseurl + "tag/list_from_content/" + EncodeUrl(id);
            wc.DownloadStringAsync(new Uri(url), info);
        }
        public void getLinksFromContent(string id)
        {
           WebClient wc = GetFreeWebClientDownLoad();
           RequestInfo info = new RequestInfo(typeof(ListLink), TypeOfRequest.GET_LINKS_FROM_CONTENT);
           string url = baseurl + "link/list_from_content/" + EncodeUrl(id);
           wc.DownloadStringAsync(new Uri(url), info);
        }

        public void getLinkDetailFromUri(string uri)
        {
            WebClient wc = GetFreeWebClientDownLoad();
            RequestInfo info = new RequestInfo(typeof(FullLinkList), TypeOfRequest.GET_LINK_DETAIL_FROM_URI);
            string url = baseurl + "link/detail/" + EncodeUrl(uri);
            wc.DownloadStringAsync(new Uri(url), info);
        }

        public void getDetail(string id)
        {
            WebClient wc = GetFreeWebClientDownLoad();
            RequestInfo info = new RequestInfo(typeof(ListContent), TypeOfRequest.GET_DETAIL);
            string url = baseurl + "content/detail/" + EncodeUrl(id);
            wc.DownloadStringAsync(new Uri(url), info);
        }
        public void getTagsByType()
        {
            WebClient wc = GetFreeWebClientDownLoad();
            RequestInfo info = new RequestInfo(typeof(ListTag), TypeOfRequest.GET_TAGS_BY_TYPE);
            string url = baseurl + "tag/list_by_type/CONTENT";
            wc.DownloadStringAsync(new Uri(url), info);
        }

        public void AbortConnection()
        {
            foreach (WebClient wc in this.PumgranaWebClientPoolDownload)
            {
                if (wc.IsBusy == true)
                    wc.CancelAsync();
            }

            foreach (WebClient wc in this.PumgranaWebClientPoolUpload)
            {
                if (wc.IsBusy == true)
                    wc.CancelAsync();
            }
        }
    }
}