using System.ComponentModel.DataAnnotations;

namespace LoginAPI.Data
{
    public class Employee
    {
        [Key]
        public int EmployeeNumber { get; set; }
        [MaxLength(100)]
        public string EmployeeName { get; set; } = null!;
        [MaxLength(100)]
        public string Department { get; set; } = null!;
        [MaxLength(100)]
        public string GivenName { get; set; } = null!;
        [MaxLength(100)]
        public string Surname { get; set; } = null!;
        [MaxLength(100)]
        public string? Role { get; set; } = null!;

    }
}
