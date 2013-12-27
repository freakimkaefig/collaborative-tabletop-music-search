using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Ctms.Applications.Services
{
    [Export(typeof(IEntityService)), Export]
    internal class EntityService : IEntityService
    {
        /*
        private BookLibraryEntities entities;
        private ObservableCollection<Book> books;
        private ObservableCollection<Person> persons;


        public BookLibraryEntities Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        public ObservableCollection<Book> Books
        {
            get
            {
                if (books == null && entities != null)
                {
                    // We need to call ToArray so that Include will be executed (Eager loading).
                    entities.Books.Include("LendTo").ToArray();
                    books = new EntityObservableCollection<Book>(entities.Books);
                }
                return books;
            }
        }

        public ObservableCollection<Person> Persons
        {
            get
            {
                if (persons == null && entities != null)
                {
                    persons = new EntityObservableCollection<Person>(entities.Persons);
                }
                return persons;
            }
        }
         */ 
    }
}
