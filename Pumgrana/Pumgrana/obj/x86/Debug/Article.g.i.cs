﻿#pragma checksum "C:\Users\Tristan\documents\visual studio 2013\Projects\Pumgrana\Pumgrana\Article.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4953BFBD809BE84AC1E46EECD9A8F5F7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Pumgrana {
    
    
    public partial class Article : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.ProgressBar ProgressLoadContent;
        
        internal Microsoft.Phone.Controls.Pivot ArticlePivot;
        
        internal Microsoft.Phone.Controls.PivotItem ContentPivotItem;
        
        internal Microsoft.Phone.Controls.PivotItem TagsPivotItem;
        
        internal Microsoft.Phone.Controls.PivotItem LinkedPivotItem;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Pumgrana;component/Article.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ProgressLoadContent = ((System.Windows.Controls.ProgressBar)(this.FindName("ProgressLoadContent")));
            this.ArticlePivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("ArticlePivot")));
            this.ContentPivotItem = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("ContentPivotItem")));
            this.TagsPivotItem = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("TagsPivotItem")));
            this.LinkedPivotItem = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("LinkedPivotItem")));
        }
    }
}
