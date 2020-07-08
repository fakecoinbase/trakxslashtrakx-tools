﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trakx.Persistence;

namespace Trakx.Persistence.Migrations
{
    [DbContext(typeof(IndiceRepositoryContext))]
    partial class IndiceRepositoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Trakx.Persistence.DAO.ComponentDefinitionDao", b =>
                {
                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("CoinGeckoId")
                        .IsRequired()
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

            modelBuilder.Entity("Trakx.Persistence.DAO.ComponentQuantityDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ComponentDefinitionDaoAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("IndiceCompositionDaoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(38, 18)");

                    b.HasKey("Id");

                    b.HasIndex("ComponentDefinitionDaoAddress");

                    b.HasIndex("IndiceCompositionDaoId");

                    b.ToTable("ComponentQuantities");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.ComponentValuationDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ComponentQuantityDaoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IndiceValuationDaoId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<string>("PriceSource")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

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

                    b.HasIndex("IndiceValuationDaoId");

                    b.ToTable("ComponentValuations");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.DepositorAddressDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrencySymbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserDaoId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal?>("VerificationAmount")
                        .HasColumnType("decimal(38, 18)");

                    b.HasKey("Id");

                    b.HasIndex("UserDaoId");

                    b.ToTable("DepositorAddresses");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceCompositionDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("IndiceDefinitionDaoSymbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Version")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("IndiceDefinitionDaoSymbol");

                    b.ToTable("IndiceCompositions");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceDefinitionDao", b =>
                {
                    b.Property<string>("Symbol")
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<string>("Address")
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

                    b.ToTable("IndiceDefinitions");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceSupplyTransactionDao", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreationTimestamp")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EthereumBlockId")
                        .HasColumnType("int");

                    b.Property<string>("IndiceCompositionDaoId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Quantity")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<string>("SenderAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("TransactionHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("IndiceCompositionDaoId");

                    b.ToTable("IndiceSupplyTransactions");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceValuationDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("IndiceCompositionDaoId")
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

                    b.HasIndex("IndiceCompositionDaoId");

                    b.ToTable("IndiceValuations");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.UserDao", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.WrappingTransactionDao", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(38, 18)");

                    b.Property<int?>("EthereumBlockId")
                        .HasColumnType("int");

                    b.Property<string>("EthereumTransactionHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FromCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int?>("NativeChainBlockId")
                        .HasColumnType("int");

                    b.Property<string>("NativeChainTransactionHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReceiverAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<string>("SenderAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(256)")
                        .HasMaxLength(256);

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("ToCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("TransactionState")
                        .HasColumnType("int");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.Property<string>("User")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WrappingTransactions");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.ComponentQuantityDao", b =>
                {
                    b.HasOne("Trakx.Persistence.DAO.ComponentDefinitionDao", "ComponentDefinitionDao")
                        .WithMany()
                        .HasForeignKey("ComponentDefinitionDaoAddress")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trakx.Persistence.DAO.IndiceCompositionDao", "IndiceCompositionDao")
                        .WithMany("ComponentQuantityDaos")
                        .HasForeignKey("IndiceCompositionDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.ComponentValuationDao", b =>
                {
                    b.HasOne("Trakx.Persistence.DAO.ComponentQuantityDao", "ComponentQuantityDao")
                        .WithMany()
                        .HasForeignKey("ComponentQuantityDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trakx.Persistence.DAO.IndiceValuationDao", null)
                        .WithMany("ComponentValuationDaos")
                        .HasForeignKey("IndiceValuationDaoId");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.DepositorAddressDao", b =>
                {
                    b.HasOne("Trakx.Persistence.DAO.UserDao", "UserDao")
                        .WithMany("AddressDaos")
                        .HasForeignKey("UserDaoId");
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceCompositionDao", b =>
                {
                    b.HasOne("Trakx.Persistence.DAO.IndiceDefinitionDao", "IndiceDefinitionDao")
                        .WithMany("IndiceCompositionDaos")
                        .HasForeignKey("IndiceDefinitionDaoSymbol")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceSupplyTransactionDao", b =>
                {
                    b.HasOne("Trakx.Persistence.DAO.IndiceCompositionDao", "IndiceCompositionDao")
                        .WithMany()
                        .HasForeignKey("IndiceCompositionDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trakx.Persistence.DAO.IndiceValuationDao", b =>
                {
                    b.HasOne("Trakx.Persistence.DAO.IndiceCompositionDao", "IndiceCompositionDao")
                        .WithMany("IndiceValuationDaos")
                        .HasForeignKey("IndiceCompositionDaoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
