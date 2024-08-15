using InvestCloudFrontTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace InvestCloudFrontTest.Services
{
    public class Response
    {
        public bool Successfull { get; set; }
        public string Error {  get; set; }  
    }
    public interface IUserService
    {
        Task<Response> CreateAccountAsync(User newUser);
        Task<Response> ValidUserAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly DatabaseHelper _databaseHelper;
        public UserService(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
        }
         
        public Task<int> CreateUserAsync(User user)
        {
            return _databaseHelper.SaveUserAsync(user);
        }
        public async Task<Response> ValidUserAsync(User user)
        {
            Response response = new Response();
            var dbuser = await _databaseHelper.GetUserAsync(user.Username);
            if (dbuser == null)
            {
                response.Successfull = false;
                response.Error = "User does not exist";
            }
            else if (dbuser.Password == user.Password)
            {
                response.Successfull = true;
                response.Error = "Login Successful";
            }
            else 
            {
                response.Successfull = false;
                response.Error = "Password does not match";
            }
            return response; 
        }

        public async  Task<Response> CreateAccountAsync(User newUser)
        {
            Response response = new Response();
            var dbuser = await _databaseHelper.GetUserAsync(newUser.Username);
            if (dbuser == null)
            {
                response.Successfull = false;
                var result = await _databaseHelper.SaveUserAsync(newUser);
                if(result == -1)
                {
                    response.Successfull = false;
                    response.Error = "Could not save Account";
                }
                else
                {
                    response.Successfull = true;
                    response.Error = "Account Created Successfuly";
                }
            }
            else
            {
                response.Successfull = false;
                response.Error = "User Already Exist";
            }
            return response;
        }
    }

}
