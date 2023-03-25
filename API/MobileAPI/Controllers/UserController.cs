using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DataContract.Request;
using DatabaseScaffolding.Model;
using ServiceLayer.Authentication;

namespace MobileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuthFilter]
    public class UserController : ControllerBase
    {
        private DrowsinessDetectionContext _dbContext;

        public UserController(DrowsinessDetectionContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("GetUsers")]
        public IActionResult Get()
        {
            try
            {
                var users = _dbContext.Users.ToList();
                if (users.Count == 0) return StatusCode(404, "No users found");
                return Ok(users);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("CreateUser")]
        public IActionResult Create([FromBody] UserRequest request)
        {
            try
            {
                User user = new User()
                {
                    UserId = Guid.NewGuid(),
                    Name = request.Name,
                    IsDeleted = false,
                    IsDisabled = true,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    LastModifiedDate = DateTime.Now
                };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
            return Ok(new JsonResult(new { Success = true, Message = "User Created Successfully" }));
        }

        [HttpPut("UpdateUser")]
        public IActionResult Update([FromBody] UserRequest request)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.UserId == request.UserId);
                if (user is null) return StatusCode(404, new JsonResult(new { Success = false, Message = "User not found" }));

                user.Name = request.Name ?? user.Name;
                user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
                user.Email = request.Email ?? user.Email;
                user.LastModifiedDate = DateTime.Now;

                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
            return Ok(new JsonResult(new { Success = true, Message = "User Updated Successfully" }));
        }

        [HttpDelete("DeleteUser/{Id}")]
        public IActionResult Remove([FromRoute] Guid Id)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.UserId == Id);
                if (user is null) return StatusCode(404, new JsonResult(new { Success = false, Message = "User not found" }));

                _dbContext.Remove(user);
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            { 
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
            return Ok(new JsonResult(new { Success = true, Message = "User Hard Deleted Successfully" }));
        }

        [HttpPut("DisableUser/{Id}")]
        public IActionResult DisableUser([FromRoute] Guid Id)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(x => x.UserId == Id);
                if (user is null) return StatusCode(404, new JsonResult(new { Success = false, Message = "User not found" }));
                user.IsDisabled = true;

                _dbContext.Entry(user).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
            }
            return Ok(new JsonResult(new { Success = true, Message = "User Disabled Successfully" }));
        }
    }
}
