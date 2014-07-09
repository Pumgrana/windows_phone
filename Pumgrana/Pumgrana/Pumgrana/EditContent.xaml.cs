﻿using System;
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
using System.Windows.Controls.Primitives;
using System.Text;
using System.Windows.Data;

namespace Pumgrana
{
    public partial class EditContent : PhoneApplicationPage
    {
        DT_EditContent DT = new DT_EditContent();
        PumgranaWebClient HttpClient = new PumgranaWebClient();

        public EditContent()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string tmp = "";
            NavigationContext.QueryString.TryGetValue("Param", out tmp);
            if (tmp == "Edit")
            {
                DT.Param = Pumgrana.Param_Article.EDIT;
                DT.Param_Str = "Edit content";
            }
            else
            {
                DT.Param = Pumgrana.Param_Article.ADD;
                DT.Param_Str = "Add article";
            }
            base.OnNavigatedTo(e);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.DT;
        }

        private void Pivot_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Popup p = this.FindName("PopUpAddTag") as Popup;
            p.IsOpen = false;

            Pivot P = sender as Pivot;
            PivotItem I = P.SelectedItem as PivotItem;
            string Which = I.Header as string;
            if (Which == "Content")
            {
                LoadContentPivotItem(I);
            }
            else if (Which == "Tags")
            {
                LoadTagsPivotItem(I);
            }
            else if (Which == "Linked Articles")
            {
                LoadLinkPivotItem(I);
            }
            else
            {
                LoadTitleItem(I);
            }
        }

        private void LoadTitleItem(PivotItem I)
        {
            if (DT.Param == Pumgrana.Param_Article.EDIT)
            {
                Content c = PhoneApplicationService.Current.State["Article_Content"] as Content;
                (I.FindName("EditTitle") as TextBox).Text = c.title;
                if (c.summary != null)
                    (I.FindName("EditSummary") as TextBox).Text = c.summary;
            }
        }

        private void LoadContentPivotItem(PivotItem I)
        {
            if (DT.Param == Pumgrana.Param_Article.EDIT)
            {
                Content c = PhoneApplicationService.Current.State["Article_Content"] as Content;
                I.DataContext = c;

                Binding B = new Binding();
                B.Path = new PropertyPath("text");
                B.Mode = BindingMode.TwoWay;
                B.Source = c;
                EditContentText.SetBinding(TextBox.TextProperty, B);
            }
        }

        private void    LoadTagsPivotItem(PivotItem P)
        {
            DT.list_Tag.Clear();
            HttpClient.GetTagsByType += HttpClient_GetTagsByType;
            HttpClient.getTagsByType();
        }

        void HttpClient_GetTagsByType(object sender, object output)
        {
            DT.list_Tag.Clear();
            ListTag lt = output as ListTag;
            if (lt.tags.Count > 0)
            {
                ListTag ContentTags = null;
                if (DT.Param ==  Param_Article.EDIT)
                    ContentTags = PhoneApplicationService.Current.State["Article_Tag"] as ListTag;
                foreach (Tag t in lt.tags)
                {
                    TagToApply ToAdd = new TagToApply();
                    ToAdd.title = t.subject;
                    ToAdd.Id = t.uri;
                    if (ContentTags.tags != null)
                        ToAdd.IsChecked= ContentTags.tags.Exists(tmp => tmp.subject == ToAdd.title);
                    else
                        ToAdd.IsChecked = false;
                    DT.list_Tag.Add(ToAdd);
                }
            }
        }

        private void LoadLinkPivotItem(PivotItem I)
        {
            DT.list_Link.Clear();
            if (DT.Param == Param_Article.EDIT)
            {
                HttpClient.GetLinksFromContent += HttpClient_GetLinksFromContent;
                HttpClient.getLinksFromContent((PhoneApplicationService.Current.State["Article_Content"] as Content).uri);
            }
        }

        void HttpClient_GetLinksFromContent(object sender, object output)
        {
            DT.list_Link.Clear();
            ListLink ll = output as ListLink;
            if (ll.links != null)
            {
                foreach (Link l in ll.links)
                {
                    DT.list_Link.Add(l);
                }
            }
        }

        private void AddTagPopUp(object sender, RoutedEventArgs e)
        {
            (this.FindName("ApplyTagButton") as Button).Content = "Add";
            Popup p = this.FindName("PopUpAddTag") as Popup;
            p.IsOpen = true;
            DT.TagAction = DT_EditContent.AddOrDeleteTag.ADD;
        }

        private void ApplyAddTag(object sender, RoutedEventArgs e)
        {
            string data = (this.FindName("TextNewTag") as AutoCompleteBox).Text;
            bool    is_Open = true;

            if (DT.TagAction == DT_EditContent.AddOrDeleteTag.ADD)
            {
                TagToApply  tag = new TagToApply();
                tag.title = data;
                tag.IsChecked = true;
                HttpClient.InsertTags += HttpClient_InsertTags;
                HttpClient.PostTag(data);
                DT.list_Tag.Add(tag);
                is_Open = false;
            }
            else
            {
                if (DT.list_Tag.Count((tmp) => tmp.title == data) > 0)
                {
                    is_Open = false;
                    TagToApply Tag = DT.list_Tag.First((tmp) => tmp.title == data);
                    HttpClient.DeleteTags += HttpClient_DeleteTags;
                    HttpClient.DeleteTag(Tag.Id);
                    DT.list_Tag.Remove(Tag);
                }
                else
                {
                    MessageBox.Show("The tag \"" + data + "\" doesn't exist.");
                }
            }

            Popup p = this.FindName("PopUpAddTag") as Popup;
            p.IsOpen = is_Open;

            (this.FindName("TextNewTag") as AutoCompleteBox).Text = "";
        }

        void HttpClient_InsertTags(object sender, object output)
        {
            HttpClient.InsertTags -= HttpClient_InsertTags;
            WriteTagAnswer data = output as WriteTagAnswer;
            if (data.status == 201)
            {
                DT.list_Tag.Last().Id = data.tags_id.First()._id;
                MessageBox.Show("Tag " + DT.list_Tag.Last().title + " has been added.");
            }
            else
            {
                DT.list_Tag.Remove(DT.list_Tag.Last());
                MessageBox.Show("Error : " + data.error);
            }
        }

        void HttpClient_DeleteTags(object sender, object output)
        {
            HttpClient.DeleteTags -= HttpClient_DeleteTags;
            RequestObject info = output as RequestObject;
            if (info.status == 200)
            {
                MessageBox.Show("Tag deleted.");
            }
            else
            {
                MessageBox.Show("Error : " + info.error);
            }
        }

        private void ApplyDeleteTag(object sender, System.Windows.RoutedEventArgs e)
        {
            DT.TagAction = DT_EditContent.AddOrDeleteTag.DELETE;
            (this.FindName("ApplyTagButton") as Button).Content = "Delete";
            Popup p = this.FindName("PopUpAddTag") as Popup;
            p.IsOpen = true;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> tags = new List<string>();
            Content c = new Content();
            c.summary = (this.FindName("EditSummary") as TextBox).Text;
            c.title = (this.FindName("EditTitle") as TextBox).Text;
            c.body = (this.FindName("EditContentText") as TextBox).Text;

            foreach (TagToApply t in DT.list_Tag)
            {
                if (t.IsChecked == true)
                    tags.Add(t.Id);
            }

            if (DT.Param == Param_Article.EDIT)
            {
                c.uri = (PhoneApplicationService.Current.State["Article_Content"] as Content).uri;
                HttpClient.UpdatedContent += HttpClient_UpdatedContent;
                HttpClient.UpdateContent(c, tags);
            }
            else
            {
                HttpClient.InsertContent += HttpClient_InsertContent;
                HttpClient.CreateContent(c, tags);
            }
        }

        void HttpClient_UpdatedContent(object sender, object output)
        {
            RequestObject data = output as RequestObject;
            if (data.status == 200)
            {
                MessageBox.Show("Content updated");
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Error while updating content : " + data.error);
            }
            HttpClient.UpdatedContent -= HttpClient_UpdatedContent;
        }

        void HttpClient_InsertContent(object sender, object output)
        {
            CreateContent cc = output as CreateContent;
            if (cc.status == 201)
            {
                MessageBox.Show("Content created successfully !");
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Failed to create the content. Error : " + cc.error);
            }
            HttpClient.InsertContent -= HttpClient_InsertContent;
        }
    }
}