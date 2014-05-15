using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waf.InformationManager.EmailClient.Modules.Domain.Emails
{
    public interface IEmailDeletionService
    {
        void DeleteEmail(EmailFolder emailFolder, Email email);
    }
}
