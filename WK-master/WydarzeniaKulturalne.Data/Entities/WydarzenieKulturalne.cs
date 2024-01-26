using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities
{
    public class WydarzenieKulturalne
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa musi zawierać min 3 znaki, max 30")]
        [StringLength(30, MinimumLength = 3)]
        public string Nazwa { get; set; }
        // ? nie każde wydarzenie musi mieć swój opis

        public string? Opis { get; set; }

        [Display(Name = "Zdjęcie")]
        public string? ZdjecieUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "Cena jest wymagana")]
        public decimal Cena { get; set; }
        [Display(Name = "Data utworzenia")]
        public DateTime DataUwtorzenia { get; set; } = DateTime.UtcNow;

        public int KategoriaWydarzeniaId { get; set; }
        [Display(Name = "Kategoria")]
        public KategoriaWydarzenia? KategoriaWydarzenia { get; set; }

        public bool Promowane { get; set; }
        public SpecjalnyTag? SpecjalnyTag { get; set; }
        public int? SpecjalnyTagId { get; set; }
        public virtual ICollection<Bilety> Bilety { get; set; }
        //public int? LokalizacjaWydarzeniaId { get; set; } = null;
    }
}
