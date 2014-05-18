using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;

namespace Waf.BookLibrary.Library.Applications.Services
{
    internal class EntityObservableCollection<T> : ObservableCollection<T> where T : class
    {
        private readonly IObjectSet<T> objectSet;


        public EntityObservableCollection(IObjectSet<T> objectSet) 
            : base((IEnumerable<T>)objectSet ?? new T[] {})
        {
            if (objectSet == null) { throw new ArgumentNullException("objectSet"); }
            this.objectSet = objectSet;
        }
        

        protected override void InsertItem(int index, T item)
        {
            objectSet.AddObject(item);

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            T itemToDelete = this[index];
            objectSet.DeleteObject(itemToDelete);
            
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            T[] itemsToDelete = this.ToArray();
            foreach (T item in itemsToDelete)
            {
                objectSet.DeleteObject(item);
            }

            base.ClearItems();
        }

        protected override void SetItem(int index, T item)
        {
            T itemToReplace = this[index];
            objectSet.DeleteObject(itemToReplace);
            objectSet.AddObject(item);

            base.SetItem(index, item);
        }
    }
}
