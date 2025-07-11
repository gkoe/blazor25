﻿using Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Entities
{
    public class OrderItem : EntityObject
    {
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = new Order();
        public int OrderId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = new Product();
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }
}
