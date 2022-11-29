using System.ComponentModel.DataAnnotations;

namespace LoginAPI.Models
{
    public class RegisterUserDTO
    {
        public string GivenName { get; set; } // Department
        public string Surname { get; set; } // Employee Number
        public string DepartmentCode { get; set; }
        public string EmployeeNumber { get; set; }
        public string Password { get; set; } 
        public string Gender { get; set; }
        public string EmployeeCodeNumber
        {
            get => $"{DepartmentCode}{EmployeeNumber}";
        }

    }
}