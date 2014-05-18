using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.BookLibrary.Library.Applications.DataModels;
using Waf.BookLibrary.Library.Applications.Views;
using Waf.BookLibrary.Library.Domain;

namespace Waf.BookLibrary.Library.Applications.ViewModels
{
    public class BookListViewModel : ViewModel<IBookListView>
    {
        private readonly IEnumerable<BookDataModel> books;
        private readonly ObservableCollection<BookDataModel> selectedBooks;
        private BookDataModel selectedBook;
        private ICommand addNewCommand;
        private ICommand removeCommand;
        private string filterText = "";

        
        public BookListViewModel(IBookListView view, IEnumerable<BookDataModel> books)
            : base(view)
        {
            if (books == null) { throw new ArgumentNullException("books"); }
            
            this.books = books;
            this.selectedBooks = new ObservableCollection<BookDataModel>();
        }


        public IEnumerable<BookDataModel> Books { get { return books; } }

        public ObservableCollection<BookDataModel> SelectedBooks
        {
            get { return selectedBooks; }
        }

        public IEnumerable<BookDataModel> BookCollectionView { get; set; }

        public BookDataModel SelectedBook
        {
            get { return selectedBook; }
            set
            {
                if (selectedBook != value)
                {
                    selectedBook = value;
                    RaisePropertyChanged("SelectedBook");
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

        public bool Filter(BookDataModel bookDataModel)
        {
            if (string.IsNullOrEmpty(filterText)) { return true; }
            
            Book book = bookDataModel.Book;
            return (book.Title == null || book.Title.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                || (book.Author == null || book.Author.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
    }
}
