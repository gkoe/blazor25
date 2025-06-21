using Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Entities
{
    public class Product : EntityObject
    {
        [Required]
        public string ProductNr { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty; 
        public double Price { get; set; }
        public List<Category> Categories { get; set; } = [];
    }
}
