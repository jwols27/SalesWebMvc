﻿using System;
using SalesWebMvc.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SalesWebMvc.Models {
    public class SalesRecord {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [DataType(DataType.Currency)]
        public double Amount { get; set; }
        
        public SaleStatus Status { get; set; }
        public Seller Seller { get; set; }

        [Display(Name = "Seller")]
        public int SellerId { get; set; }

        public SalesRecord() { }

        public SalesRecord(int id, DateTime date, double amount, SaleStatus status, Seller seller) {
            Id = id;
            Date = date;
            Amount = amount;
            Status = status;
            Seller = seller;
        }
    }
}
