using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Waf.InformationManager.Common.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Globalization;

namespace Waf.InformationManager.EmailClient.Modules.Domain.Emails
{
    public class Email : ValidationModel, IValidatableObject
    {
        private static readonly EmailValidationAttribute emailValidation = new EmailValidationAttribute();

        private EmailType emailType;
        private string title = "";
        private string from = "";
        private IEnumerable<string> to;
        private IEnumerable<string> cc;
        private IEnumerable<string> bcc;
        private DateTime sent;
        private string message;


        public Email()
        {
            to = new string[] { };
            cc = new string[] { };
            bcc = new string[] { };
        }


        public EmailType EmailType
        {
            get { return emailType; }
            set
            {
                if (emailType != value)
                {
                    emailType = value;
                    RaisePropertyChanged("EmailType");
                }
            }
        }

        [StringLength(255), DisplayName("Title")]
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        [StringLength(255), DisplayName("From")]
        public string From
        {
            get { return from; }
            set
            {
                if (from != value)
                {
                    from = value;
                    RaisePropertyChanged("From");
                }
            }
        }

        public IEnumerable<string> To
        {
            get { return to; }
            set
            {
                if (to != value)
                {
                    if (value == null) { throw new ArgumentNullException("value"); }
                    to = value;
                    RaisePropertyChanged("To");
                }
            }
        }

        public IEnumerable<string> CC
        {
            get { return cc; }
            set
            {
                if (cc != value)
                {
                    if (value == null) { throw new ArgumentNullException("value"); }
                    cc = value;
                    RaisePropertyChanged("CC");
                }
            }
        }

        public IEnumerable<string> Bcc
        {
            get { return bcc; }
            set
            {
                if (bcc != value)
                {
                    if (value == null) { throw new ArgumentNullException("value"); }
                    bcc = value;
                    RaisePropertyChanged("Bcc");
                }
            }
        }

        public DateTime Sent
        {
            get { return sent; }
            set
            {
                if (sent != value)
                {
                    sent = value;
                    RaisePropertyChanged("Sent");
                }
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    RaisePropertyChanged("Message");
                }
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            
            foreach (string email in To) { ValidateEmail(validationResults, email, "To", "To"); }
            foreach (string email in CC) { ValidateEmail(validationResults, email, "CC", "CC"); }
            foreach (string email in Bcc) { ValidateEmail(validationResults, email, "Bcc", "BCC"); }

            if (!To.Any() && !CC.Any() && !Bcc.Any())
            {
                validationResults.Add(new ValidationResult("This email doesn't define a recipient.", new[] { "To", "CC", "Bcc" }));
            }

            return validationResults;
        }

        private static void ValidateEmail(ICollection<ValidationResult> validationResults, string email, string field, string displayName)
        {
            if (!emailValidation.IsValid(email))
            {
                validationResults.Add(new ValidationResult(string.Format(CultureInfo.CurrentCulture, 
                    "The email {0} in the {1} field is not valid.", email, displayName), new[] { field }));
            }
        }
    }
}
