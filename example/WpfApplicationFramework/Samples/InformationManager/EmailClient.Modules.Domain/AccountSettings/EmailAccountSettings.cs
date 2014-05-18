using System.Linq;
using Waf.InformationManager.Common.Domain;

namespace Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings
{
    public abstract class EmailAccountSettings : ValidationModel
    {
        public abstract EmailAccountSettings Clone();
    }
}
