﻿using Core.Model;
using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class ExpenseDTO
    {
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        public Frequency Frequency { get; set; }
        public string Notes { get; set; }
        public List<string> Tags { get; set; }
    }
}