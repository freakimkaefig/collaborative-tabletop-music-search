﻿using System.Collections.Generic;
using System.Linq;
using Waf.BookLibrary.Library.Domain;

namespace Waf.BookLibrary.Reporting.Applications.DataModels
{
    public class BorrowedBooksReportDataModel
    {
        private readonly IEnumerable<IGrouping<Person, Book>> groupedBooks;
        

        public BorrowedBooksReportDataModel(IEnumerable<Book> books)
        {
            this.groupedBooks = from book in books
                                group book by book.LendTo into grp
                                where grp.Key != null
                                orderby grp.Key.Firstname
                                select grp;
        }


        public IEnumerable<IGrouping<Person, Book>> GroupedBooks { get { return groupedBooks; } }
    }
}
