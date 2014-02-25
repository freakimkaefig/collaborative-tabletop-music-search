using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Waf.BookLibrary.Library.Domain.Properties;
using System.Waf.Foundation;

namespace Waf.BookLibrary.Library.Domain
{
    internal interface IBook
    {
        [Required(ErrorMessageResourceName = "TitleMandatory", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(100, ErrorMessageResourceName = "TitleMaxLength", ErrorMessageResourceType = typeof(Resources))]
        string Title { get; set; }

        [Required(ErrorMessageResourceName = "AuthorMandatory", ErrorMessageResourceType = typeof(Resources))]
        [StringLength(100, ErrorMessageResourceName = "AuthorMaxLength", ErrorMessageResourceType = typeof(Resources))]
        string Author { get; set; }

        [StringLength(100, ErrorMessageResourceName = "PublisherMaxLength", ErrorMessageResourceType = typeof(Resources))]
        string Publisher { get; set; }

        [StringLength(14, ErrorMessageResourceName = "IsbnMaxLength", ErrorMessageResourceType = typeof(Resources))]
        string Isbn { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "PagesEqualOrLarger", ErrorMessageResourceType = typeof(Resources))]
        int Pages { get; set; }
    }
    
    
    [MetadataType(typeof(IBook))]
    public partial class Book : IBook, IDataErrorInfo, IFormattable
    {
        [NonSerialized]
        private readonly DataErrorInfoSupport dataErrorInfoSupport;
        

        public Book()
        {
            // SQL Server Compact does not support entities with server-generated keys or values when it is used 
            // with the Entity Framework. Therefore, we need to create the keys ourselves.
            // See also: http://technet.microsoft.com/en-us/library/cc835494.aspx
            Id = Guid.NewGuid();

            dataErrorInfoSupport = new DataErrorInfoSupport(this);

            LendToReference.AssociationChanged += LendToReferenceAssociationChanged;
        }


        public Language Language
        {
            // Entity Framework doesn't support Enums. We use an Int32 value internal and cast it to our enum.
            get { return (Language)LanguageInternal; }
            set { LanguageInternal = (int)value; }
        }

        string IDataErrorInfo.Error { get { return dataErrorInfoSupport.Error; } }

        string IDataErrorInfo.this[string memberName] { get { return dataErrorInfoSupport[memberName]; } }


        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, Resources.BookToString, Title, Author);
        }

        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);

            if (property == "LanguageInternal")
            {
                OnPropertyChanged("Language");
            }
        }

        private void LendToReferenceAssociationChanged(object sender, CollectionChangeEventArgs e)
        {
            // The navigation property LendTo doesn't support the PropertyChanged event. We have to raise it ourselves.
            OnPropertyChanged("LendTo");
        }
    }
}
