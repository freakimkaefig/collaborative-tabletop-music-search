using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Ctms.Applications.DataModels;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Ctms.Domain.Objects;
using System.Windows.Media.Imaging;
using Microsoft.Surface.Presentation.Controls;
using Ctms.Applications.Workers;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;

namespace Ctms.Presentation.Resources
{
    public partial class ResultStyleResources : ResourceDictionary, IResultStyleResources
    {
        public Action<String> _prelistenAction;
        public Action<String> _addToPlaylistAction;

        public ResultStyleResources()
        {
            InitializeComponent();
        }

        public Action<String> PrelistenAction
        {
            get { return _prelistenAction; }
            set
            {
                _prelistenAction = value;
            }
        }
        public Action<String> AddToPlaylistAction
        {
            get { return _addToPlaylistAction; }
            set
            {
                _addToPlaylistAction = value;
            }
        }

        private void TabItem_TouchDown(object sender, TouchEventArgs e)
        {
            TabItem tab = sender as TabItem;
            TabControl control = tab.Parent as TabControl;
            control.SelectedItem = tab;
            e.Handled = true;
        }

        private void SongList_Prelisten_Click(object sender, RoutedEventArgs e)
        {
            SurfaceButton sButton = e.Source as SurfaceButton;
            ArtistSong song = sButton.DataContext as ArtistSong;
            //_prelistenAction.Invoke(song.TrackId);
            //_prelistenAction(song.TrackId);
        }

        private void SongList_AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
