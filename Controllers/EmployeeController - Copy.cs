using LoginAPI.Models;
using LoginAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LoginAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EmployeesController2 : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController2(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("GetAllEmployee")]
        public async Task<IActionResult> GetAllEmployee()
        {
            try
            {
                var response = await _employeeService.GetAllEmployee();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse.ReturnErrorResponse(ex.Message);
            }
        }

        [HttpGet("GetEmployeeByEmployee/{EmployeeNumber}")]
        public async Task<IActionResult> GetEmployeeByEmployeeNumber(int EmployeeNumber)
        {
            try
            {
                var response = await _employeeService.GetEmployeeByEmployeeNumber(EmployeeNumber);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse.ReturnErrorResponse(ex.Message);
            }
        }


        [HttpPost("AddEmployee")]
        public async Task<IActionResult> AddEmployee([FromBody] EmployeeDTO employeeInfo)
        {
            try
            {
                var response = await _employeeService.AddEmployee(employeeInfo);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse.ReturnErrorResponse(ex.Message);
            }

        }

        [HttpPut("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDTO employeeInfo)
        {
            try
            {
                var response = await _employeeService.UpdateEmployee(employeeInfo);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return ErrorResponse.ReturnErrorResponse(ex.Message);
            }

        }

        [HttpDelete("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee([FromBody] DeleteEmployeeDTO employeeInfo)
        {
            try
            {
                var response = await _employeeService.DeleteEmployee(employeeInfo);
                return Ok(response);

            }
            catch (Exception ex)
            {
                return ErrorResponse.ReturnErrorResponse(ex.Message);
            }

        }

    }
}
