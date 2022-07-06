﻿// <auto-generated />
using System;
using CarpetPlanner6.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarpetPlanner6.Migrations
{
    [DbContext(typeof(CarpetDataContext))]
    partial class CarpetDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("CarpetPlanner6.Models.CarpetEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Removed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StripeSeparator")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.Property<int>("Width")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(100);

                    b.HasKey("Id");

                    b.ToTable("Carpets");
                });

            modelBuilder.Entity("CarpetPlanner6.Models.ColorEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ordinal")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Removed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Rgb")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("CarpetPlanner6.Models.StripeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CarpetEntityId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CarpetId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Color")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Height")
                        .HasColumnType("REAL");

                    b.Property<int>("Ordinal")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CarpetEntityId");

                    b.ToTable("Stripes");
                });

            modelBuilder.Entity("CarpetPlanner6.Models.StripeEntity", b =>
                {
                    b.HasOne("CarpetPlanner6.Models.CarpetEntity", null)
                        .WithMany("Stripes")
                        .HasForeignKey("CarpetEntityId");
                });

            modelBuilder.Entity("CarpetPlanner6.Models.CarpetEntity", b =>
                {
                    b.Navigation("Stripes");
                });
#pragma warning restore 612, 618
        }
    }
}
