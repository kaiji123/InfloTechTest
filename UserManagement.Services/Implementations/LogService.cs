using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Data;
using UserManagement.Models;

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;
    private readonly IDataContext _dataAccess;
    private List<Logs>? logs; // Define the logs field for caching purpose

    public LogService(IDataContext dataAccess, ILogger<LogService> logger)
    {
        _dataAccess = dataAccess;
        _logger = logger;
        InitializeLogsAsync().Wait(); // Initialize the logs list asynchronously
    }

    // Initialize the logs list asynchronously
    private async Task InitializeLogsAsync()
    {
        logs = (await _dataAccess.GetAll<Logs>().ToListAsync()).ToList();
    }

    public void LogAction(long userId, string action, string source, string? payload, NoticeLevel noticeLevel = NoticeLevel.Info)
    {
        _logger.LogInformation("User {UserId} performed action: {Action}", userId, action);
        _dataAccess.CreateAsync(new Logs { UserId = userId, Action = action, Timestamp = DateTime.Now, Source = source, Payload = payload, NoticeLevel = noticeLevel });
    }

    public async Task<IEnumerable<Logs>> GetLogsForUserAsync(long userId)
    {
        if (logs == null)
            await InitializeLogsAsync();
        
        // we are sure it is not null
        return logs!.Where(log => log.UserId == userId).ToList();
    }

    public async Task<IEnumerable<Logs>> GetAllLogsAsync()
    {
        if (logs == null)
            await InitializeLogsAsync();

        return logs!;
    }

    public async Task<IEnumerable<Logs>> GetLogByIdAsync(long id)
    {
        if (logs == null)
            await InitializeLogsAsync();   

        return logs!.Where(log => log.Id == id).ToList();
    }
}
