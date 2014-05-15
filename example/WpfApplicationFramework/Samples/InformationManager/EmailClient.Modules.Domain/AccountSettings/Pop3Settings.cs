﻿using System.ComponentModel.DataAnnotations;

namespace Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings
{
    public class Pop3Settings : EmailAccountSettings
    {
        private UserCredits pop3UserCredits;
        private UserCredits smtpUserCredits;
        private string pop3ServerPath;
        private string smtpServerPath;


        public Pop3Settings()
        {
            pop3UserCredits = new UserCredits();
            smtpUserCredits = new UserCredits();
        }


        [Required, Display(Name = "POP3 Server")]
        public string Pop3ServerPath
        {
            get { return pop3ServerPath; }
            set
            {
                if (pop3ServerPath != value)
                {
                    pop3ServerPath = value;
                    RaisePropertyChanged("Pop3ServerPath");
                }
            }
        }

        public UserCredits Pop3UserCredits
        {
            get { return pop3UserCredits; }
        }

        [Required, Display(Name = "SMTP Server")]
        public string SmtpServerPath
        {
            get { return smtpServerPath; }
            set
            {
                if (smtpServerPath != value)
                {
                    smtpServerPath = value;
                    RaisePropertyChanged("SmtpServerPath");
                }
            }
        }

        public UserCredits SmtpUserCredits
        {
            get { return smtpUserCredits; }
        }


        public override EmailAccountSettings Clone()
        {
            return new Pop3Settings() 
            { 
                pop3UserCredits = this.pop3UserCredits.Clone(), 
                smtpUserCredits = this.smtpUserCredits.Clone(), 
                pop3ServerPath = this.pop3ServerPath, 
                smtpServerPath = this.smtpServerPath 
            };
        }
    }
}
