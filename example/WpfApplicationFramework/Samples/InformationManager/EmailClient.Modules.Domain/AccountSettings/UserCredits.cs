using System.ComponentModel.DataAnnotations;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings
{
    public class UserCredits : ValidationModel
    {
        private string userName;
        private string password;


        [Required, Display(Name = "Username")]
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

        public string Password
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    RaisePropertyChanged("Password");
                }
            }
        }


        public virtual UserCredits Clone()
        {
            return new UserCredits() { userName = this.userName, password = this.password };
        }
    }
}
