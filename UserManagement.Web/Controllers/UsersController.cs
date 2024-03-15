using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    private readonly ILogService _logService; // Inject ILogService
    private readonly string source = "UserManagement.Web";
    public UsersController(IUserService userService, ILogService logService) // Inject ILogService
    {
        _userService = userService;
        _logService = logService; // Assign the injected service to the private field
    }

    [HttpGet]
    public async Task<IActionResult> ListAsync()
    {
        var items = (await _userService.GetAllAsync()).Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            // assign DateOfBirth if want to display
            // DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        // return View(model); // if we are following asp mvc structure
        return Ok(model); // Change the return type to IActionResult and return Json(model)
    }

    // added filter route, this becomes redundant when we add blazor, because we can handle it fully client side
    [HttpGet("filter")]
    public async Task<IActionResult> FilterAsync(bool isActive)
    {   
        var filteredItems = await _userService.FilterByActiveAsync(isActive);
        // Select the properties for ViewModel
        var userViewModels = filteredItems.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive
        }).ToList();

        // return the User list model with filtered results
        var model = new UserListViewModel
        {
            Items = userViewModels
        };
        return View("List", model); 
    }



    [HttpGet("edit/{id}")]
    public async Task<IActionResult> EditAsync(long id)
    {
        
        // get requestHeaders
        string? requestHeaders = GetRequestHeadersJson();
        var users = await _userService.GetUserByIdAsync(id);
        var user = users.FirstOrDefault();

        // handle not found
        if (user == null)
        {
            _logService.LogAction(id, "User not found", source, payload: requestHeaders, noticeLevel: NoticeLevel.Error);
            return NotFound();
        }

        // init model for view
        var model = new UserListViewModel
        {
            Items = new List<UserListItemViewModel> 
            {
                new UserListItemViewModel
                {
                    Id = user.Id,
                    Forename = user.Forename,
                    Surname = user.Surname,
                    Email = user.Email,
                    IsActive = user.IsActive
                }
            }
        };

        // return View("Edit", model.Items.FirstOrDefault());
        return Ok(model.Items.FirstOrDefault());
    }

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> EditAsync(long id, [FromBody] UserListItemViewModel model)
    {    

        Console.WriteLine(id);
        // get requestHeaders
        var requestHeaders = GetRequestHeadersJson();

        // If the model state is not valid, return the same view with validation errors
        if (!ModelState.IsValid)
        {
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key]!.Errors;
      
                foreach (var error in errors)
                {
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            
            return View(model);
        }

        // find user
        var usersTask = _userService.GetUserByIdAsync(id);
        var userToUpdate = (await usersTask).FirstOrDefault();

        // we this condition here because we might want to know who is trying the requests of not found users
        if (userToUpdate == null)
        {
            _logService.LogAction(id, "User not found", source, payload: requestHeaders, noticeLevel: NoticeLevel.Error);
            return NotFound();
        }
        string changes = GetChanges(userToUpdate, model);
        _logService.LogAction(id, changes, source, requestHeaders);


        // Update user details
        userToUpdate.Forename = model.Forename;
        userToUpdate.Surname = model.Surname;
        userToUpdate.Email = model.Email;
        userToUpdate.IsActive = model.IsActive;

        // calling update method from service
        await _userService.UpdateAsync(userToUpdate);

        return RedirectToAction("List");
    }



    [HttpGet("view/{id}")]
    public async Task<IActionResult> ViewAsync(long id)
    {
       var requestHeaders = GetRequestHeadersJson();
        var usersTask = _userService.GetUserByIdAsync(id);
        var user = (await usersTask).FirstOrDefault();

        // return notfound if user is not found
        if (user == null)
        {
            _logService.LogAction(id, "User not found", source, payload: requestHeaders, noticeLevel: NoticeLevel.Error);
            return NotFound();
        }

        // get all logs for that user
        var logsTask = _logService.GetLogsForUserAsync(id);
        var logs = await logsTask;
        // assign to viewmodel
        var logViewModels = logs.Select(log => new LogItemViewModel
        {
            UserId = log.UserId ?? id,
            Action = log.Action,
            Timestamp = log.Timestamp
        }).ToList();


        var model = new UserListViewModel
        {
            Items = new List<UserListItemViewModel> 
            {
                new UserListItemViewModel
                {
                    Id = user.Id,
                    Forename = user.Forename,
                    Surname = user.Surname,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    Logs = logViewModels
                }
            }
        };
        _logService.LogAction(id, "User Viewed", source, payload: requestHeaders);
        // return View("View", model.Items.FirstOrDefault());
        return Ok(model.Items.FirstOrDefault());
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        string? requestHeaders = GetRequestHeadersJson();
        var usersTask = _userService.GetUserByIdAsync(id);
        var user = (await usersTask).FirstOrDefault();
        if (user == null)
        {
            _logService.LogAction(id, "User not found", source, payload: requestHeaders, noticeLevel: NoticeLevel.Error);
            return NotFound();
        }

        var model = new UserListViewModel
        {
            Items = new List<UserListItemViewModel> 
            {
                new UserListItemViewModel
                {
                    Id = user.Id,
                    Forename = user.Forename,
                    Surname = user.Surname,
                    Email = user.Email,
                    IsActive = user.IsActive
                }
            }
        };
        

        // return View("Delete", model.Items.FirstOrDefault());
        return Ok(model.Items.FirstOrDefault());
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> DeleteConfirmedAsync(long id)
    {
        var requestHeaders = GetRequestHeadersJson();
        var usersTask = _userService.GetUserByIdAsync(id);
        var userToDelete = (await usersTask).FirstOrDefault();
        if (userToDelete == null)
        {
            // log user not found
            _logService.LogAction(id, "User not found", source, payload: requestHeaders, noticeLevel: NoticeLevel.Error);
            return NotFound();
        }

        // delete user
        await _userService.DeleteAsync(userToDelete);

        // log user deleted
        _logService.LogAction(id, "User deleted", source, payload: requestHeaders);

        return RedirectToAction("List");
    }



    [HttpGet("add")]
    public IActionResult Add()
    {
        // Return a view for adding a new user
        return View();
    }


    [HttpPost("add")]
    public IActionResult Add([FromBody]UserListItemViewModel model)
    {
        var requestHeaders = GetRequestHeadersJson();
        if (!ModelState.IsValid)
        {
            // If the model state is not valid, return the same view with validation errors
            return View(model);
        }

        // Create a new user object and populate it with data from the model
        var newUser = new User
        {
            Forename = model.Forename,
            Surname = model.Surname,
            Email = model.Email,
            IsActive = model.IsActive
        };



        _logService.LogAction(newUser.Id, "User added", source, payload: requestHeaders);



        // Call the service method to add the new user
        _userService.AddAsync(newUser);

        // Redirect to the list view after successfully adding the user
        return RedirectToAction("List");
    }


        // a method to get all edited changes to a string
        private string GetChanges(User existingUser, UserListItemViewModel newUser)
        {
            string changes = "";

            if (existingUser.Forename != newUser.Forename)
            {
                changes += $"Forename changed from '{existingUser.Forename}' to '{newUser.Forename}'";
            }

            if (existingUser.Surname != newUser.Surname)
            {
                changes += $"{(string.IsNullOrEmpty(changes) ? "" : ", ")} Surname changed from '{existingUser.Surname}' to '{newUser.Surname}'";
            }

            if (existingUser.Email != newUser.Email)
            {
                changes += $"{(string.IsNullOrEmpty(changes) ? "" : ", ")} Email changed from '{existingUser.Email}' to '{newUser.Email}'";
            }

            if (existingUser.IsActive != newUser.IsActive)
            {
                changes += $"{(string.IsNullOrEmpty(changes) ? "" : ", ")} IsActive changed from '{existingUser.IsActive}' to '{newUser.IsActive}'";
            }

            return changes;
        }

        private string GetRequestHeadersJson()
        {
            return "{" + string.Join(",\n", Request.Headers.Select(header => $"\"{header.Key}\": \"{string.Join(",", header.Value!)}\"")) + "}";
        }
}
