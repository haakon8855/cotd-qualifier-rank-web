﻿// <auto-generated />
using System;
using CotdQualifierRank.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CotdQualifierRank.Database.Migrations
{
    [DbContext(typeof(CotdContext))]
    [Migration("20241104131857_NonNullableMapUid")]
    partial class NonNullableMapUid
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CotdQualifierRank.Database.Entities.CompetitionEntity", b =>
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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Competitions");
                });

            modelBuilder.Entity("CotdQualifierRank.Database.Entities.NadeoCompetitionEntity", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

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

            modelBuilder.Entity("CotdQualifierRank.Database.Entities.RecordEntity", b =>
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

            modelBuilder.Entity("CotdQualifierRank.Database.Entities.RecordEntity", b =>
                {
                    b.HasOne("CotdQualifierRank.Database.Entities.CompetitionEntity", null)
                        .WithMany("Leaderboard")
                        .HasForeignKey("CompetitionId");
                });

            modelBuilder.Entity("CotdQualifierRank.Database.Entities.CompetitionEntity", b =>
                {
                    b.Navigation("Leaderboard");
                });
#pragma warning restore 612, 618
        }
    }
}
