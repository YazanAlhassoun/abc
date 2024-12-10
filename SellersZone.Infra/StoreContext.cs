using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SellersZone.Core.Models;
using SellersZone.Core.Models.Identity;

namespace SellersZone.Infra
{
    public class StoreContext : IdentityDbContext<AppUser>
    {
        public StoreContext()
        {
        }
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<AboutUs> AboutUses { get; set; }
        public DbSet<Carousel> Carousels { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<ProductsSetion> ProductsSetions { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<ProductsOrder> ProductsOrders { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<PurchaseProduct> PurchaseProducts { get; set; }
        public DbSet<SideEarning> SideEarnings { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<UserPaymentMethod> UserPaymentMethods { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // for product section table
            modelBuilder.Entity<ProductsSetion>()
            .HasKey(ps => new { ps.ProductId, ps.SectionId });

            modelBuilder.Entity<ProductsSetion>()
                .HasOne(ps => ps.Product)
                .WithMany(p => p.ProductsSection)
                .HasForeignKey(ps => ps.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductsSetion>()
                .HasOne(ps => ps.Section)
                .WithMany(s => s.ProductsSetion)
                .HasForeignKey(ps => ps.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
                   .HasOne(u => u.Wallet)
                   .WithMany(w => w.AppUsers)
                   .HasForeignKey(u => u.WalletId);

            // for make all tables with ({ Restrict Behavior }) on add migrations
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Iterate through all foreign key properties
                foreach (var foreignKey in entityType.GetForeignKeys())
                {
                    // Set delete behavior to Restrict
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.DetectChanges();
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
