using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Reflection;
using System.Waf.Foundation;
using System.Waf.UnitTesting;
using Waf.BookLibrary.Library.Domain.Properties;

namespace Waf.BookLibrary.Library.Domain.Test
{
    [TestClass]
    public class BookTest
    {
        [TestMethod]
        public void GeneralBookTest()
        {
            Book book = new Book();
            Assert.IsNotNull(book.Id);

            book.Title = "Star Wars - Heir to the Empire";
            book.Author = "Timothy Zahn";
            book.Publisher = "Spectra";
            book.PublishDate = new DateTime(1992, 5, 1);
            book.Isbn = "0553296124";
            book.Language = Language.English;
            book.Pages = 416;

            Assert.AreEqual("", book.Validate());

            Assert.AreEqual("Star Wars - Heir to the Empire by Timothy Zahn", 
                book.ToString(null, CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void BookLanguagePropertyChangedTest()
        {
            Book book = new Book();
            book.Language = Language.English;

            AssertHelper.PropertyChangedEvent(book, x => x.Language, () => book.Language = Language.German);
            Assert.AreEqual(Language.German, book.Language);
        }

        [TestMethod]
        public void BookLendToPropertyChangedTest()
        {
            Book book = new Book();
            Assert.IsNull(book.LendTo);

            Person person = new Person();
            AssertHelper.PropertyChangedEvent(book, x => x.LendTo, () => book.LendTo = person);
            Assert.AreEqual(person, book.LendTo);
        }

        [TestMethod]
        public void BookTitleValidationTest()
        {
            Book book = new Book();

            Assert.IsNull(book.Title);
            Assert.AreEqual(Resources.TitleMandatory, book.Validate("Title"));

            book.Title = TestHelper.CreateString('A', 101);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.TitleMaxLength, "Title", 100), 
                book.Validate("Title"));

            book.Title = TestHelper.CreateString('A', 100);
            Assert.AreEqual("", book.Validate("Title"));
        }

        [TestMethod]
        public void BookAuthorValidationTest()
        {
            Book book = new Book();

            Assert.IsNull(book.Author);
            Assert.AreEqual(Resources.AuthorMandatory, book.Validate("Author"));

            book.Author = TestHelper.CreateString('A', 101);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.AuthorMaxLength, "Author", 100),
                book.Validate("Author"));

            book.Author = TestHelper.CreateString('A', 100);
            Assert.AreEqual("", book.Validate("Author"));
        }

        [TestMethod]
        public void BookPublisherValidationTest()
        {
            Book book = new Book();

            Assert.IsNull(book.Publisher);
            Assert.AreEqual("", book.Validate("Publisher"));

            book.Publisher = TestHelper.CreateString('A', 101);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.PublisherMaxLength, "Publisher", 100),
                book.Validate("Publisher"));

            book.Publisher = TestHelper.CreateString('A', 100);
            Assert.AreEqual("", book.Validate("Publisher"));
        }

        [TestMethod]
        public void BookIsbnValidationTest()
        {
            Book book = new Book();

            Assert.IsNull(book.Isbn);
            Assert.AreEqual("", book.Validate("Isbn"));

            book.Isbn = TestHelper.CreateString('A', 15);
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.IsbnMaxLength, "Isbn", 14),
                book.Validate("Isbn"));

            book.Isbn = TestHelper.CreateString('A', 14);
            Assert.AreEqual("", book.Validate("Isbn"));
        }

        [TestMethod]
        public void BookPagesValidationTest()
        {
            Book book = new Book();

            Assert.AreEqual(0, book.Pages);
            Assert.AreEqual("", book.Validate("Pages"));

            book.Pages = -1;
            Assert.AreEqual(string.Format(CultureInfo.CurrentCulture, Resources.PagesEqualOrLarger, "Pages", 0),
                book.Validate("Pages"));

            book.Pages = 400;
            Assert.AreEqual("", book.Validate("Pages"));
        }
    }
}
