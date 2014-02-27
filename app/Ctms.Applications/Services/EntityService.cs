using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using MusicSearch.SearchObjects;

namespace Ctms.Applications.Services
{
    //Provides entities (data from database and files). Just getters, no setters, but add to/remove from list is possible.
    [Export]
    public class EntityService
    {
        private ObservableCollection<Style> _styles;

        public ObservableCollection<Style> Styles
        {
            get
            {
                if (_styles == null)
                {   // read and set styles
                    var styles = XmlProvider.ReadStyles();

                    _styles = new ObservableCollection<Style>();                    
                    foreach (var style in styles)
                    {
                        _styles.Add(style);
                    }
                }
                return _styles;
            }
        }


        /*
        //WAF BookLibrary Examples:

        //private BookLibraryEntities entities;
        
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
