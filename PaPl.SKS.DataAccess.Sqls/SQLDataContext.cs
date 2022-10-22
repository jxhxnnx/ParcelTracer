using Microsoft.EntityFrameworkCore;
using PaPl.SKS.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaPl.SKS.DataAccess.Sql
{

    [ExcludeFromCodeCoverage]

    public class SQLDataContext : DbContext
    {


        //Set of Tables
        public DbSet<Parcel> Parcel { get; set; }
        public DbSet<Hop> Hop { get; set; }
        public DbSet<Webhook> Webhook { get; set; }


        public SQLDataContext(DbContextOptions<SQLDataContext> options) : base(options)
        {   
            
        }

        protected override void OnModelCreating(ModelBuilder build)
        {
            build.Entity<Hop>(db =>
            {
                db.HasDiscriminator(field => field.HopType);
            }
            );

            build.Entity<Truck>()
                .HasBaseType<Hop>();

            build.Entity<Warehouse>()
                .HasBaseType<Hop>();

            build.Entity<TransferWarehouse>()
                .HasBaseType<Hop>();
        }

        public SQLDataContext() { }
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.AddInterceptors(new[] { new LogInterceptor() });
            optionsBuilder.UseSqlServer("Initial Catalog=Sample;User=sa;Password=yourStrong(!)Password;Data Source=localhost;MultipleActiveResultSets=True"
                );
            // ,sopt => sopt.UseNetTopologySuite());
        }
       */

    }
}
