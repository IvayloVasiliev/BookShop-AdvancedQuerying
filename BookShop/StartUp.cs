namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                var result = GetGoldenBooks(db);
                Console.WriteLine(result);
            }
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(a => a.AgeRestriction == ageRestriction)
                .Select(t => t.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;        
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var command = "Gold";
            var editionType = Enum.Parse<EditionType>(command, true);

            var books = context.Books
                .Where(b => b.EditionType == editionType && b.Copies < 5000 )
                .OrderBy(x => x.BookId)
                .Select(t => t.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;

        }


    }
}
