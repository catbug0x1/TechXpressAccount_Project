using AccountProject.BusinessLogicLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountProject.BusinessLogicLayer.Services
{
    public interface IUserService
    {
        Task<(bool Success, string Message, UserDTO User)> RegisterUserAsync(RegisterUserDTO registerDTO);
        Task<(bool Success, string Message, UserDTO User)> AuthenticateUserAsync(LoginUserDTO loginDTO);
    }
}
