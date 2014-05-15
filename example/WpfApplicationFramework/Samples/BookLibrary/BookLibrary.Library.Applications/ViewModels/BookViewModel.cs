﻿using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.BookLibrary.Library.Applications.Views;
using Waf.BookLibrary.Library.Domain;

namespace Waf.BookLibrary.Library.Applications.ViewModels
{
    [Export]
    public class BookViewModel : ViewModel<IBookView>
    {
        private bool isValid = true;
        private Book book;
        private ICommand lendToCommand;

        
        [ImportingConstructor]
        public BookViewModel(IBookView view)
            : base(view)
        {
        }


        public bool IsEnabled { get { return Book != null; } }

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

        public Book Book
        {
            get { return book; }
            set
            {
                if (book != value)
                {
                    book = value;
                    RaisePropertyChanged("Book");
                    RaisePropertyChanged("IsEnabled");
                }
            }
        }

        public ICommand LendToCommand
        {
            get { return lendToCommand; }
            set
            {
                if (lendToCommand != value)
                {
                    lendToCommand = value;
                    RaisePropertyChanged("LendToCommand");
                }
            }
        }
    }
}
