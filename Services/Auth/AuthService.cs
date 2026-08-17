using RestaurantReservation.Db.Repositories; 
using RestaurantReservation.Db.Entities;     

namespace RestaurantReservation.API.Services.Interfaces;

public class AuthService : IAuth
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    public AuthService(IEmployeeRepository employeeRepository,
        IJwtTokenGenerator  jwtTokenGenerator)
    {
        _employeeRepository = employeeRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string?> LoginAsync(string employeeName)
    {
        var employee = await _employeeRepository.GetByNameAsync(employeeName);
        if (employee is null || employee.Position != "Manager")
        {
            return null;
        }
        return _jwtTokenGenerator.GenerateToken(employee);        
    }
}