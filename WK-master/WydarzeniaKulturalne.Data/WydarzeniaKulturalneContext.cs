using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WydarzeniaKulturalne.Data.Entities;

namespace WydarzeniaKulturalne.Data
{
    public class WydarzeniaKulturalneContext : DbContext
    {
        public WydarzeniaKulturalneContext(DbContextOptions<WydarzeniaKulturalneContext> options)
            : base(options)
        {
        }
     
        public DbSet<Uzytkownik> Uzytkownik { get; set; } = default!;
        public DbSet<Rola> Rola { get; set; } = default!;
        public DbSet<SpecjalnyTag> SpecjalnyTag { get; set; } = default!;
        public DbSet<WydarzenieKulturalne> WydarzenieKulturalne { get; set; } = default!;

        public DbSet<LokalizacjaWydarzenia> LokalizacjaWydarzenia { get; set; } = default!;

        public DbSet<KategoriaWydarzenia> KategoriaWydarzenia { get; set; } = default!;
        public DbSet<Bilety> Bilety { get; set; } = default!;
        public DbSet<Zamowienie> Zamowienie { get; set; } = default!;
        public DbSet<ZamowienieSzczegoly> ZamowienieSzczegoly { get; set; } = default!;
        public DbSet<ElementKoszyka> ElementKoszyka { get; set; } = default!;

    }
}