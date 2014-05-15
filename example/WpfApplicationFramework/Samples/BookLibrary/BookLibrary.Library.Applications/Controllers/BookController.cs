using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Waf.Applications;
using Waf.BookLibrary.Library.Applications.DataModels;
using Waf.BookLibrary.Library.Applications.Services;
using Waf.BookLibrary.Library.Applications.ViewModels;
using Waf.BookLibrary.Library.Applications.Views;
using Waf.BookLibrary.Library.Domain;
using System.Waf.Foundation;

namespace Waf.BookLibrary.Library.Applications.Controllers
{
    /// <summary>
    /// Responsible for the book management and the master / detail views.
    /// </summary>
    [Export]
    internal class BookController : Controller
    {
        private readonly CompositionContainer container;
        private readonly IShellService shellService;
        private readonly IEntityService entityService;
        private readonly BookViewModel bookViewModel;
        private readonly DelegateCommand addNewCommand;
        private readonly DelegateCommand removeCommand;
        private readonly DelegateCommand lendToCommand;
        private BookListViewModel bookListViewModel;
        private SynchronizingCollection<BookDataModel, Book> bookDataModels;
        

        [ImportingConstructor]
        public BookController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            BookViewModel bookViewModel)
        {
            this.container = container;
            this.shellService = shellService;
            this.entityService = entityService;
            this.bookViewModel = bookViewModel;
            this.addNewCommand = new DelegateCommand(AddNewBook, CanAddNewBook);
            this.removeCommand = new DelegateCommand(RemoveBook, CanRemoveBook);
            this.lendToCommand = new DelegateCommand(p => LendTo((Book)p));
        }


        public void Initialize()
        {
            bookViewModel.LendToCommand = lendToCommand;
            AddWeakEventListener(bookViewModel, BookViewModelPropertyChanged);

            IBookListView bookListView = container.GetExportedValue<IBookListView>();
            bookDataModels = new SynchronizingCollection<BookDataModel, Book>(entityService.Books, 
                b => new BookDataModel(b, lendToCommand));
            bookListViewModel = new BookListViewModel(bookListView, bookDataModels);
            bookListViewModel.AddNewCommand = addNewCommand;
            bookListViewModel.RemoveCommand = removeCommand;
            AddWeakEventListener(bookListViewModel, BookListViewModelPropertyChanged);

            shellService.BookListView = bookListViewModel.View;
            shellService.BookView = bookViewModel.View;

            bookListViewModel.SelectedBook = bookListViewModel.Books.FirstOrDefault();
        }

        private bool CanAddNewBook() { return bookViewModel.IsValid; }

        private void AddNewBook()
        {
            Book book = new Book();
            entityService.Books.Add(book);

            bookListViewModel.SelectedBook = bookDataModels.Single(b => b.Book == book);
            bookListViewModel.Focus();
        }

        private bool CanRemoveBook() { return bookListViewModel.SelectedBook != null; }

        private void RemoveBook()
        {
            // Use the BookCollectionView, which represents the sorted/filtered state of the books, to determine the next book to select.
            IEnumerable<BookDataModel> booksToExclude = bookListViewModel.SelectedBooks.Except(new[] { bookListViewModel.SelectedBook });
            BookDataModel nextBook = CollectionHelper.GetNextElementOrDefault(bookListViewModel.BookCollectionView.Except(booksToExclude), 
                bookListViewModel.SelectedBook);
            
            foreach (BookDataModel book in bookListViewModel.SelectedBooks.ToArray())
            {
                entityService.Books.Remove(book.Book);
            }

            bookListViewModel.SelectedBook = nextBook ?? bookListViewModel.BookCollectionView.LastOrDefault();
            bookListViewModel.Focus();
        }

        private void LendTo(Book book)
        {
            LendToViewModel lendToViewModel = new LendToViewModel(container.GetExportedValue<ILendToView>(), book, entityService.Persons);
            if (lendToViewModel.ShowDialog(shellService.ShellView))
            {
                book.LendTo = lendToViewModel.SelectedPerson;
            }
        }

        private void UpdateCommands()
        {
            addNewCommand.RaiseCanExecuteChanged();
            removeCommand.RaiseCanExecuteChanged();
            lendToCommand.RaiseCanExecuteChanged();
        }

        private void BookListViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedBook")
            {
                bookViewModel.Book = bookListViewModel.SelectedBook != null ? bookListViewModel.SelectedBook.Book : null;
                UpdateCommands();
            }
        }

        private void BookViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid")
            {
                UpdateCommands();
            }
        }
    }
}
