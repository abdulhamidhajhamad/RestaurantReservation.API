using RestaurantReservation.Db.Entities;

namespace RestaurantReservation.API.Services.Employees;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetManagersAsync();
    Task<decimal> GetAverageOrderAmountAsync(int employeeId);
}