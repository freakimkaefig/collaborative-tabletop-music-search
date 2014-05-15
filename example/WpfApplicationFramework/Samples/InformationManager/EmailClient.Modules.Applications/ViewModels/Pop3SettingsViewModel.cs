using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings;

namespace Waf.InformationManager.EmailClient.Modules.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class Pop3SettingsViewModel : ViewModel<IPop3SettingsView>
    {
        private Pop3Settings model;
        private bool useSameUserCredits;


        [ImportingConstructor]
        public Pop3SettingsViewModel(IPop3SettingsView view) : base(view)
        {
        }


        public Pop3Settings Model 
        { 
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    AddWeakEventListener(model.Pop3UserCredits, Pop3UserCreditsPropertyChanged);
                    RaisePropertyChanged("Model");
                }
            }
        }

        public bool UseSameUserCredits
        {
            get { return useSameUserCredits; }
            set
            {
                if (useSameUserCredits != value)
                {
                    useSameUserCredits = value;
                    if (useSameUserCredits)
                    {
                        Model.SmtpUserCredits.UserName = Model.Pop3UserCredits.UserName;
                        Model.SmtpUserCredits.Password = Model.Pop3UserCredits.Password;
                    }
                    RaisePropertyChanged("UseSameUserCredits");
                }
            }
        }

        private void Pop3UserCreditsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserName")
            {
                if (UseSameUserCredits)
                {
                    Model.SmtpUserCredits.UserName = Model.Pop3UserCredits.UserName;
                }
            }
            else if (e.PropertyName == "Password")
            {
                if (UseSameUserCredits)
                {
                    Model.SmtpUserCredits.Password = Model.Pop3UserCredits.Password;
                }
            }
        }
    }
}
