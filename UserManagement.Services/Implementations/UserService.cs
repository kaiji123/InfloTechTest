using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations
{
    public class UserService : IUserService
    {
        private readonly IDataContext _dataAccess;
        private List<User>? users; // Define the users field for caching purpose

        public UserService(IDataContext dataAccess)
        {
            _dataAccess = dataAccess;
            // Initialize the users list asynchronously
            InitializeUsersAsync().Wait();
        }

        // Initialize the users list asynchronously
        private async Task InitializeUsersAsync()
        {
            users = (await _dataAccess.GetAll<User>().ToListAsync()).ToList();
        }

        /// <summary>
        /// Return users by active state
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
        {
            if (users == null)
                await InitializeUsersAsync();

            return users!.Where(user => user.IsActive == isActive);
        }

        public async Task<IEnumerable<User>> GetUserByIdAsync(long id)
        {
            if (users == null)
                await InitializeUsersAsync();

            return users!.Where(user => user.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            if (users == null)
                await InitializeUsersAsync();

            return users!;
        }

        // Update user asynchronously
        public async Task UpdateAsync(User updatedUser)
        {
            await _dataAccess.UpdateAsync(updatedUser);
      
        }

        // Delete user asynchronously
        public async Task DeleteAsync(User userToUpdate)
        {
            await _dataAccess.DeleteAsync(userToUpdate);
          
        }

        // Create new user asynchronously
        public async Task AddAsync(User newUser)
        {
            await _dataAccess.CreateAsync(newUser);
   
        }
    }
}
