using System;

namespace PersonalAccountData.API.DTOs
{
    public class ResidentDto
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsMainResident { get; set; }
    }
}
