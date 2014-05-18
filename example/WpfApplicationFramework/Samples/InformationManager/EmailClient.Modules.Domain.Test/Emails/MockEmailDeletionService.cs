using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.EmailClient.Modules.Domain.Emails;

namespace Test.InformationManager.EmailClient.Modules.Domain.Emails
{
    public class MockEmailDeletionService : IEmailDeletionService
    {
        public Action<EmailFolder, Email> DeleteEmailAction { get; set; }
        

        public void DeleteEmail(EmailFolder emailFolder, Email email)
        {
            DeleteEmailAction(emailFolder, email);    
        }
    }
}
