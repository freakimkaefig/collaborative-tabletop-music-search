using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Waf.BookLibrary.Library.Domain;

namespace Waf.BookLibrary.Reporting.Applications.DataModels
{
    public class BookListReportDataModel : DataModel
    {
        private readonly IEnumerable<Book> books;


        public BookListReportDataModel(IEnumerable<Book> books)
        {
            this.books = books;
        }


        public IEnumerable<Book> Books { get { return books; } }

        public int BookCount { get { return books.Count(); } }
    }
}
