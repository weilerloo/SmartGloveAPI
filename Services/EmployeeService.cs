using Microsoft.Extensions.Hosting;
using LoginAPI.Models;
using LoginAPI.Services;
using System;
using LoginAPI.Context;
using LoginAPI.Data;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace LoginAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly EmployeeDbContext _context;
        public EmployeeService(EmployeeDbContext context)
        {
            _context = context;
        }

        public async Task<MainResponse> AddEmployee(EmployeeDTO EmployeeDTO)
        {
            var response = new MainResponse();
            try
            {
                if (_context.Employees.Any(f => f.EmployeeName.ToLower() == EmployeeDTO.EmployeeName.ToLower()))
                {
                    response.ErrorMessage = "Employee is already exist with this Name";
                    response.IsSuccess = false;
                }
                else
                {
                    await _context.AddAsync(new Employee
                    {
                        EmployeeName = EmployeeDTO.EmployeeName,
                        GivenName = EmployeeDTO.GivenName,
                        Surname = EmployeeDTO.Surname,
                        Department = EmployeeDTO.Department,
                    });
                    await _context.SaveChangesAsync();

                    response.IsSuccess = true;
                    response.Content = "Employee Added";
                }


            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        public async Task<MainResponse> DeleteEmployee(DeleteEmployeeDTO EmployeeDTO)
        {
            var response = new MainResponse();
            try
            {
                if (EmployeeDTO.EmployeeNumber < 0)
                {
                    response.ErrorMessage = "Please pass Employee Number";
                    return response;
                }

                var existingEmployee = _context.Employees.Where(f => f.EmployeeNumber == EmployeeDTO.EmployeeNumber).FirstOrDefault();

                if (existingEmployee != null)
                {
                    _context.Remove(existingEmployee);
                    await _context.SaveChangesAsync();

                    response.IsSuccess = true;
                    response.Content = "Employee Info Deleted";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Content = "Employee not found with specify Employee Numner";
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        public async Task<MainResponse> GetAllEmployee()
        {
            var response = new MainResponse();
            try
            {
                response.Content = await _context.Employees.ToListAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<MainResponse> GetEmployeeByEmployeeNumber(int EmpNumber)
        {
            var response = new MainResponse();
            try
            {
                response.Content =
                    await _context.Employees.Where(f => f.EmployeeNumber == EmpNumber).FirstOrDefaultAsync();
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }
            return response;
        }

        public async Task<MainResponse> UpdateEmployee(UpdateEmployeeDTO EmployeeDTO)
        {
            var response = new MainResponse();
            try
            {
                if (EmployeeDTO.EmployeeNumber < 0)
                {
                    response.ErrorMessage = "Please enter Employee Number";
                    return response;
                }

                var existingEmployee = _context.Employees.Where(f => f.EmployeeNumber == EmployeeDTO.EmployeeNumber).FirstOrDefault();

                if (existingEmployee != null)
                {
                    existingEmployee.EmployeeName = EmployeeDTO.EmployeeName;
                    existingEmployee.EmployeeNumber = EmployeeDTO.EmployeeNumber;
                    existingEmployee.GivenName = EmployeeDTO.GivenName;
                    existingEmployee.Surname = EmployeeDTO.Surname;
                    existingEmployee.Role = EmployeeDTO.Role;
                    await _context.SaveChangesAsync();

                    response.IsSuccess = true;
                    response.Content = "Record Updated";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Content = "Employee not found with specify Employee ID";
                }

            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }
    }
}
