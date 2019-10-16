using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataGraph.Models;

namespace DataGraph.Data
{
    public class DataGraphContext : DbContext
    {
        public DataGraphContext (DbContextOptions<DataGraphContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataGraphInstance>()
                .HasKey(i => new { i.CustomerId, i.Id });
            modelBuilder.Entity<DataGraphInstance>().Property(p => p.Id).ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DataGraph.Models.DataGraphInstance> DataGraph { get; set; }
    }
}
