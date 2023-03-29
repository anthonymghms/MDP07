using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DataContract.Request;
using DatabaseMigration;
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

        //[HttpGet("GetUsers")]
        //public IActionResult Get()
        //{
        //    try
        //    {
        //        var users = _dbContext.Users.ToList();
        //        if (users.Count == 0) return StatusCode(404, "No users found");
        //        return Ok(users);
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, e.Message);
        //    }
        //}

        //[HttpPost("CreateUser")]
        //public IActionResult Create([FromBody] UserRequest request)
        //{
        //    try
        //    {
        //        User user = new User()
        //        {
        //            UserId = Guid.NewGuid(),
        //            FirstName = request.FirstName,
        //            LastName = request.LastName,
        //            Username = request.Username,
        //            Password = request.Password,
        //            PhoneNumber = request.PhoneNumber,
        //            Email = request.Email,
        //            IsDeleted = false,
        //            IsDisabled = true,
        //            CreationDate = DateTime.Now,
        //            LastModifiedDate = DateTime.Now
        //        };
        //        _dbContext.Users.Add(user);
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
        //    }
        //    return Ok(new JsonResult(new { Success = true, Message = "User Created Successfully" }));
        //}

        //[HttpPut("UpdateUser")]
        //public IActionResult Update([FromBody] UserRequest request)
        //{
        //    try
        //    {
        //        var user = _dbContext.Users.FirstOrDefault(x => x.UserId == request.UserId);
        //        if (user is null) return StatusCode(404, new JsonResult(new { Success = false, Message = "User not found" }));

        //        user.FirstName = request.FirstName ?? user.FirstName;
        //        user.LastName = request.LastName ?? user.LastName;
        //        user.Username = request.Username ?? user.Username;
        //        user.Password = request.Password ?? user.Password;
        //        user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
        //        user.Email = request.Email ?? user.Email;
        //        user.LastModifiedDate = DateTime.Now;

        //        _dbContext.Entry(user).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
        //    }
        //    return Ok(new JsonResult(new { Success = true, Message = "User Updated Successfully" }));
        //}

        //[HttpDelete("DeleteUser")]
        //public IActionResult Remove([FromBody] UserRequest request)
        //{
        //    try
        //    {
        //        var user = _dbContext.Users.FirstOrDefault(x => x.UserId == request.UserId);
        //        if (user is null) return StatusCode(404, new JsonResult(new { Success = false, Message = "User not found" }));
        //        user.IsDeleted = true;

        //        _dbContext.Entry(user).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
        //    }
        //    return Ok(new JsonResult(new { Success = true, Message = "User Deleted Successfully" }));
        //}

        //[HttpPut("DisableUser")]
        //public IActionResult DisableUser([FromBody] UserRequest request)
        //{
        //    try
        //    {
        //        var user = _dbContext.Users.FirstOrDefault(x => x.UserId == request.UserId);
        //        if (user is null) return StatusCode(404, new JsonResult(new { Success = false, Message = "User not found" }));
        //        user.IsDisabled = true;

        //        _dbContext.Entry(user).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
        //    }
        //    return Ok(new JsonResult(new { Success = true, Message = "User Disabled Successfully" }));
        //}

        //[HttpPut("AddEmergencyContact/{Id}")]
        //public IActionResult AddEmergencyContact([FromRoute] Guid Id, [FromBody] EmergencyContactRequest request)
        //{
        //    try
        //    {
        //        var user = _dbContext.Users.FirstOrDefault(x => x.UserId == Id);
        //        var emergencyContactId = Guid.NewGuid();
        //        user.EmergencyContacts.Add(new UserEmergencyContact
        //        {
        //            UserId = Id,
        //            User = user,
        //            EmergencyContactId = emergencyContactId,
        //            EmergencyContact = new EmergencyContact
        //            {
        //                EmergencyContactId = emergencyContactId,
        //                EmergencyContactUserId = request.EmergencyContactUserId,
        //                CreationDate = DateTime.Now,
        //                LastModifiedDate = DateTime.Now,
        //                IsDeleted = false,
        //                IsDisabled = false
        //            }
        //        });
        //        _dbContext.Entry(user).State = EntityState.Modified;
        //        _dbContext.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, new JsonResult(new { Success = false, message = e.Message }));
        //    }
        //    return Ok(new JsonResult(new { Success = true, Message = "Emergency Contact Added Successfully" }));
        //}
    }
}
