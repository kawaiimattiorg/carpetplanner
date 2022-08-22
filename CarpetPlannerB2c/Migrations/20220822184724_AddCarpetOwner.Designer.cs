﻿// <auto-generated />
using System;
using CarpetPlannerB2c.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarpetPlannerB2c.Migrations
{
    [DbContext(typeof(CarpetDataContext))]
    [Migration("20220822184724_AddCarpetOwner")]
    partial class AddCarpetOwner
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.5");

            modelBuilder.Entity("CarpetPlannerB2c.Models.CarpetEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Removed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StripeSeparator")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Width")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(100);

                    b.HasKey("Id");

                    b.ToTable("Carpets");
                });

            modelBuilder.Entity("CarpetPlannerB2c.Models.ColorEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ordinal")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Removed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Rgb")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("CarpetPlannerB2c.Models.StripeEntity", b =>
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

            modelBuilder.Entity("CarpetPlannerB2c.Models.StripeEntity", b =>
                {
                    b.HasOne("CarpetPlannerB2c.Models.CarpetEntity", null)
                        .WithMany("Stripes")
                        .HasForeignKey("CarpetEntityId");
                });

            modelBuilder.Entity("CarpetPlannerB2c.Models.CarpetEntity", b =>
                {
                    b.Navigation("Stripes");
                });
#pragma warning restore 612, 618
        }
    }
}
