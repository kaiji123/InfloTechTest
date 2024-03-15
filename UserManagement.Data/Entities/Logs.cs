using System;
namespace UserManagement.Models;

public class Logs
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public required string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Source { get; set; } 
    public string? Payload { get; set; } 
    public NoticeLevel NoticeLevel { get; set; }
}

public enum NoticeLevel
{
    Info,
    Warning,
    Error
}