namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                var result = GetBooksByPrice(db);
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
           
            var editionType = Enum.Parse<EditionType>("Gold");

            var books = context.Books
                .Where(b => b.EditionType == editionType && b.Copies < 5000 )
                .OrderBy(x => x.BookId)
                .Select(t => t.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;

        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(x => x.Price)
                .Select(x => new 
                {
                    x.Title,
                    x.Price
                })
                .ToList();
           
            var result = string.Join(Environment.NewLine, books.Select(x=> $"{x.Title} - ${x.Price:F2}"));

            return result;
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {

            var books = context.Books
                .Where(d => d.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(t => t.Title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var books = context.Books
                .Where(bc => bc.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .Select(t => t.Title)
                .OrderBy(t => t)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(d => d.ReleaseDate.Value < targetDate)
                .OrderByDescending(r => r.ReleaseDate.Value)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .ToList();

            string result = string.Join(Environment.NewLine, books.Select(x=> $"{x.Title} - {x.EditionType}" +
            $" - ${x.Price:F2}"));

            return result;
        }

    }
}
