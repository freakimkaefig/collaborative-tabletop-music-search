using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class SearchViewModel : ViewModel<ISearchView>
    {
        private bool isValid = true;
        private Search search;
        private string keywordType;
        private ICommand startSearchCommand;


        [ImportingConstructor]
        public SearchViewModel(ISearchView view)
            : base(view)
        {
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

        public Search Search
        {
            get { return search; }
            set
            {
                if (search != value)
                {
                    search = value;
                    RaisePropertyChanged("Search");
                }
            }
        }

        public string KeywordType
        {
            get { return keywordType; }
            set
            {
                if (keywordType != value)
                {
                    keywordType = value;
                    RaisePropertyChanged("KeywordType");
                }
            }
        }

        public ICommand StartSearchCommand
        {
            get { return startSearchCommand; }
            set
            {
                if (startSearchCommand != value)
                {
                    startSearchCommand = value;
                    RaisePropertyChanged("StartSearchCommand");
                }
            }
        }
    }
}
