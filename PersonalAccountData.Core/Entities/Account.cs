using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalAccountData.Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } //на случай того что номер будет 16-ричным или содержать в себе буквы по требованиям
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Address { get; set; }
        public double Area { get; set; }
        public List<Resident> Residents { get; set; } = new List<Resident>();
    }
}
