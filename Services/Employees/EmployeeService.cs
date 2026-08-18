using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<Employee>> GetManagersAsync()
    {
        return await _employeeRepository.ListManagers();
    }

    public async Task<decimal> GetAverageOrderAmountAsync(int employeeId)
    {
        return await _employeeRepository.CalculateAverageOrderAmountAsync(employeeId);
    }
}