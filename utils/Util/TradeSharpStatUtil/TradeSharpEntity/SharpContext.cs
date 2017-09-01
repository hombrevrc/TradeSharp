using System.Data.Common;
using System.Data.Entity;

namespace TradeSharpEntity
{
    public class SharpContext : DbContext
    {
        public DbSet<Position> Position { get; set; }

        public DbSet<PositionClosed> PositionClosed { get; set; }

        public SharpContext()
        {
        }

        public SharpContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Position>().ToTable("POSITION");
            modelBuilder.Entity<Position>().HasKey(x => x.ID);
            modelBuilder.Entity<PositionClosed>().HasKey(x => x.ID);

            modelBuilder.Entity<Position>().Property(x => x.PriceEnter).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.PriceBest).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.PriceWorst).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.Stoploss).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.Takeprofit).HasPrecision(8, 5);

            modelBuilder.Entity<Position>().Property(x => x.TrailLevel1).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailLevel2).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailLevel3).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailLevel4).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailTarget1).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailTarget2).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailTarget3).HasPrecision(8, 5);
            modelBuilder.Entity<Position>().Property(x => x.TrailTarget4).HasPrecision(8, 5);

            modelBuilder.Entity<PositionClosed>().Property(x => x.PriceEnter).HasPrecision(8, 5);
            modelBuilder.Entity<PositionClosed>().Property(x => x.PriceBest).HasPrecision(8, 5);
            modelBuilder.Entity<PositionClosed>().Property(x => x.PriceWorst).HasPrecision(8, 5);
            modelBuilder.Entity<PositionClosed>().Property(x => x.PriceExit).HasPrecision(8, 5);
            modelBuilder.Entity<PositionClosed>().Property(x => x.ResultBase).HasPrecision(16, 2);
            modelBuilder.Entity<PositionClosed>().Property(x => x.ResultDepo).HasPrecision(16, 2);
            modelBuilder.Entity<PositionClosed>().Property(x => x.ResultPoints).HasPrecision(8, 2);
        }
    }
}