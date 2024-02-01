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
        [ScaffoldColumn(false)]
        public int IdZamowienie { get; set; } //orderId
        public string Imie { get; set; }
        public string Nazwisko { get; set; }

        [ScaffoldColumn(false)]
        public string Email { get; set; }
        [ScaffoldColumn(false)]
        public int UzytkownikId { get; set; }
        public virtual Uzytkownik? uzytkownik { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        [ScaffoldColumn(false)]
        public decimal Suma { get; set; } //total
        [ScaffoldColumn(false)]
        public DateTime OrderDate { get; set; }
        public List<ZamowienieSzczegoly> ZamowienieSzczegolu { get; set; }
    }
}
