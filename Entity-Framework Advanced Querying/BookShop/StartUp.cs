using System;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;


namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //var inpuLine = Console.ReadLine();
            var result = CountCopiesByAuthor(db);
            Console.WriteLine(result);


        }


        public static string CountCopiesByAuthor(BookShopContext context)
        {

            var author = context.Authors.Select(x => new
            {
                x.FirstName,
                x.LastName,
                totalBooksCopies = x.Books.Sum(y => y.Copies)
            })
                .ToList()
                .OrderByDescending(x => x.totalBooksCopies);

            return string.Join(Environment.NewLine,
                author.Select(x => $"{x.FirstName} {x.LastName} - {x.totalBooksCopies}"));


        }


        public static int CountBooks(BookShopContext context, int lengthCheck)
            => context.Books.Where(x => x.Title.Length > lengthCheck).ToList().Count();
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var pattern = $"{input}%";
            var books = context.Books.Where(x => EF.Functions.Like(x.Author.LastName, pattern))
                .Select(x => new
                {
                    x.BookId,
                    x.Author.FirstName,
                    x.Author.LastName,
                    x.Title

                })
                .ToList()
                .OrderBy(x => x.BookId);

            return string.Join(Environment.NewLine, books.Select(x => $"{x.Title} ({x.FirstName} {x.LastName})"));

        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var pattern = $"%{input}%";

            var books = context.Books.Where(x => EF.Functions.Like(x.Title, pattern)).Select(x => new
            {
                x.Title
            }).ToList().OrderBy(x => x.Title);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));

        }
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {

            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new { x.FirstName, x.LastName })
                .ToList()
                .OrderBy(x => (x.FirstName + ' ' + x.LastName));

            return string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName}"));
        }
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var maxDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var books = context.Books.Where(x => x.ReleaseDate.Value < maxDate).Select(x => new
            {
                bookName = x.Title,
                bookEdition = x.EditionType.ToString(),
                bookPrice = x.Price,
                bookReleaseDate = x.ReleaseDate.Value,
            })
                .ToList()
                .OrderByDescending(x => x.bookReleaseDate);

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine(
                    $"{book.bookName} - {book.bookEdition} - ${book.bookPrice:F2}");//{book.bookReleaseDate}");
            }

            return sb.ToString().TrimEnd();

        }
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var bookCategories = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToUpper());


            var books = context.Books
                .Where(x => x.BookCategories.Any(y => bookCategories.Contains(y.Category.Name.ToUpper()) == true))
                .Select(x => new
                {
                    x.Title,
                    categoryName = x.BookCategories.Select(y => new { y.Category.Name })

                })
                .OrderBy(x => x.Title)
                .ToList();



            return string.Join(Environment.NewLine, books.Select(x => x.Title));


        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {

            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new { x.Title, x.BookId })
                .ToList()
                .OrderBy(x => x.BookId);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));



        }
        public static string GetBooksByPrice(BookShopContext context)
        {
            var bookMinPrice = 40M;

            var books = context.Books
                .Where(x => x.Price > bookMinPrice)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookCopies = 5000;

            var books = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < bookCopies)
                .Select(x => new
                {
                    x.BookId,
                    x.Title

                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();
            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageGroup = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books

                .Where(x => x.AgeRestriction == ageGroup)
                .Select(x => new
                {
                    x.Title

                })
                .OrderBy(x => x.Title)
                .ToList();


            var sb = new StringBuilder();
            sb.AppendLine(string.Join(Environment.NewLine, books.Select(x => x.Title)));

            return sb.ToString().TrimEnd();
        }

    }
}
