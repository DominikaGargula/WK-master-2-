using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema; 

namespace WydarzeniaKulturalne.Data.Entities
{
    public class SpecjalnyTag
    {
        [Key]
        public int Id { get; set; }
        public string? Nazwa { get; set; }
    }
}
