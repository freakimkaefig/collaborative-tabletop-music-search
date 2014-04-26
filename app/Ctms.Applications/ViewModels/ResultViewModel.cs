using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using System.Collections.ObjectModel;
using SpotifySharp;
using Ctms.Applications.DataModels;
using Microsoft.Surface.Presentation.Controls;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class ResultViewModel : ViewModel<IResultView>
    {
        private bool isValid = true;
        private Result result;
        private string title;

        private SurfaceButton _dropTargetLeft;
        private SurfaceButton _dropTargetRight;
        private System.Windows.Controls.Image _plusImageLeft;
        private System.Windows.Controls.Image _plusImageRight;

        private bool _playlistOpened;

        private ICommand selectCommand;
        private ICommand prelistenCommand;
        private ICommand clickedResultCommand;
        private ICommand _addTrackCommand;
        private ICommand _loadDetailsCommand;
        private ObservableCollection<ResultDataModel> _results;


        [ImportingConstructor]
        public ResultViewModel(IResultView view)
            : base(view)
        {
            
            _results = new ObservableCollection<ResultDataModel>();
            view.AddResources();
        }


        public bool IsEnabled { get { return true; } }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public ObservableCollection<ResultDataModel> Results
        {
            get { return _results; }
            set
            {
                if (_results != value)
                {
                    _results = value;
                    RaisePropertyChanged("Results");
                }
            }
        }

        public Result Result
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    result = value;
                    RaisePropertyChanged("Result");
                }
            }
        }

        public SurfaceButton DropTargetLeft
        {
            get { return _dropTargetLeft; }
            set
            {
                if (_dropTargetLeft != value)
                {
                    _dropTargetLeft = value;
                    RaisePropertyChanged("DropTargetLeft");
                }
            }
        }

        public SurfaceButton DropTargetRight
        {
            get { return _dropTargetRight; }
            set
            {
                if (_dropTargetRight != value)
                {
                    _dropTargetRight = value;
                    RaisePropertyChanged("DropTargetRight");
                }
            }
        }

        public System.Windows.Controls.Image PlusImageLeft
        {
            get { return _plusImageLeft; }
            set
            {
                if (_plusImageLeft != value)
                {
                    _plusImageLeft = value;
                    RaisePropertyChanged("PlusImageLeft");
                }
            }
        }

        public System.Windows.Controls.Image PlusImageRight
        {
            get { return _plusImageRight; }
            set
            {
                if (_plusImageRight != value)
                {
                    _plusImageRight = value;
                    RaisePropertyChanged("PlusImageRight");
                }
            }
        }

        public bool PlaylistOpened
        {
            get { return _playlistOpened; }
            set
            {
                if (_playlistOpened != value)
                {
                    _playlistOpened = value;
                    RaisePropertyChanged("PlaylistOpened");
                }
            }
        }

        public ICommand SelectCommand
        {
            get { return selectCommand; }
            set
            {
                if (selectCommand != value)
                {
                    selectCommand = value;
                    RaisePropertyChanged("SelectCommand");
                }
            }
        }

        public ICommand PrelistenCommand
        {
            get { return prelistenCommand; }
            set
            {
                if (prelistenCommand != value)
                {
                    prelistenCommand = value;
                    RaisePropertyChanged("PrelistenCommand");
                }
            }
        }


        public ICommand ClickedResultCommand
        {
            get { return clickedResultCommand; }
            set
            {
                if (clickedResultCommand != value)
                {
                    clickedResultCommand = value;
                    RaisePropertyChanged("ClickedResultCommand");
                }
            }
        }

        public ICommand AddTrackCommand
        {
            get { return _addTrackCommand; }
            set
            {
                if (_addTrackCommand != value)
                {
                    _addTrackCommand = value;
                    RaisePropertyChanged("AddTrackCommand");
                }
            }
        }

        public ICommand LoadDetailsCommand
        {
            get { return _loadDetailsCommand; }
            set
            {
                if (_loadDetailsCommand != value)
                {
                    _loadDetailsCommand = value;
                    RaisePropertyChanged("LoadDetailsCommand");
                }
            }
        }
    }
}
