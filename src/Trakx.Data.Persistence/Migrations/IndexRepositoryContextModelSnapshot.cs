﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trakx.Data.Persistence;

namespace Trakx.Data.Persistence.Migrations
{
    [DbContext(typeof(IndexRepositoryContext))]
    partial class IndexRepositoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentDefinitionDao", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<int>("Decimals")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.HasKey("Address");

                    b.ToTable("ComponentDefinitions");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentQuantityDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ComponentDefinitionDaoAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("IndexCompositionDaoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(38, 18)");

                    b.HasKey("Id");

                    b.HasIndex("ComponentDefinitionDaoAddress");

                    b.HasIndex("IndexCompositionDaoId");

                    b.ToTable("ComponentQuantities");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentValuationDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ComponentQuantityDaoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IndexValuationDaoId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<string>("QuoteCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<decimal?>("Weight")
                        .HasColumnType("decimal(10, 10)");

                    b.HasKey("Id");

                    b.HasIndex("ComponentQuantityDaoId");

                    b.HasIndex("IndexValuationDaoId");

                    b.ToTable("ComponentValuations");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentWeightDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ComponentDefinitionDaoAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("IndexDefinitionDaoSymbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(10, 10)");

                    b.HasKey("Id");

                    b.HasIndex("ComponentDefinitionDaoAddress");

                    b.HasIndex("IndexDefinitionDaoSymbol");

                    b.ToTable("ComponentWeights");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.IndexCompositionDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("IndexDefinitionDaoSymbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("IndexDefinitionDaoSymbol");

                    b.ToTable("IndexCompositions");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.IndexDefinitionDao", b =>
                {
                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(512)")
                        .HasMaxLength(512);

                    b.Property<int>("NaturalUnit")
                        .HasColumnType("int");

                    b.HasKey("Symbol");

                    b.ToTable("IndexDefinitions");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.IndexValuationDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IndexCompositionDaoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("NetAssetValue")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<string>("QuoteCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("IndexCompositionDaoId");

                    b.ToTable("IndexValuations");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentQuantityDao", b =>
                {
                    b.HasOne("Trakx.Data.Persistence.DAO.ComponentDefinitionDao", "ComponentDefinitionDao")
                        .WithMany()
                        .HasForeignKey("ComponentDefinitionDaoAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trakx.Data.Persistence.DAO.IndexCompositionDao", "IndexCompositionDao")
                        .WithMany("ComponentQuantityDaos")
                        .HasForeignKey("IndexCompositionDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentValuationDao", b =>
                {
                    b.HasOne("Trakx.Data.Persistence.DAO.ComponentQuantityDao", "ComponentQuantityDao")
                        .WithMany()
                        .HasForeignKey("ComponentQuantityDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trakx.Data.Persistence.DAO.IndexValuationDao", null)
                        .WithMany("ComponentValuationDaos")
                        .HasForeignKey("IndexValuationDaoId");
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.ComponentWeightDao", b =>
                {
                    b.HasOne("Trakx.Data.Persistence.DAO.ComponentDefinitionDao", "ComponentDefinitionDao")
                        .WithMany()
                        .HasForeignKey("ComponentDefinitionDaoAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trakx.Data.Persistence.DAO.IndexDefinitionDao", "IndexDefinitionDao")
                        .WithMany("ComponentWeightDaos")
                        .HasForeignKey("IndexDefinitionDaoSymbol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.IndexCompositionDao", b =>
                {
                    b.HasOne("Trakx.Data.Persistence.DAO.IndexDefinitionDao", "IndexDefinitionDao")
                        .WithMany("IndexCompositionDaos")
                        .HasForeignKey("IndexDefinitionDaoSymbol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trakx.Data.Persistence.DAO.IndexValuationDao", b =>
                {
                    b.HasOne("Trakx.Data.Persistence.DAO.IndexCompositionDao", "IndexCompositionDao")
                        .WithMany("IndexValuationDaos")
                        .HasForeignKey("IndexCompositionDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
