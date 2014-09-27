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
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Threading.Tasks;

namespace Pumgrana
{
    public partial class ShowContent : PhoneApplicationPage
    {
        DT_ShowContent DT = new DT_ShowContent();
        PumgranaWebClient wc = new PumgranaWebClient();

        private int NbOfContent = 0;

        private List<ApplicationBarIconButton> ButtonsApplyTag { get; set; }

        private ObservableCollection<TagToApply> TagApplied { get; set; }

        public ShowContent()
        {
            InitializeComponent();
            ButtonsApplyTag = new List<ApplicationBarIconButton>();
            TagApplied = new ObservableCollection<TagToApply>();

            ApplicationBarIconButton ApplyTag = new ApplicationBarIconButton();
            ApplyTag.IconUri = new Uri("/Assets/AppBar/check.png", UriKind.Relative);
            ApplyTag.IsEnabled = true;
            ApplyTag.Text = "Apply";

            ApplicationBarIconButton CancelTag = new ApplicationBarIconButton();
            CancelTag.IconUri = new Uri("/Assets/AppBar/back.png", UriKind.Relative);
            CancelTag.IsEnabled = true;
            CancelTag.Text = "Cancel";

            ButtonsApplyTag.Add(CancelTag);
            ButtonsApplyTag.Add(ApplyTag);

            var pullDetector = new WP8PullDetector();
            pullDetector.Bind(this.ListContent);
            pullDetector.Compression += pullDetector_Compression;

            wc.Error += wc_Error;
        }

        void pullDetector_Compression(object sender, CompressionEventArgs e)
        {
            if (e.Type == CompressionType.Top)
                RefreshContents();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DT_ShowContent dt_local = await IsolatedStorageOperations.Load<DT_ShowContent>("Content.xml");
            if (dt_local.listContent.Count > 0)
            {
                this.DT = dt_local;
            }
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            this.ListContent.DataContext = this.DT;
            GetContentFromServer();
            GetAllTags();
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
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Collapsed;
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
            this.NbOfContent += 1;
            this.NavigationService.Navigate(new Uri("/Article.xaml?id=" + id, UriKind.Relative));
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

        private void RefreshContents()
        {
            this.ProgressLoadContent.Visibility = System.Windows.Visibility.Visible;
            GetContentFromServer(getSelectedTags());
        }

        private void CancelChangesButton_Click(object sender, EventArgs e)
        {
            if (ContentPivot.SelectedIndex == 1) // Tag window
            {
                ContentPivot.SelectedIndex = 0;
                this.DT.listTag = TagApplied;
            }
        }

        void ApplyChangesButton_Click(object sender, EventArgs e)
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
                TagApplied = this.DT.listTag;
            }
            else
            {
                //ChangeApplicationBarIconButtons(new List<ApplicationBarIconButton>()); // Clear the icon bar buttons
            }
        }

        private void ClearCache_Event(object sender, System.EventArgs e)
        {
            ClearCache();
        }

        private async Task ClearCache()
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            IReadOnlyList<StorageFile> files = await local.GetFilesAsync();
            foreach (StorageFile file in files)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                System.Diagnostics.Debug.WriteLine("File " + file.Name + " deleted");
            }
            this.NbOfContent = 0;
        }

        private void RefreshContent_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton button = sender as ApplicationBarIconButton;
            button.IsEnabled = false;
            RefreshContents();
            button.IsEnabled = true;
        }
    }
}