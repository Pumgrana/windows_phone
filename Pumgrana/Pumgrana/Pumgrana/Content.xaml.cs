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
using System.Runtime.Serialization.Json;
using System.Windows.Data;
using System.Threading;

namespace Pumgrana
{
    public partial class ShowContent : PhoneApplicationPage
    {
        private enum ShowContentMode
        {
            SHOW = 0,
            DELETE
        }
        private ShowContentMode Mode { get; set; }
        DT_ShowContent DT = new DT_ShowContent();
        PumgranaWebClient wc = new PumgranaWebClient();

        private List<ApplicationBarIconButton> ButtonsDisplayContent { get; set; }
        private List<ApplicationBarIconButton> ButtonsDeleteContent { get; set; }
        private List<ApplicationBarIconButton> ButtonsApplyTag { get; set; }

        public ShowContent()
        {
            InitializeComponent();
            ButtonsDeleteContent = new List<ApplicationBarIconButton>();
            ButtonsDisplayContent = new List<ApplicationBarIconButton>();
            ButtonsApplyTag = new List<ApplicationBarIconButton>();

            ApplicationBarIconButton AddArticleButton = new ApplicationBarIconButton();
            AddArticleButton.IconUri = new Uri("/Assets/AppBar/add.png", UriKind.Relative);
            AddArticleButton.Text = "Add Article";
            AddArticleButton.IsEnabled = false;
            ButtonsDisplayContent.Add(AddArticleButton);
            ApplicationBar.Buttons.Add(AddArticleButton);

            ApplicationBarIconButton DeleteArticleButton = new ApplicationBarIconButton();
            DeleteArticleButton.IconUri = new Uri("/Assets/AppBar/delete.png", UriKind.Relative);
            DeleteArticleButton.Text = "Delete Article";
            DeleteArticleButton.IsEnabled = true;
            DeleteArticleButton.Click += SwitchMode;
            ButtonsDisplayContent.Add(DeleteArticleButton);
            ApplicationBar.Buttons.Add(DeleteArticleButton);

            ApplicationBarIconButton RefreshButton = new ApplicationBarIconButton();
            RefreshButton.IconUri = new Uri("/Assets/AppBar/refresh.png", UriKind.Relative);
            RefreshButton.Text = "Refresh";
            RefreshButton.IsEnabled = true;
            RefreshButton.Click += RefreshContents;
            ButtonsDisplayContent.Add(RefreshButton);
            ApplicationBar.Buttons.Add(RefreshButton);

            ApplicationBarIconButton ApplyChangesButton = new ApplicationBarIconButton();
            ApplyChangesButton.IconUri = new Uri("/Assets/AppBar/check.png", UriKind.Relative);
            ApplyChangesButton.Text = "Apply";
            ApplyChangesButton.Click += ApplyChangesButton_Click;
            ButtonsDeleteContent.Add(ApplyChangesButton);

            ApplicationBarIconButton CancelChangesButton = new ApplicationBarIconButton();
            CancelChangesButton.IconUri = new Uri("/Assets/AppBar/cancel.png", UriKind.Relative);
            CancelChangesButton.Text = "Cancel";
            CancelChangesButton.Click += CancelChangesButton_Click;
            ButtonsDeleteContent.Add(CancelChangesButton);

            ButtonsApplyTag = ButtonsDeleteContent;

            Mode = ShowContentMode.SHOW;

            wc.Error += wc_Error;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DT_ShowContent dt_local = IsolatedStorageOperations.Load<DT_ShowContent>("Content.xml");
            if (dt_local.listContent.Count > 0)
            {
                this.DT = dt_local;
            }
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            this.Mode = ShowContentMode.SHOW;
            this.ListContent.DataContext = this.DT;
            this.ListContentToDelete.DataContext = this.DT;
            GetAllTags(); // Call GetContentFromServer as well
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.DT.listContent.Clear();
            this.DT.listTag.Clear();
            this.DT.ContentToDelete.Clear();
            base.OnNavigatedFrom(e);
        }

        void wc_Error(object sender, object output)
        {
            Exception e = output as Exception;
            MessageBox.Show(e.Message);

            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
            (this.ApplicationBar.Buttons[2] as ApplicationBarIconButton).IsEnabled = true;
        }

        private void GetAllTags()
        {
            wc.GetTagsByType += wc_GetTagsByType;
            wc.getTagsByType();
        }

        void wc_GetTagsByType(object sender, object output)
        {
            wc.GetTagsByType -= wc_GetTagsByType;
            this.DT.listTag.Clear();
            ListTag lt = output as ListTag;

            foreach (Tag t in lt.tags)
            {
                TagToApply ToAdd = new TagToApply();
                ToAdd.IsChecked = true;
                ToAdd.Id = t.uri;
                ToAdd.title = t.subject;
                this.DT.listTag.Add(ToAdd);
            }
            GetContentFromServer(getSelectedTags());
            this.ListSelectedTags.DataContext = this.DT;
        }

