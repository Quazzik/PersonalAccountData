using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PersonalAccountData.API.DTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "The account number must consist of 10 digits")]
        public string AccountNumber { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public string Address { get; set; }
        [Range(0.1, double.MaxValue)]
        public double Area { get; set; }
        public List<ResidentDto> Residents { get; set; } = new();
    }

    public class AccountFilterDTO
    {
        public string Search {  get; set; }
        public bool? HasResidents { get; set; }
        public DateTime? ActiveDate { get; set; }
        public string AccountNumber { get; set; }
        public string Address { get; set; }
        public string ResidentName { get; set; }
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
    }
}
