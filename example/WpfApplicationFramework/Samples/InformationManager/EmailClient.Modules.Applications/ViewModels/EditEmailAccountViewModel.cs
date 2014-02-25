using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;

namespace Waf.InformationManager.EmailClient.Modules.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditEmailAccountViewModel : ViewModel<IEditEmailAccountView>
    {
        private ICommand backCommand;
        private ICommand nextCommand;
        private object contentView;
        private bool isValid = true;
        private bool isLastPage;
        
        
        [ImportingConstructor]
        public EditEmailAccountViewModel(IEditEmailAccountView view) : base(view)
        {
        }


        public ICommand BackCommand 
        { 
            get { return backCommand; }
            set 
            {
                if (backCommand != value)
                {
                    backCommand = value;
                    RaisePropertyChanged("BackCommand");
                }
            }
        }

        public ICommand NextCommand 
        { 
            get { return nextCommand; }
            set 
            {
                if (nextCommand != value)
                {
                    nextCommand = value;
                    RaisePropertyChanged("NextCommand");
                }
            }
        }

        public object ContentView
        {
            get { return contentView; }
            set 
            {
                if (contentView != value)
                {
                    contentView = value;
                    RaisePropertyChanged("ContentView");
                }
            }
        }

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

        public bool IsLastPage
        {
            get { return isLastPage; }
            set
            {
                if (isLastPage != value)
                {
                    isLastPage = value;
                    RaisePropertyChanged("IsLastPage");
                }
            }
        }


        public void ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
        }

        public void Close()
        {
            ViewCore.Close();
        }
    }
}
