using System.Collections.Generic;
using System.Collections.ObjectModel;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.EmailClient.Modules.Domain.Emails
{
    public class EmailClientRoot : ValidationModel, IEmailDeletionService
    {
        private readonly ObservableCollection<EmailAccount> emailAccounts;
        private readonly EmailFolder inbox;
        private readonly EmailFolder outbox;
        private readonly EmailFolder sent;
        private readonly EmailFolder drafts;
        private readonly EmailFolder deleted;


        public EmailClientRoot()
        {
            this.emailAccounts = new ObservableCollection<EmailAccount>();
            this.inbox = new EmailFolder(this);
            this.outbox = new EmailFolder(this);
            this.sent = new EmailFolder(this);
            this.drafts = new EmailFolder(this);
            this.deleted = new EmailFolder(this);
        }


        public IEnumerable<EmailAccount> EmailAccounts { get { return emailAccounts; } }

        public EmailFolder Inbox { get { return inbox; } }

        public EmailFolder Outbox { get { return outbox; } }

        public EmailFolder Sent { get { return sent; } }

        public EmailFolder Drafts { get { return drafts; } }

        public EmailFolder Deleted { get { return deleted; } }


        public void AddEmailAccount(EmailAccount emailAccount)
        {
            emailAccounts.Add(emailAccount);
        }

        public void RemoveEmailAccount(EmailAccount emailAccount)
        {
            emailAccounts.Remove(emailAccount);
        }

        public void ReplaceEmailAccount(EmailAccount oldEmailAccount, EmailAccount newEmailAccount)
        {
            int index = emailAccounts.IndexOf(oldEmailAccount);
            emailAccounts[index] = newEmailAccount;
        }

        public void DeleteEmail(EmailFolder emailFolder, Email email)
        {
            emailFolder.Emails.Remove(email);
            
            if (emailFolder != Deleted)
            {
                Deleted.AddEmail(email);    
            }
        }
    }
}
