using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class UserListViewModel
{
    public List<UserListItemViewModel> Items { get; set; } = new();
}

public class UserListItemViewModel
{   

    // we will change the existing properties to required so it can give error when user inputs null     
    public required long Id { get; set; }

    [Required(ErrorMessage = "Forename is required")]
    public required string Forename { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    public required string Surname { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public required string Email { get; set; }

    public bool IsActive { get; set; }

    // decided to add Logs property to UserClass
    public List<LogItemViewModel> Logs { get; set; } = new List<LogItemViewModel>();

    // this is to be added when we want to view the DateOfBirth 
    // public DateTime DateOfBirth { get; internal set; }
}

// viewmodel for UserLog
public class LogItemViewModel
{

    public long Id { get; set; }
    public long ? UserId { get; set; }
    public required string Action { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Source {get;set;}
    public string? Payload {get; set;}
}
