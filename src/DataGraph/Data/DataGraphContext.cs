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

            modelBuilder.Entity<DataGraphObject>()
                .HasKey(i => new { i.CustomerId, i.GraphId, i.ObjectId });
            modelBuilder.Entity<DataGraphObject>().Property(p => p.ObjectId).ValueGeneratedOnAdd();

            modelBuilder.Entity<DataGraphLiteralPropertyValue>()
                .HasKey(i => new { i.CustomerId, i.GraphId, i.ObjectId, i.PropertyName });

            modelBuilder.Entity<DataGraphReferencePropertyValue>()
                .HasKey(i => new { i.CustomerId, i.GraphId, i.ObjectId, i.PropertyName });

            modelBuilder.Entity<DataGraphListOfLiteralsPropertyValue>()
                .HasKey(i => new { i.CustomerId, i.GraphId, i.ObjectId, i.PropertyName, i.ListItemId });

            modelBuilder.Entity<DataGraphListOfReferencesPropertyValue>()
                .HasKey(i => new { i.CustomerId, i.GraphId, i.ObjectId, i.PropertyName, i.ReferencedObjectId });

            //modelBuilder.Entity<DataGraphObject>()
            //    .HasOne(i => i.Graph)
            //    .WithMany(g => g.Objects);

            //modelBuilder.Entity<DataGraphListOfReferencesPropertyValue>()
            //    .HasOne(i => i.Object)
            //    .WithMany(o => o.ListOfReferencesPropertyValues);

            modelBuilder.Entity<DataGraphReferencePropertyValue>()
                .HasOne(i => i.ReferencedObject)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DataGraphListOfReferencesPropertyValue>()
                .HasOne(i => i.ReferencedObject)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DataGraph.Models.DataGraphInstance> DataGraph { get; set; }

        public DbSet<DataGraphObject> Objects { get; set; }

        public DbSet<DataGraphLiteralPropertyValue> LiteralPropertyValues { get; set; }

        public DbSet<DataGraphReferencePropertyValue> ReferencePropertyValues { get; set; }

        public DbSet<DataGraphListOfLiteralsPropertyValue> ListOfLiterals { get; set; }

        public DbSet<DataGraphListOfReferencesPropertyValue> ListOfReferences { get; set; }
    }
}
