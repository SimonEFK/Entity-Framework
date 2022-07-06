﻿using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using BookShop.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;


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

            string inpuLine = Console.ReadLine();
            var result = GetBookTitlesContaining(db, inpuLine);
            Console.WriteLine(result);


        }




        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            string pattern = $"%{input}%";

            var books = context.Books.Where(x=>EF.Functions.Like(x.Title,pattern)).Select(x=>new
            {
                x.Title
            }).ToList().OrderBy(x=>x.Title);

            return string.Join(Environment.NewLine, books.Select(x => x.Title));


        }












        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {

            var firstNameLastLetters = input;
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

            var sb = new StringBuilder();


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
            decimal bookMinPrice = 40M;

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
            int bookCopies = 5000;

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
