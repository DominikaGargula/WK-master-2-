using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WydarzeniaKulturalne.Data.Entities
{
    public class Rola
    {
        [Key]

        public int Id { get; set; }
        public string? Nazwa { get; set; }
        public bool Aktywna { get; set; }
    }
}
