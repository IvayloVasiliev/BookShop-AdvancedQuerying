﻿namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Z.EntityFramework.Plus;

    public class StartUp
    {
        public static void Main()
        {
            using (var context = new BookShopContext())
            {
                var result = RemoveBooks(context);
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

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(fn => fn.FirstName.EndsWith(input))
                .OrderBy(x=>x)
                .Select(x => new
                {
                    FullName = x.FirstName + " " + x.LastName
                })
                .ToList();

            string result = string.Join(Environment.NewLine, authors.Select(x => $"{x.FullName}"));

            return result;

        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.Contains(input))
                .Select(t => t.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {

            var books = context.Books
                .Where(a => EF.Functions.Like(a.Author.LastName, $"{input}%"))
                .OrderBy(i => i.BookId)
                .Select(x => new
                {
                    x.Title,
                    x.Author.FirstName,
                    x.Author.LastName
                })
                .ToList();

            string result = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} " +
            $"({x.FirstName} {x.LastName})"));

            return result;
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            return context.Books
                .Count(t => t.Title.Length > lengthCheck);       
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                   .Select(x => new
                   {
                       FullName = x.FirstName + " " + x.LastName,
                       BooksCount = x.Books.Sum(c => c.Copies)
                   })
                   .OrderByDescending(b=>b.BooksCount)
                   .ToList();

            string result = string.Join(Environment.NewLine, authors.Select(x => $"{x.FullName} - " +
            $"{x.BooksCount}"));

            return result;
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    TotalProfit = x.CategoryBooks.Sum(s=> s.Book.Price * s.Book.Copies)
                })
                .OrderByDescending(t=> t.TotalProfit)
                .ThenBy(c=> c.CategoryName)
                .ToList();

            string result = string.Join(Environment.NewLine, categories.Select(x => $"{x.CategoryName} " +
            $"${x.TotalProfit}"));

            return result;
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    Books = x.CategoryBooks.Select(e => new
                    {
                        e.Book.Title,
                        e.Book.ReleaseDate
                    })
                        .OrderByDescending(e => e.ReleaseDate)
                        .Take(3)
                        .ToList()
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{ category.CategoryName}");

                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //BULK Operation with Z.EntityFramework.Plus.EFCore
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .Update(x => new Book() { Price = x.Price + 5 });

            context.SaveChanges();  
        }

        //BULK Operation with Z.EntityFramework.Plus.EFCore
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(c => c.Copies < 4200)
                .Delete();

            return books;       
        }





    }
}
