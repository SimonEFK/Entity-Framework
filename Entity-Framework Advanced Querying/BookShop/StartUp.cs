using System;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;


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
            
            //string inpuLine = Console.ReadLine();
            var result = GetBooksByPrice(db);
            Console.WriteLine(result);


        }



        public static string GetBooksByPrice(BookShopContext context)
        {
            decimal bookMinPrice = 40M;

            var books = context.Books
                .Where(x=>x.Price>bookMinPrice)
                .Select(x=>new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x=>x.Price)
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
                .Where(x => x.EditionType == EditionType.Gold&&x.Copies< bookCopies)
                .Select(x=>new
            {
                x.BookId,
                x.Title

            })
                .OrderBy(x=>x.BookId)
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
                .Select(x=>new
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
