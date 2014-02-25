using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;

namespace Waf.InformationManager.EmailClient.Modules.Applications.ViewModels
{
    [Export]
    public class EmailLayoutViewModel : ViewModel<IEmailLayoutView>
    {
        private object emailListView;
        private object emailView;

        
        [ImportingConstructor]
        public EmailLayoutViewModel(IEmailLayoutView view) : base(view)
        {
        }


        public object EmailListView
        {
            get { return emailListView; }
            set
            {
                if (emailListView != value)
                {
                    emailListView = value;
                    RaisePropertyChanged("EmailListView");
                }
            }
        }

        public object EmailView
        {
            get { return emailView; }
            set
            {
                if (emailView != value)
                {
                    emailView = value;
                    RaisePropertyChanged("EmailView");
                }
            }
        }
    }
}
