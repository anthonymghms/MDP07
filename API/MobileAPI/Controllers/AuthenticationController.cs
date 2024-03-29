﻿using Common;
using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using DataContract.Request;
using ServiceLayer.EmailService;
using ServiceLayer.Authentication;
using DatabaseMigration.Model;
using Microsoft.AspNetCore.Cors;
using DatabaseMigration;

namespace User.Management.API.Controllers


{
    [EnableCors("AllowAll")]
    [Route("/api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        #region Attributes

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DrowsinessDetectionContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructors

        public AuthenticationController(UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService,
            SignInManager<AppUser> signInManager, IConfiguration configuration,
            DrowsinessDetectionContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
            _context = context;
        }
        
        #endregion

        #region Endpoints

        [ApiKeyAuthFilter]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, string role)
        {
            //Check User Exist
            try
            {
                var userExist = await _userManager.FindByEmailAsync(request.Email);
                if (userExist != null)
                {
                    return StatusCode(StatusCodes.Status403Forbidden,
                        new Response { Status = "Error", Message = "User already exists!" });
                }
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = ex.Message });
            }
            //Add the User in the database
            AppUser user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username,
                PhoneNumber = request.PhoneNumber,
                TwoFactorEnabled = false,
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                Birthday = DateTime.Parse(request.Birthday),
                LoginCount = 0,
            };

            UserConfig settings = user.UserConfig = new UserConfig
            {
                AlertType = "Visual",
                AlertVolume = 100,
                TwoFactorAuthEnabled = false,
                DarkMode = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                NotificationsEnabled = false,
                PhoneNumber = request.PhoneNumber,
                Id = Guid.NewGuid(),
                User = user,
                UserId = user.Id,
                Username = request.Username,
                LocationSharing = true,
                AlertLevel = "Moderate"
            };
            if (await _roleManager.RoleExistsAsync(role))
            {
                try
                {
                    var result = await _userManager.CreateAsync(user, request.Password);
                    var _ = await _context.UserConfig.AddAsync(settings);
                    if (!result.Succeeded)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = "Error", Message = result.ToString() });
                    }
                    //Add role to the user....
                    await _userManager.AddToRoleAsync(user, role);

                    //Add Token to Verify the email....
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                    var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                    _emailService.SendEmail(message);
                }
                catch (Exception e)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                            new Response { Status = "Error", Message = e.Message });
                }


                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"User created & Email sent to {user.Email} Successfully" });

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "This role does not exist." });
            }


        }

        [ApiKeyAuthFilter]
        [HttpGet("SendConfirmationEmail")]
        public async Task<IActionResult> SendConfirmationEmail(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Confirmation email link", confirmationLink!);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"Email sent to \'{HalfHiddenEmail(user.Email)}\' successfully" });
            }
            catch  
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "An error has occured" });
            }
        }
        
        [ApiKeyAuthFilter]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // handle the forgot password request
            try
            {
                var user = await _userManager.FindByNameAsync(request.Username);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordLink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email}, Request.Scheme);
                var message = new Message(new string[] { user.Email! }, "Reset password link", resetPasswordLink!);
                _emailService.SendEmail(message);
                return StatusCode(StatusCodes.Status200OK,
                    new Response { Status = "Success", Message = $"Email sent to \'{HalfHiddenEmail(user.Email)}\' successfully" });
            }
            catch(Exception e)
            {
                return StatusCode(StatusCodes.Status100Continue, new Response { Status = "Error", Message= e.Message });
            }
        }
        
        [HttpGet("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
            }
            catch { }
            return StatusCode(StatusCodes.Status200OK,
                        new Response());
        }
        
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                      new Response { Status = "Success", Message = "Email Verified Successfully" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                       new Response { Status = "Error", Message = "This User Doesnot exist!" });
        }

        [ApiKeyAuthFilter]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                     new Response { Status = "Error", Message = $"Please confirm your email by clicking the link sent to {HalfHiddenEmail(user.Email)}" });
                }
                if (await _userManager.IsLockedOutAsync(user))
                {
                    TimeSpan span = (user.LockoutEnd - DateTime.Now).Value;
                    return StatusCode(StatusCodes.Status423Locked,
                     new Response { Status = "Error", Message = $"Your account {user.UserName} is currently locked please wait {span.Minutes} minutes and {span.Seconds} seconds" });
                }
                if (user.TwoFactorEnabled)
                {
                    Console.WriteLine("enabled");
                    await _signInManager.SignOutAsync();
                    await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
                    var token = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);

                    var message = new Message(new string[] { user.Email! }, "OTP Confirmation", token);
                    _emailService.SendEmail(message);

                    return StatusCode(StatusCodes.Status200OK,
                     new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}" });
                }
                user.AccessFailedCount = 0;
                user.LoginCount += 1;
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                

                var jwtToken = GetToken(authClaims);

                await _userManager.UpdateAsync(user);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    loginCount = user.LoginCount,
                    expiration = jwtToken.ValidTo
                });
                //returning the token...

            }
            if (user != null && user.AccessFailedCount >= Int32.Parse(_configuration["Limit:MaxWrongPasswordAttemptsToLock"]))
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddMinutes(Int32.Parse(_configuration["Limit:LockTime"])));
                return StatusCode(StatusCodes.Status423Locked,
                 new Response { Status = "Error", Message = $"Your account {user.UserName} has been locked please wait {_configuration["Limit:LockTime"]} minutes" });
            }
            if (user != null)
            {
                user.AccessFailedCount += 1;
                await _userManager.UpdateAsync(user);
            }
            return StatusCode(StatusCodes.Status401Unauthorized,
                 new Response { Status = "Error", Message = $"Wrong username or password" });
        }

        [ApiKeyAuthFilter]
        [HttpPost("Login-2FA")]
        public async Task<IActionResult> LoginWithOTP([FromBody] OtpRequest request)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(request.username);
                Console.WriteLine($"User object: {user}");
                Console.WriteLine($"Request username: {request.username}");
                Console.WriteLine($"Request OTP: {request.otp}");
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound,
                       new Response { Status = "Not Found", Message = "There is no such user" });
                }
                if (user != null && user.TwoFactorEnabled)
                {
                    var signIn = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, request.otp, true, true);
                    Console.WriteLine($"SignIn suceeded: {signIn.Succeeded}");
                    Console.WriteLine($"SignIn is not allowed: {signIn.IsNotAllowed}");
                    Console.WriteLine($"SignIn is locked out: {signIn.IsLockedOut}");
                    Console.WriteLine($"SignIn reuires two factor: {signIn.RequiresTwoFactor}");


                    if (signIn.Succeeded)
                    {
                        Console.WriteLine("inside");
                        if (user != null)
                        {
                            var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };
                            var userRoles = await _userManager.GetRolesAsync(user);
                            foreach (var role in userRoles)
                            {
                                authClaims.Add(new Claim(ClaimTypes.Role, role));
                            }

                            var jwtToken = GetToken(authClaims);

                            Console.WriteLine(jwtToken);

                            return Ok(new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                                expiration = jwtToken.ValidTo
                            });
                        }
                    }
                    return StatusCode(StatusCodes.Status401Unauthorized,
                    new Response { Status = "Error", Message = $"Invalid Code, you sent user:{request.username} and code:{request.otp}" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized,
                        new Response { Status = "Error", Message = "User null or two factor not enabled" });

                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized,
                new Response { Status = "Error", Message = $"{e.Message}" });
            }
        }

        #endregion

        #region Private Methods

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private string HalfHiddenEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            int hiddenChars = atIndex / 2;

            string hiddenEmail = new string('*', 2) + email.Substring(hiddenChars);

            return hiddenEmail;
        }

        #endregion

        #region Tests

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("AuthorizeTest")]
        public string AuthorizeTest()
        {
            var identity = HttpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                return $"Hello, {identity.Name}!";
            }
            else
            {
                return "Not authenticated.";
            }
        }

        [HttpGet("Test")]
        public string Test()
        {
            return "Success";
        }

        #endregion
    }
}