using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuestionExample.Data;

public partial class SalesRepContext : DbContext
{
    public SalesRepContext()
    {
    }

    public SalesRepContext(DbContextOptions<SalesRepContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Factory> Factories { get; set; }

    public virtual DbSet<ForecastOrder> ForecastOrders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandNo);

            entity.ToTable("BRAND", tb => tb.HasComment("品牌"));

            entity.Property(e => e.BrandNo)
                .HasMaxLength(24)
                .HasComment("品牌編號")
                .HasColumnName("BRAND_NO");
            entity.Property(e => e.BrandDesc)
                .HasMaxLength(48)
                .HasComment("品牌說明")
                .HasColumnName("BRAND_DESC");
            entity.Property(e => e.Division)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("division");
            entity.Property(e => e.TeamLeader)
                .HasMaxLength(32)
                .HasComment("品牌負責人")
                .HasColumnName("TEAM_LEADER");
        });

        modelBuilder.Entity<Factory>(entity =>
        {
            entity.HasKey(e => e.FactNo);

            entity.ToTable("FACTORY", tb => tb.HasComment("工廠/公司"));

            entity.Property(e => e.FactNo)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasComment("工廠/公司編號")
                .HasColumnName("FACT_NO");
            entity.Property(e => e.Country)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasComment("國別(TW、CN、VN)")
                .HasColumnName("COUNTRY");
            entity.Property(e => e.FactName)
                .HasMaxLength(80)
                .HasComment("工廠/公司名稱")
                .HasColumnName("FACT_NAME");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("UPDATE_DATE");
        });

        modelBuilder.Entity<ForecastOrder>(entity =>
        {
            entity.HasKey(e => new { e.Ym, e.BrandNo });

            entity.ToTable("FORECAST_ORDER", tb => tb.HasComment("預告訂單"));

            entity.Property(e => e.Ym)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasComment("訂單年月")
                .HasColumnName("YM");
            entity.Property(e => e.BrandNo)
                .HasMaxLength(24)
                .HasComment("品牌編號")
                .HasColumnName("BRAND_NO");
            entity.Property(e => e.Price)
                .HasComment("金額(USD)")
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("PRICE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
