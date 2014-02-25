using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Waf.InformationManager.Common.Domain
{
    /// <summary>
    /// Specifies an email data field. This attribute defines the validation rule for verifying an email address.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class EmailValidationAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailValidationAttribute"/> class.
        /// </summary>
        public EmailValidationAttribute() : base(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$")
        {
            ErrorMessage = "The provided email address is invalid.";
        }
    }
}
