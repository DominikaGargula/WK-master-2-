﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WydarzeniaKulturalne.Data;

#nullable disable

namespace WydarzeniaKulturalne.Data.Migrations
{
    [DbContext(typeof(WydarzeniaKulturalneContext))]
    [Migration("20240201190314_dodaniePolaUzytkownika")]
    partial class dodaniePolaUzytkownika
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Bilety", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("CzyDostepne")
                        .HasColumnType("bit");

                    b.Property<DateTime>("DataWydarzenia")
                        .HasColumnType("datetime2");

                    b.Property<int>("IloscBiletow")
                        .HasColumnType("int");

                    b.Property<int>("LokalizacjaWydarzeniaId")
                        .HasColumnType("int");

                    b.Property<int>("WydarzenieKulturalneId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LokalizacjaWydarzeniaId");

                    b.HasIndex("WydarzenieKulturalneId");

                    b.ToTable("Bilety");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.ElementKoszyka", b =>
                {
                    b.Property<int>("IdElementuKoszyka")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdElementuKoszyka"));

                    b.Property<int?>("BiletyId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DataUtworzenia")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdBilet")
                        .HasColumnType("int");

                    b.Property<string>("IdSesjiKoszyka")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Ilosc")
                        .HasColumnType("int");

                    b.HasKey("IdElementuKoszyka");

                    b.HasIndex("BiletyId");

                    b.ToTable("ElementKoszyka");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.KategoriaWydarzenia", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("id"));

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Opis")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("id");

                    b.ToTable("KategoriaWydarzenia");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.LokalizacjaWydarzenia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("KodPocztowy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Miejscowosc")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NazwaMiejsca")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NumerDomu")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<string>("Ulica")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("LokalizacjaWydarzenia");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Rola", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Aktywna")
                        .HasColumnType("bit");

                    b.Property<string>("Nazwa")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Rola");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.SpecjalnyTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nazwa")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SpecjalnyTag");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Uzytkownik", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Haslo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Imie")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nazwisko")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<int?>("RolaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RolaId");

                    b.ToTable("Uzytkownik");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.WydarzenieKulturalne", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Cena")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("DataUwtorzenia")
                        .HasColumnType("datetime2");

                    b.Property<int>("KategoriaWydarzeniaId")
                        .HasColumnType("int");

                    b.Property<int?>("LokalizacjaWydarzeniaId")
                        .HasColumnType("int");

                    b.Property<string>("Nazwa")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.Property<string>("Opis")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Promowane")
                        .HasColumnType("bit");

                    b.Property<int?>("SpecjalnyTagId")
                        .HasColumnType("int");

                    b.Property<string>("ZdjecieUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("KategoriaWydarzeniaId");

                    b.HasIndex("LokalizacjaWydarzeniaId");

                    b.HasIndex("SpecjalnyTagId");

                    b.ToTable("WydarzenieKulturalne");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Zamowienie", b =>
                {
                    b.Property<int>("IdZamowienie")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdZamowienie"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Imie")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nazwisko")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Suma")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("UzytkownikId")
                        .HasColumnType("int");

                    b.HasKey("IdZamowienie");

                    b.HasIndex("UzytkownikId");

                    b.ToTable("Zamowienie");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.ZamowienieSzczegoly", b =>
                {
                    b.Property<int>("IdZamowienieSzczegoly")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdZamowienieSzczegoly"));

                    b.Property<int>("BiletyId")
                        .HasColumnType("int");

                    b.Property<decimal>("Cena")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<int>("IdBilet")
                        .HasColumnType("int");

                    b.Property<int>("IdZamowienie")
                        .HasColumnType("int");

                    b.Property<int>("Ilosc")
                        .HasColumnType("int");

                    b.Property<int>("ZamowienieIdZamowienie")
                        .HasColumnType("int");

                    b.HasKey("IdZamowienieSzczegoly");

                    b.HasIndex("BiletyId");

                    b.HasIndex("ZamowienieIdZamowienie");

                    b.ToTable("ZamowienieSzczegoly");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Bilety", b =>
                {
                    b.HasOne("WydarzeniaKulturalne.Data.Entities.LokalizacjaWydarzenia", "Lokalizacja")
                        .WithMany("Bilety")
                        .HasForeignKey("LokalizacjaWydarzeniaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WydarzeniaKulturalne.Data.Entities.WydarzenieKulturalne", "Wydarzenie")
                        .WithMany("Bilety")
                        .HasForeignKey("WydarzenieKulturalneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lokalizacja");

                    b.Navigation("Wydarzenie");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.ElementKoszyka", b =>
                {
                    b.HasOne("WydarzeniaKulturalne.Data.Entities.Bilety", "Bilety")
                        .WithMany()
                        .HasForeignKey("BiletyId");

                    b.Navigation("Bilety");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Uzytkownik", b =>
                {
                    b.HasOne("WydarzeniaKulturalne.Data.Entities.Rola", "Rola")
                        .WithMany()
                        .HasForeignKey("RolaId");

                    b.Navigation("Rola");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.WydarzenieKulturalne", b =>
                {
                    b.HasOne("WydarzeniaKulturalne.Data.Entities.KategoriaWydarzenia", "KategoriaWydarzenia")
                        .WithMany("WydarzenieKulturalne")
                        .HasForeignKey("KategoriaWydarzeniaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WydarzeniaKulturalne.Data.Entities.LokalizacjaWydarzenia", null)
                        .WithMany("WydarzenieKulturalne")
                        .HasForeignKey("LokalizacjaWydarzeniaId");

                    b.HasOne("WydarzeniaKulturalne.Data.Entities.SpecjalnyTag", "SpecjalnyTag")
                        .WithMany()
                        .HasForeignKey("SpecjalnyTagId");

                    b.Navigation("KategoriaWydarzenia");

                    b.Navigation("SpecjalnyTag");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Zamowienie", b =>
                {
                    b.HasOne("WydarzeniaKulturalne.Data.Entities.Uzytkownik", "uzytkownik")
                        .WithMany()
                        .HasForeignKey("UzytkownikId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("uzytkownik");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.ZamowienieSzczegoly", b =>
                {
                    b.HasOne("WydarzeniaKulturalne.Data.Entities.Bilety", "Bilety")
                        .WithMany()
                        .HasForeignKey("BiletyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WydarzeniaKulturalne.Data.Entities.Zamowienie", "Zamowienie")
                        .WithMany("ZamowienieSzczegolu")
                        .HasForeignKey("ZamowienieIdZamowienie")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bilety");

                    b.Navigation("Zamowienie");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.KategoriaWydarzenia", b =>
                {
                    b.Navigation("WydarzenieKulturalne");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.LokalizacjaWydarzenia", b =>
                {
                    b.Navigation("Bilety");

                    b.Navigation("WydarzenieKulturalne");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.WydarzenieKulturalne", b =>
                {
                    b.Navigation("Bilety");
                });

            modelBuilder.Entity("WydarzeniaKulturalne.Data.Entities.Zamowienie", b =>
                {
                    b.Navigation("ZamowienieSzczegolu");
                });
#pragma warning restore 612, 618
        }
    }
}
