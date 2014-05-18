using System.ComponentModel.DataAnnotations;

namespace Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings
{
    public class ExchangeSettings : EmailAccountSettings
    {
        private string serverPath;
        private string userName;


        [Required, Display(Name = "Exchange Server")]
        public string ServerPath
        {
            get { return serverPath; }
            set
            {
                if (serverPath != value)
                {
                    serverPath = value;
                    RaisePropertyChanged("ServerPath");
                }
            }
        }

        [Required, Display(Name = "User Name")]
        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }


        public override EmailAccountSettings Clone()
        {
            return new ExchangeSettings() { serverPath = this.serverPath, userName = this.userName };
        }
    }
}
