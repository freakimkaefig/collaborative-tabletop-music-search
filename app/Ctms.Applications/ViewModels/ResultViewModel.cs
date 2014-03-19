using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using System.Collections.ObjectModel;
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
        private ICommand clickedResultCommand;
        private ObservableCollection<Result> _results;


        [ImportingConstructor]
        public ResultViewModel(IResultView view)
            : base(view)
        {
            
            _results = new ObservableCollection<Result>()
            {
                new Result() 
                {
                    Song =  new Song()
                    {
                        Title = "Fireworks",
                        Artist = "Katy Perry"
                    }
                },
                new Result() 
                {
                    Song =  new Song()
                    {
                        Title = "Achy Breaky Heart",
                        Artist = "Billy Ray Cyrus"
                    }
                },
                new Result() 
                {
                    Song =  new Song()
                    {
                        Title = "Watercolour",
                        Artist = "Pendulum"
                    }
                }
            };

            //_results = new ObservableCollection<ResultDataModel>();
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

        public ObservableCollection<Result> Results
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
