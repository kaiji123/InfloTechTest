using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

public interface ILogService
{
    void LogAction(long userId, string action, string source, string? payload = null, NoticeLevel noticeLevel = NoticeLevel.Info);

    Task<IEnumerable<Logs>> GetLogsForUserAsync(long userId);

    Task<IEnumerable<Logs>> GetAllLogsAsync();

    Task<IEnumerable<Logs>> GetLogByIdAsync(long id);
}
