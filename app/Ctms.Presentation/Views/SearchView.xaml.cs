﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für Search.xaml
    /// </summary>
    //public partial class SearchView : UserControl
    //{
    //    public SearchView()
    //    {
    //        InitializeComponent();
    //    }
    //}

    [Export(typeof(ISearchView))]
    public partial class SearchView : UserControl, ISearchView
    {
        public SearchView()
        {
            InitializeComponent();
        }
    }
}
