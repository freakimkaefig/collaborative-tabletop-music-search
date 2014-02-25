using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings;
using System.ComponentModel.DataAnnotations;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.EmailClient.Modules.Domain.Emails
{
    public class EmailAccount : ValidationModel
    {
        private string name;
        private string email;
        private EmailAccountSettings emailAccountSettings;


        [Required, Display(Name = "Name")]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        [Required, StringLength(100), Display(Name = "Email Address")]
        [EmailValidation]
        public string Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        public EmailAccountSettings EmailAccountSettings
        {
            get { return emailAccountSettings; }
            set
            {
                if (emailAccountSettings != value)
                {
                    emailAccountSettings = value;
                    RaisePropertyChanged("EmailAccountSettings");
                }
            }
        }


        public virtual EmailAccount Clone()
        {
            return new EmailAccount() 
            { 
                name = this.name, 
                email = this.email, 
                emailAccountSettings = this.emailAccountSettings != null ? this.emailAccountSettings.Clone() : null 
            };
        }
    }
}
