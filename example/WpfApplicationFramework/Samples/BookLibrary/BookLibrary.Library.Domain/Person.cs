using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Waf.BookLibrary.Library.Domain.Properties;
using System.Waf.Foundation;

namespace Waf.BookLibrary.Library.Domain
{
    internal interface IPerson
    {
        [Required(ErrorMessageResourceName="FirstnameMandatory", ErrorMessageResourceType=typeof(Resources))]
        [StringLength(30, ErrorMessageResourceName="FirstnameMaxLength", ErrorMessageResourceType=typeof(Resources))]
        string Firstname { get; set; }

        [Required(ErrorMessageResourceName="LastnameMandatory", ErrorMessageResourceType=typeof(Resources))]
        [StringLength(30, ErrorMessageResourceName="LastnameMaxLength", ErrorMessageResourceType=typeof(Resources))]
        string Lastname { get; set; }

        [StringLength(100, ErrorMessageResourceName="EmailMaxLength", ErrorMessageResourceType=typeof(Resources))]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", 
            ErrorMessageResourceName="EmailInvalid", ErrorMessageResourceType=typeof(Resources))]
        string Email { get; set; }
    }

    
    [MetadataType(typeof(IPerson))]
    public partial class Person : IPerson, IDataErrorInfo, IFormattable
    {
        [NonSerialized]
        private readonly DataErrorInfoSupport dataErrorInfoSupport;


        public Person()
        {
            // SQL Server Compact does not support entities with server-generated keys or values when it is used 
            // with the Entity Framework. Therefore, we need to create the keys ourselves.
            // See also: http://technet.microsoft.com/en-us/library/cc835494.aspx
            Id = Guid.NewGuid();

            dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }


        string IDataErrorInfo.Error { get { return dataErrorInfoSupport.Error; } }

        string IDataErrorInfo.this[string memberName] { get { return dataErrorInfoSupport[memberName]; } }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, Resources.PersonToString, Firstname, Lastname);
        }
    }
}
