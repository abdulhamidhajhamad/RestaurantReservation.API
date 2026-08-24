using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Memory;
using RestaurantReservation.Db.Entities;
using RestaurantReservation.Db.Repositories;

namespace RestaurantReservation.API.Services.Auth;

public class AuthService : IAuth
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRevokedTokenRepository _revokedTokenRepository; 
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMemoryCache _cache;                             

    public AuthService(
        IEmployeeRepository employeeRepository,
        IRevokedTokenRepository revokedTokenRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IMemoryCache cache)
    {
        _employeeRepository = employeeRepository;
        _revokedTokenRepository = revokedTokenRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _cache = cache;
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

    public async Task<bool> LogoutAsync(string? jti, string? expString)
    {
        if (string.IsNullOrEmpty(jti) || !long.TryParse(expString, out var exp))
        {
            return false;
        }

        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

        await _revokedTokenRepository.AddAsync(new RevokedToken
        {
            Jti = jti,
            ExpiryDate = expiryDate
        });

        var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(expiryDate);
        _cache.Set($"revoked_{jti}", true, cacheOptions);

        return true;
    }

    public async Task<bool> LogoutAllDevicesAsync(string? userIdString)
    {
        if (!int.TryParse(userIdString, out var userId))
        {
            return false;
        }

        var employee = await _employeeRepository.GetEmployeeById(userId);
        if (employee is null)
        {
            return false;
        }

        employee.TokensValidFrom = DateTime.UtcNow;

        await _employeeRepository.UpdateEmployee(employee);

        return true;
    }
}