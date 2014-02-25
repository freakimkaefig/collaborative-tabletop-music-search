using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.BookLibrary.Library.Applications.Test.Views;
using Waf.BookLibrary.Library.Applications.ViewModels;
using System.Waf.UnitTesting;
using Waf.BookLibrary.Library.Domain;
using System.Waf.Applications;
using Waf.BookLibrary.Library.Applications.Test.Services;

namespace Waf.BookLibrary.Library.Applications.Test.ViewModels
{
    [TestClass]
    public class PersonListViewModelTest
    {
        [TestMethod]
        public void PersonListViewModelPersonsTest()
        {
            List<Person> persons = new List<Person>()
            {
                new Person() { Firstname = "Harry" },
                new Person() { Firstname = "Ron" }
            };
            
            MockPersonListView personListView = new MockPersonListView();

            AssertHelper.ExpectedException<ArgumentNullException>(() => new PersonListViewModel(personListView, null));
            PersonListViewModel personListViewModel = new PersonListViewModel(personListView, persons);

            Assert.AreEqual(persons, personListViewModel.Persons);
            Assert.IsNull(personListViewModel.SelectedPerson);
            Assert.IsFalse(personListViewModel.SelectedPersons.Any());

            // Select the first person
            AssertHelper.PropertyChangedEvent(personListViewModel, x => x.SelectedPerson,
                () => personListViewModel.SelectedPerson = persons.First());
            Assert.AreEqual(persons.First(), personListViewModel.SelectedPerson);
            
            personListViewModel.SelectedPersons.Add(persons.First());
            Assert.IsTrue(personListViewModel.SelectedPersons.SequenceEqual(new Person[] { persons.First() }));

            // Select both persons
            personListViewModel.SelectedPersons.Add(persons.Last());
            Assert.IsTrue(personListViewModel.SelectedPersons.SequenceEqual(persons));
        }

        [TestMethod]
        public void PersonListViewModelCommandsTest()
        {
            MockPersonListView personListView = new MockPersonListView();
            PersonListViewModel personListViewModel = new PersonListViewModel(personListView, new List<Person>());

            DelegateCommand mockCommand = new DelegateCommand(() => {});
            AssertHelper.PropertyChangedEvent(personListViewModel, x => x.AddNewCommand, () => 
                personListViewModel.AddNewCommand = mockCommand);
            Assert.AreEqual(mockCommand, personListViewModel.AddNewCommand);

            mockCommand = new DelegateCommand(() => { });
            AssertHelper.PropertyChangedEvent(personListViewModel, x => x.RemoveCommand, () =>
                personListViewModel.RemoveCommand = mockCommand);
            Assert.AreEqual(mockCommand, personListViewModel.RemoveCommand);

            mockCommand = new DelegateCommand(() => { });
            AssertHelper.PropertyChangedEvent(personListViewModel, x => x.CreateNewEmailCommand, () =>
                personListViewModel.CreateNewEmailCommand = mockCommand);
            Assert.AreEqual(mockCommand, personListViewModel.CreateNewEmailCommand);
        }

        [TestMethod]
        public void PersonListViewModelFilterTest()
        {
            List<Person> persons = new List<Person>()
            {
                new Person() { Firstname = "Harry", Lastname = "Potter" },
                new Person() { Firstname = "Ron", Lastname = "Weasley" }
            };

            MockPersonListView personListView = new MockPersonListView();
            PersonListViewModel personListViewModel = new PersonListViewModel(personListView, persons);

            Assert.IsTrue(personListViewModel.Filter(persons[0]));
            Assert.IsTrue(personListViewModel.Filter(persons[1]));

            AssertHelper.PropertyChangedEvent(personListViewModel, x => x.FilterText, () => personListViewModel.FilterText = "r");
            Assert.AreEqual("r", personListViewModel.FilterText);
            Assert.IsTrue(personListViewModel.Filter(persons[0]));
            Assert.IsTrue(personListViewModel.Filter(persons[1]));

            personListViewModel.FilterText = "arr";
            Assert.IsTrue(personListViewModel.Filter(persons[0]));
            Assert.IsFalse(personListViewModel.Filter(persons[1]));

            personListViewModel.FilterText = "eas";
            Assert.IsFalse(personListViewModel.Filter(persons[0]));
            Assert.IsTrue(personListViewModel.Filter(persons[1]));

            personListViewModel.FilterText = "xyz";
            Assert.IsFalse(personListViewModel.Filter(persons[0]));
            Assert.IsFalse(personListViewModel.Filter(persons[1]));

            persons.Add(new Person());
            Assert.IsTrue(personListViewModel.Filter(persons[2]));
            persons[2].Firstname = "Hermione";
            Assert.IsTrue(personListViewModel.Filter(persons[2]));
        }
    }
}
