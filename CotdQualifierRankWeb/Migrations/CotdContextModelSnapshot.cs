﻿// <auto-generated />
using System;
using CotdQualifierRankWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CotdQualifierRankWeb.Migrations
{
    [DbContext(typeof(CotdContext))]
    partial class CotdContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CotdQualifierRankWeb.Models.Competition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("NadeoChallengeId")
                        .HasColumnType("int");

                    b.Property<int>("NadeoCompetitionId")
                        .HasColumnType("int");

                    b.Property<string>("NadeoMapUid")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("CotdQualifierRankWeb.Models.NadeoCompetition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NbPlayers")
                        .HasColumnType("int");

                    b.Property<string>("liveId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("NadeoCompetitions");
                });

            modelBuilder.Entity("CotdQualifierRankWeb.Models.Record", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CompetitionId")
                        .HasColumnType("int");

                    b.Property<int>("Time")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CompetitionId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("CotdQualifierRankWeb.Models.Record", b =>
                {
                    b.HasOne("CotdQualifierRankWeb.Models.Competition", null)
                        .WithMany("Leaderboard")
                        .HasForeignKey("CompetitionId");
                });

            modelBuilder.Entity("CotdQualifierRankWeb.Models.Competition", b =>
                {
                    b.Navigation("Leaderboard");
                });
#pragma warning restore 612, 618
        }
    }
}
