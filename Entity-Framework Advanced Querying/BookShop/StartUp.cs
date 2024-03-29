﻿using System;
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
            
        }

        //15
        public static int RemoveBooks(BookShopContext context)
        {

            var booksToRemove = context.Books.Where(x => x.Copies < 4200).ToList();

            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();
            return booksToRemove.Count;
        }
        //14
        public static void IncreasePrices(BookShopContext context)
        {
            var maximumYear = 2010;

            var books = context.Books.Where(x => x.ReleaseDate.Value.Year < maximumYear);
            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();

        }
        //13
        public static string GetMostRecentBooks(BookShopContext context)
        {

            var categories = context.Categories
                .Select(x => new
                {

                    x.Name,
                    top3MostRecentBooks = x.CategoryBooks.Select(y => new
                    {
                        bookName = y.Book.Title,
                        bookReleaseDate = y.Book.ReleaseDate
                    })
                    .OrderByDescending(book => book.bookReleaseDate)
                    .Take(3)

                })
                .ToList()
                .OrderBy(x => x.Name);

            var sb = new StringBuilder();
            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.top3MostRecentBooks)
                {
                    sb.AppendLine($"{book.bookName} ({book.bookReleaseDate.Value.Year})");
                }
            }
            return sb.ToString().TrimEnd();
        }
        //12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {

            var categories = context.Categories.Select(x => new
            {
                x.Name,
                totalProfit = x.CategoryBooks.Sum(y => y.Book.Price * y.Book.Copies)

            })
                .ToList()
                .OrderByDescending(x => x.totalProfit)
                .ThenBy(category => category.Name);

            return string.Join(Environment.NewLine, categories.Select(x => $"{x.Name} ${x.totalProfit:F2}"));


        }
        //11
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
        //10
        public static int CountBooks(BookShopContext context, int lengthCheck)
            => context.Books.Where(x => x.Title.Length > lengthCheck).ToList().Count();
        //09
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
        //08
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var pattern = $"%{input}%";

            var books = context.Books.Where(x => EF.Functions.Like(x.Title, pattern)).Select(x => new
            {
                x.Title
            }).ToList().OrderBy(x => x.Title);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));

        }
        //07
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {

            var authors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .Select(x => new { x.FirstName, x.LastName })
                .ToList()
                .OrderBy(x => (x.FirstName + ' ' + x.LastName));

            return string.Join(Environment.NewLine, authors.Select(x => $"{x.FirstName} {x.LastName}"));
        }
        //06
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
        //05
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
        //04
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {

            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new { x.Title, x.BookId })
                .ToList()
                .OrderBy(x => x.BookId);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));



        }
        //03
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
        //02
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
        //01
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
