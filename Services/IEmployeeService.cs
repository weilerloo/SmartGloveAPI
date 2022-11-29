using LoginAPI.Models;

namespace LoginAPI.Services
{
    public interface IEmployeeService
    {
        Task<MainResponse> AddEmployee(EmployeeDTO studentDTO);
        Task<MainResponse> UpdateEmployee(UpdateEmployeeDTO studentDTO);
        Task<MainResponse> DeleteEmployee(DeleteEmployeeDTO studentDTO);
        Task<MainResponse> GetAllEmployee();
        Task<MainResponse> GetEmployeeByEmployeeNumber(int studentID);
    }
}
