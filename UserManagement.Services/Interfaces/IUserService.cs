﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces
{
    public interface IUserService 
    {
        /// <summary>
        /// Return users by active state
        /// </summary>
        /// <param name="isActive"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> FilterByActiveAsync(bool isActive);
        
        Task<IEnumerable<User>> GetAllAsync();
        
        Task<IEnumerable<User>> GetUserByIdAsync(long id);
        
        // adding Update method
        Task UpdateAsync(User updatedUser);
        
        // adding Delete method
        Task DeleteAsync(User userToUpdate);
        
        Task AddAsync(User newUser);
    }
}
