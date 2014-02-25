﻿using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.BookLibrary.Library.Applications.Views;
using Waf.BookLibrary.Library.Domain;

namespace Waf.BookLibrary.Library.Applications.ViewModels
{
    [Export]
    public class PersonViewModel : ViewModel<IPersonView>
    {
        private bool isValid = true;
        private Person person;
        private ICommand createNewEmailCommand;

        
        [ImportingConstructor]
        public PersonViewModel(IPersonView view)
            : base(view)
        {
        }


        public bool IsEnabled { get { return Person != null; } }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public Person Person
        {
            get { return person; }
            set
            {
                if (person != value)
                {
                    person = value;
                    RaisePropertyChanged("Person");
                    RaisePropertyChanged("IsEnabled");
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
    }
}
