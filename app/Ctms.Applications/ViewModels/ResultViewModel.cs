using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using System.Collections.ObjectModel;
using SpotifySharp;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class ResultViewModel : ViewModel<IResultView>
    {
        private bool isValid = true;
        private Result result;
        private string title;
        private ICommand selectCommand;
        private ICommand prelistenCommand;
        private ICommand clickedResultCommand;
        private ObservableCollection<ResultDataModel> _results;


        [ImportingConstructor]
        public ResultViewModel(IResultView view)
            : base(view)
        {
            _results = new ObservableCollection<ResultDataModel>();
        }


        public bool IsEnabled { get { return true; } }//result != null

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

    }
}
