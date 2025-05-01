using Microsoft.EntityFrameworkCore;
using LibraryReporter.Data.Models;
using static System.Net.Mime.MediaTypeNames;
using System;
using Microsoft.Identity.Client.AppConfig;
using Library.Data.Models;

namespace LibraryReporter.Data
{
    public class WebDbContext : DbContext
    {

        public const string CONNECTION_STRING = "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = LibraryReporter; Integrated Security = True;";
        public DbSet<UserData> Users { get; set; }
        public DbSet<Models.BookData> Books { get; set; }
        public DbSet<Library.Data.Models.ReaderData> Readers { get; set; }
        public DbSet<AuthorData> Authors { get; set; }
        public DbSet<PublisherData> Publishers { get; set; }

        public DbSet<IssuedBookData> IssuedBooks { get; set; }


        public WebDbContext() { }

        public WebDbContext(DbContextOptions<WebDbContext> contextOptions)
            : base(contextOptions) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CONNECTION_STRING);
            // base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<BookData>()
            //    .HasMany(x => x.ClientsWhoTakeIt)
            //    .WithMany(x => x.BooksWhichUserTakes);


        }
    }
}
