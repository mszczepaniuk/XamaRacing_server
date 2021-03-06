﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20191219144753_RaceToRaceMap")]
    partial class RaceToRaceMap
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Infrastructure.Data.Entities.RaceCheckpoint", b =>
                {
                    b.Property<int>("RaceId")
                        .HasColumnType("int");

                    b.Property<int>("NumberInOrder")
                        .HasColumnType("int");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.HasKey("RaceId", "NumberInOrder");

                    b.ToTable("RaceCheckpoints");
                });

            modelBuilder.Entity("Infrastructure.Data.Entities.RaceMap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatorId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RaceMaps");
                });

            modelBuilder.Entity("Infrastructure.Data.Entities.RaceResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("RaceId")
                        .HasColumnType("int");

                    b.Property<double>("TimeInSeconds")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RaceId");

                    b.ToTable("RaceResults");
                });

            modelBuilder.Entity("Infrastructure.Data.Entities.RaceCheckpoint", b =>
                {
                    b.HasOne("Infrastructure.Data.Entities.RaceMap", "Race")
                        .WithMany("RaceCheckpoints")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.Data.Entities.RaceResult", b =>
                {
                    b.HasOne("Infrastructure.Data.Entities.RaceMap", "Race")
                        .WithMany("RaceResults")
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
