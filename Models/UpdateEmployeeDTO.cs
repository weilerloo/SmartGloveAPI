using Microsoft.AspNetCore.Mvc;

namespace LoginAPI.Models
{
    public class UpdateEmployeeDTO
    {
        public int EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string GivenName { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? Role { get; set; }
    }
}

