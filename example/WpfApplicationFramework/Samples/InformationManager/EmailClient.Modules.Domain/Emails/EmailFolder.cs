using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.EmailClient.Modules.Domain.Emails
{
    public class EmailFolder : ValidationModel
    {
        private readonly IEmailDeletionService emailDeletionService;
        private readonly ObservableCollection<Email> emails;


        public EmailFolder(IEmailDeletionService emailDeletionService)
        {
            this.emailDeletionService = emailDeletionService;
            this.emails = new ObservableCollection<Email>();
        }


        public ICollection<Email> Emails { get { return emails; } }


        public void AddEmail(Email email)
        {
            emails.Add(email);
        }

        public void RemoveEmail(Email email)
        {
            emailDeletionService.DeleteEmail(this, email);
        }
    }
}
