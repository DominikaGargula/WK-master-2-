using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities
{
    public class ZamowienieSzczegoly //OrderDetail
    {
        [Key]
        public int IdZamowienieSzczegoly { get; set; } //OrderDetailId
        public int IdZamowienie { get; set; } //OrderDetail
        public int IdBilet { get; set; }
        public int Ilosc { get; set; } //Quantity

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cena { get; set; } //UnitPrice
        public virtual Bilety? Bilet { get; set; }
        public virtual Zamowienie? Zamowienie { get; set; }
    }
}
