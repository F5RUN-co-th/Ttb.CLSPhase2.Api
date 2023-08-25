using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CLSPhase2.Dal.Entities;

public partial class ClsapiContext : DbContext
{
    public ClsapiContext(DbContextOptions<ClsapiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TempRequestBatch> TempRequestBatches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TempRequestBatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TempRequestBatch_pkey");

            entity.ToTable("TempRequestBatch", "cpss");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.JsonDocOutboundRequest).HasColumnType("jsonb");
            entity.Property(e => e.JsonDocOutboundResponse).HasColumnType("jsonb");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
