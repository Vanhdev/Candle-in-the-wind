using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace CandleInTheWind.Models
{
    public partial class CandleInTheWindContext : DbContext
    {
        public CandleInTheWindContext()
        {
        }

        public CandleInTheWindContext(DbContextOptions<CandleInTheWindContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-5J8LSQ0\\MSSQLSERER;Database=CandleInTheWind;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.ProductId })
                    .HasName("PK_GIOHANG");

                entity.ToTable("CARTS");

                entity.Property(e => e.CustomerId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.ProductId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("PRODUCT_ID");

                entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GIOHANG_KHACHHANG");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Carts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GIOHANG_SANPHAM");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("CATEGORIES");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NAME");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("COMMENTS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("ntext")
                    .HasColumnName("_CONTENT");

                entity.Property(e => e.CustomerId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.PostId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("POST_ID");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("TIME");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BINHLUAN_KHACHHANG");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BINHLUAN_BAIVIET");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("CUSTOMERS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("DATE_OF_BIRTH");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("EMAIL");

                entity.Property(e => e.Gender).HasColumnName("GENDER");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("NAME");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("PASSWORD");

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PHONE_NUMBER");

                entity.Property(e => e.Points).HasColumnName("POINTS");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.CustomerId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.ProductId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("PRODUCT_ID");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("date")
                    .HasColumnName("PURCHASE_DATE");

                entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.Property(e => e.Total)
                    .HasColumnType("money")
                    .HasColumnName("TOTAL");

                entity.Property(e => e.UnitPrice)
                    .HasColumnType("money")
                    .HasColumnName("UNIT_PRICE");

                entity.Property(e => e.VoucherId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("VOUCHER_ID");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DONHANG_KHACHHANG");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DONHANG_SANPHAM");

                entity.HasOne(d => d.Voucher)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.VoucherId)
                    .HasConstraintName("FK_DONHANG_MAGIAMGIA");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("POSTS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.ApprovedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("APPROVED_AT");

                entity.Property(e => e.Commentable)
                    .IsRequired()
                    .HasColumnName("COMMENTABLE")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("ntext")
                    .HasColumnName("_CONTENT");

                entity.Property(e => e.CustomerId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("CUSTOMER_ID");

                entity.Property(e => e.Status).HasColumnName("STATUS");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("TITLE");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BAIVIET_KHACHHANG");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCTS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.CategoryId)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("CATEGORY_ID");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("ntext")
                    .HasColumnName("DESCRIPTION");

                entity.Property(e => e.ImageUrl).HasColumnName("IMAGE_URL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("NAME");

                entity.Property(e => e.Price)
                    .HasColumnType("money")
                    .HasColumnName("PRICE");

                entity.Property(e => e.Stock).HasColumnName("STOCK");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SANPHAM_HANGMUC");
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.ToTable("VOUCHERS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Created)
                    .HasColumnType("date")
                    .HasColumnName("CREATED");

                entity.Property(e => e.Expired)
                    .HasColumnType("date")
                    .HasColumnName("EXPIRED");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("NAME");

                entity.Property(e => e.Quantity).HasColumnName("QUANTITY");

                entity.Property(e => e.Value).HasColumnName("VALUE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
