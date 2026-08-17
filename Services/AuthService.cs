using RestaurantReservation.Db.Repositories; 
using RestaurantReservation.Db.Entities;     

namespace RestaurantReservation.API.Services;

public class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository;
    public AuthService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
        
    }

    public async Task<string?> LoginAsync(string employeeName)
    {
        var employee = await _employeeRepository.GetByNameAsync(employeeName);
        if (employee is null || employee.Position != "Manager")
        {
            return null;
        }
        return "SUCCESS_MANAGER_FOUND";
        
    }
}