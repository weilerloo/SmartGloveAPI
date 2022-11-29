using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LoginAPI.Data
{
    public class Users : IdentityUser
    {
        [MaxLength(50)]
        public string GivenName { get; set; } = null!;

        [MaxLength(50)]
        public string Surname { get; set; } = null!;

        [MaxLength(50)]
        public string EmployeeName
        {
            get => $"{GivenName} {Surname}";
        }        
        //public string EmployeeCodeNumber
        //{
        //    get => $"{DepartmentCode} {EmployeeNumber}".Trim();
        //}

        public string EmployeeCodeNumber
        {
            get
            {
                return DepartmentCode + EmployeeNumber;
            }
        }


        [MaxLength(50)]
        public string DepartmentCode { get; set; } = null!;
        public string EmployeeNumber { get; set; } = null!;
        public string? Gender { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role { get; set; } = null!;
        public int? RoleID { get; set; }
    }
}