using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using System.Waf.Foundation;
using System.Windows.Input;
using Waf.BookLibrary.Library.Applications.Services;
using Waf.BookLibrary.Library.Applications.Views;
using Waf.BookLibrary.Library.Domain;

namespace Waf.BookLibrary.Library.Applications.ViewModels
{
    public class PersonListViewModel : ViewModel<IPersonListView>
    {
        private readonly IEnumerable<Person> persons;
        private readonly ObservableCollection<Person> selectedPersons;
        private Person selectedPerson;
        private ICommand addNewCommand;
        private ICommand removeCommand;
        private ICommand createNewEmailCommand;
        private string filterText = "";
        

        public PersonListViewModel(IPersonListView view, IEnumerable<Person> persons)
            : base(view)
        {
            if (persons == null) { throw new ArgumentNullException("persons"); }

            this.persons = persons;
            this.selectedPersons = new ObservableCollection<Person>();
        }


        public IEnumerable<Person> Persons { get { return persons; } }

        public ObservableCollection<Person> SelectedPersons
        {
            get { return selectedPersons; }
        }

        public IEnumerable<Person> PersonCollectionView { get; set; }

        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                if (selectedPerson != value)
                {
                    selectedPerson = value;
                    RaisePropertyChanged("SelectedPerson");
                }
            }
        }

        public ICommand AddNewCommand
        {
            get { return addNewCommand; }
            set
            {
                if (addNewCommand != value)
                {
                    addNewCommand = value;
                    RaisePropertyChanged("AddNewCommand");
                }
            }
        }

        public ICommand RemoveCommand
        {
            get { return removeCommand; }
            set
            {
                if (removeCommand != value)
                {
                    removeCommand = value;
                    RaisePropertyChanged("RemoveCommand");
                }
            }
        }

        public ICommand CreateNewEmailCommand
        {
            get { return createNewEmailCommand; }
            set
            {
                if (createNewEmailCommand != value)
                {
                    createNewEmailCommand = value;
                    RaisePropertyChanged("CreateNewEmailCommand");
                }
            }
        }

        public string FilterText
        {
            get { return filterText; }
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    RaisePropertyChanged("FilterText");
                }
            }
        }


        public void Focus()
        {
            ViewCore.FocusFirstCell();
        }

        public bool Filter(Person person)
        {
            if (string.IsNullOrEmpty(filterText)) { return true; }
            
            return (person.Firstname == null || person.Firstname.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                || (person.Lastname == null || person.Lastname.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
    }
}
