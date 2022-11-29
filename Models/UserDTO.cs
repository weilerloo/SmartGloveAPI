using System.ComponentModel.DataAnnotations;

namespace LoginAPI.Models
{
    public class UserDTO
    {
        public string EmployeeNumber { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string EmployeeName { get; set; }
        public string Role { get; set; }
        public int? RoleID { get; set; }

    }

    public enum RoleDetails
    {
        Employee = 1,
        Supervisor,
        HR,
        HOD,
    }
}