        private void GetContentFromServer(List<string> tags_uri = null)
        {
            wc.GetContents += wc_GetContents;
            wc.getContents(tags_uri);
        }

        void wc_GetContents(object sender, object output)
        {
            wc.GetContents -= wc_GetContents;
            this.DT.listContent.Clear();
            this.DT.ContentToDelete.Clear();
            ListContent lc = output as ListContent;

            foreach (Content c in lc.contents)
            {
                this.DT.NbOfContents += 1;
                this.DT.listContent.Add(c);
                this.DT.ContentToDelete.Add(new DeleteContent(c.title));
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCurrentPage), this.DT);

            ApplicationBarIconButton b = this.ButtonsDisplayContent.Find(x => x.Text == "Refresh");
            b.IsEnabled = true;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SaveCurrentPage(object data)
        {
            IsolatedStorageOperations.Save<Pumgrana.DT_ShowContent>(data as Pumgrana.DT_ShowContent, "Content.xml");
        }

        private void ListContent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Content c = (sender as LongListSelector).SelectedItem as Content;
            string id = c.uri;
            this.NavigationService.Navigate(new Uri("/Article.xaml?id=" + id, UriKind.Relative));
        }

        void wc_DeleteContents(object sender, object output)
        {
            RequestObject info = output as RequestObject;
            if (info.status == 200)
            {
                this.DT.listContent.Remove(this.ListContent.SelectedItem as Content);
                System.Diagnostics.Debug.WriteLine("Content deleted.");
            }
            else
            {
                //MessageBox.Show("Failed to delete the content. Error : " + info.error);
                System.Diagnostics.Debug.WriteLine("Failed to delete the content. Error : " + info.error);
            }
            wc.DeleteContents -= wc_DeleteContents;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditContent.xaml?Param=Add", UriKind.Relative));
        }

        private void SwitchMode(object sender, System.EventArgs e)
        {
            if (Mode == ShowContentMode.SHOW)
            {
                this.ListContent.Visibility = System.Windows.Visibility.Collapsed;
                this.ListContentToDelete.Visibility = System.Windows.Visibility.Visible;
                Mode = ShowContentMode.DELETE;
                ChangeApplicationBarIconButtons(ButtonsDeleteContent);
            }
            else
            {
                this.ListContent.Visibility = System.Windows.Visibility.Visible;
                this.ListContentToDelete.Visibility = System.Windows.Visibility.Collapsed;
                Mode = ShowContentMode.SHOW;
                ApplicationBar.Buttons.Clear();
                ChangeApplicationBarIconButtons(ButtonsDisplayContent);
            }
        }

        private List<string> getSelectedTags()
        {
            List<string> tags_uri = new List<string>();
            foreach (TagToApply tag in this.DT.listTag.Where(tmp => tmp.IsChecked == true))
            {
                tags_uri.Add(tag.Id);
            }
            return (tags_uri);
        }

        private void RefreshContents(object sender, System.EventArgs e)
        {
            ApplicationBarIconButton button = sender as ApplicationBarIconButton;
            button.IsEnabled = false;
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            GetContentFromServer(getSelectedTags());
        }

        private void CancelChangesButton_Click(object sender, EventArgs e)
        {
            if (ContentPivot.SelectedIndex == 1) // Tag window
            {
                ContentPivot.SelectedIndex = 0;
                foreach (TagToApply tag in this.DT.listTag)
                {
                    tag.IsChecked = true;
                }
            }
            else // Content window
            {
                SwitchMode(sender, e);
            }
        }

        void ApplyChangesButton_Click(object sender, EventArgs e)
        {
            if (ContentPivot.SelectedIndex == 0) // Content window
            {
                foreach (DeleteContent dc in this.DT.ContentToDelete.Where(x => x.IsChecked == true))
                {
                    Content ToDel = this.DT.listContent.Where(x => x.title == dc.title).First();
                    this.wc.DeleteContents += wc_DeleteContents;
                    this.wc.DeleteContent(ToDel.uri);
                }
                SwitchMode(sender, e);
            }
            else
            {
                List<string> tags_uri = new List<string>();
                foreach (TagToApply tag in  this.DT.listTag.Where(tmp => tmp.IsChecked == true))
                {
                    tags_uri.Add(tag.Id);
                }
                ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
                wc.GetContents += wc_GetContents;
                wc.getContents(tags_uri);
                ContentPivot.SelectedIndex = 0;
            }
        }

        private void ChangeApplicationBarIconButtons(List<ApplicationBarIconButton> list)
        {
            ApplicationBar.Buttons.Clear();
            foreach (ApplicationBarIconButton b in list)
            {
                ApplicationBar.Buttons.Add(b);
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;
            if (p.SelectedIndex == 1) //Tag window
            {
                ChangeApplicationBarIconButtons(ButtonsApplyTag);
            }
            else // Content  window
            {
                ChangeApplicationBarIconButtons(ButtonsDisplayContent);
            }
        }
    }
}