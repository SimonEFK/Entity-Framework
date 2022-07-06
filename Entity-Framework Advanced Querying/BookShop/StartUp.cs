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
            DbInitializer.ResetDatabase(db);
            
            string inpuLine = Console.ReadLine();
            var result = GetBooksByAgeRestriction(db, inpuLine);
            Console.WriteLine(result);


        }



        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageGroup = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .ToList()
                .Where(x => x.AgeRestriction == ageGroup)
                .Select(x=>new
                {
                    x.Title
                    
                })
                .OrderBy(x => x.Title);


            var sb = new StringBuilder();
            sb.AppendLine(string.Join(Environment.NewLine, books.Select(x => x.Title)));

            return sb.ToString().TrimEnd();
        }
    }
}
