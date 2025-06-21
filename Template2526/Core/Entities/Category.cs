using Base.Entities;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Category: EntityObject
    {
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        public List<Product> Products { get; set; } = [];
    }
}
