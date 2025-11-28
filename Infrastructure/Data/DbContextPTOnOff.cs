using ApplicationCore.Entities.PTOnOff.Auth;
using ApplicationCore.Entities.PTOnOff.Task;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class DbContextPTOnOff: DbContext
    {
        public DbContextPTOnOff() { }

        public DbContextPTOnOff(DbContextOptions<DbContextPTOnOff> options): base(options) { }

        public virtual DbSet<tblAuth> tblAuth { get; set; }
        public virtual DbSet<tblTask> tblTask { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblAuth>(entity =>
            {
                entity.ToTable("tblUser", "Auth");

                entity.HasKey(e => e.iIDUser);

                entity.Property(e => e.tUserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.tPasswordHash)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.dtDateTimeRegister)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasMany(u => u.tblTaskNavigation) 
                    .WithOne(t => t.tblAuthNavigation) 
                    .HasForeignKey(t => t.iIDUser) 
                    .OnDelete(DeleteBehavior.Cascade) 
                    .HasConstraintName("FK_Task_User");
            });

            modelBuilder.Entity<tblTask>(entity =>
            {
                entity.ToTable("tblTask", "Tasks");

                entity.HasKey(e => e.iIDTask);

                entity.Property(e => e.iIDTask).ValueGeneratedOnAdd();

                entity.Property(e => e.tTitle)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.bIsCompleted)
                    .HasColumnType("BIT")
                    .HasDefaultValue(false);

                entity.Property(e => e.dtDateTimeRegister)
                    .HasColumnType("DATETIME")
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(t => t.tblAuthNavigation)
                    .WithMany(u => u.tblTaskNavigation)
                    .HasForeignKey(t => t.iIDUser);

            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
