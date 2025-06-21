using Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Entities
{
    public class Order : EntityObject
    {
        [Required]
        public string OrderNr { get; set; } = string.Empty; 
        public DateTime Date { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = new Customer();
        public int CustomerId { get; set; }
        public OrderType OrderType { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = [];

    }

    
}
