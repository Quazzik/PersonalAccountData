using System;
using System.Collections.Generic;

namespace PersonalAccountData.API.DTOs
{
    public class AccountDTO
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Address { get; set; }
        public double Area { get; set; }
        public List<ResidentDto> Residents { get; set; } = new();
    }
}
