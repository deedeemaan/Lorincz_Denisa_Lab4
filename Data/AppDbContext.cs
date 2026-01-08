using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Lorincz_Denisa_Lab4.Models;
using System.Collections.Generic;

namespace Lorincz_Denisa_Lab4.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        public DbSet<PredictionHistory> PredictionHistories { get; set; }
    }
}