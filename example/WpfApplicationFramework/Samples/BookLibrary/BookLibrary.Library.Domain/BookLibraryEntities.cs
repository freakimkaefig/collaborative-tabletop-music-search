using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Waf.Foundation;
using Waf.BookLibrary.Library.Domain.Properties;
using System.Collections.Generic;

namespace Waf.BookLibrary.Library.Domain
{
    partial class BookLibraryEntities
    {
        static BookLibraryEntities()
        {
            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(EntityObject)),
                typeof(EntityObject));
        }
        

        public bool HasChanges
        {
            get
            {
                DetectChanges();
                return ObjectStateManager.GetObjectStateEntries(EntityState.Added).Any()
                    || ObjectStateManager.GetObjectStateEntries(EntityState.Modified).Any()
                    || ObjectStateManager.GetObjectStateEntries(EntityState.Deleted).Any();
            }
        }


        partial void OnContextCreated()
        {
            SavingChanges += SavingChangesHandler;
        }

        private void SavingChangesHandler(object sender, EventArgs e)
        {
            List<string> errorList = new List<string>();
            
            foreach (ObjectStateEntry entry in ObjectStateManager.GetObjectStateEntries(
                EntityState.Added | EntityState.Modified))
            {
                IDataErrorInfo entity = entry.Entity as IDataErrorInfo;
                if (entity != null)
                {
                    string error = entity.Validate();
                    if (!string.IsNullOrEmpty(error))
                    {
                        errorList.Add(string.Format(CultureInfo.CurrentCulture, Resources.EntityInvalid,
                            EntityToString(entity), error));
                    }
                }
            }

            string errorMessage = string.Join(Environment.NewLine, errorList);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new ValidationException(errorMessage);
            }
        }

        private static string EntityToString(object entity)
        {
            IFormattable formattableEntity = entity as IFormattable;
            if (formattableEntity != null)
            {
                return formattableEntity.ToString(null, CultureInfo.CurrentCulture);
            }
            return entity.ToString();
        }
    }
}
