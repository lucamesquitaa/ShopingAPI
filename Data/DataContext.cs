using Microsoft.EntityFrameworkCore;
using Backoffice.Models;

namespace Backoffice.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        //DbSet representa as tabelas do banco de dados
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}