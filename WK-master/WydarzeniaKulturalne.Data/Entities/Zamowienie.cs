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
    public partial class Zamowienie //order
    {
        [Key]
        public int IdZamowienie { get; set; } //orderId
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public string Email { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Suma { get; set; } //total
        public DateTime OrderDate { get; set; }
        public List<ZamowienieSzczegoly> ZamowienieSzczegolu { get; set; }
    }
}
