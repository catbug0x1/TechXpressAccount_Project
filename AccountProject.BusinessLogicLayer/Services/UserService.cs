using AccountProject.BusinessLogicLayer.DTOs;
using AccountProject.DataAccessLayer.Data.Repositories;
using AccountProject.DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AccountProject.BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<(bool Success, string Message, UserDTO User)> RegisterUserAsync(RegisterUserDTO registerDTO)
        {
            // Validate password match
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                return (false, "Passwords do not match", null);
            }

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDTO.Email))
            {
                return (false, "Email is already in use", null);
            }

            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(registerDTO.UserName))
            {
                return (false, "Username is already taken", null);
            }

            // Hash password
            string passwordHash = HashPassword(registerDTO.Password);

            // Create user
            var user = new User
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateUserAsync(user);

            if (!created)
            {
                return (false, "Failed to create user", null);
            }

            var userDto = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return (true, "User registered successfully", userDto);
        }

        public async Task<(bool Success, string Message, UserDTO User)> AuthenticateUserAsync(LoginUserDTO loginDTO)
        {
            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);

            if (user == null)
            {
                return (false, "Invalid email or password", null);
            }

            string passwordHash = HashPassword(loginDTO.Password);

            if (user.PasswordHash != passwordHash)
            {
                return (false, "Invalid email or password", null);
            }

            var userDto = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return (true, "Authentication successful", userDto);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
