using LoginAPI.Data;
using LoginAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LoginAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UsersController(UserManager<Users> userManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(CreateRoleDTO roleDTO)
        {

            var response = await _roleManager.CreateAsync(new IdentityRole
            {
                Name = roleDTO.RoleName
            });

            if (response.Succeeded)
            {
                return Ok("New Role Created");
            }
            else
            {
                return BadRequest(response.Errors);
            }
        }


        [HttpPost("AssignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserDTO assignRoleToUserDTO)
        {

            var userDetails = await _userManager.FindByNameAsync(assignRoleToUserDTO.EmployeeNumber);

            if (userDetails != null)
            {

                var userRoleAssignResponse = await _userManager.AddToRoleAsync(userDetails, assignRoleToUserDTO.RoleName);

                if (userRoleAssignResponse.Succeeded)
                {
                    return Ok("Role Assigned to User: " + assignRoleToUserDTO.RoleName);
                }
                else
                {
                    return BadRequest(userRoleAssignResponse.Errors);
                }
            }
            else
            {
                return BadRequest("There are no user exist with this email");
            }


        }

        [AllowAnonymous]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var response = new MainResponse();
            if (refreshTokenRequest is null)
            {
                response.ErrorMessage = "Invalid  request";
                return BadRequest(response);
            }

            var principal = GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);

            if (principal != null)
            {
                var name = principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Name);

                var user = await _userManager.FindByNameAsync(name?.Value);

                if (user is null || user.RefreshToken != refreshTokenRequest.RefreshToken)
                {
                    response.ErrorMessage = "Invalid Request";
                    return BadRequest(response);
                }

                string newAccessToken = GenerateAccessToken(user);
                string refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                response.IsSuccess = true;
                response.Content = new AuthenticationResponse
                {
                    RefreshToken = refreshToken,
                    AccessToken = newAccessToken
                };
                return Ok(response);
            }
            else
            {
                return ErrorResponse.ReturnErrorResponse("Invalid Token Found");
            }

        }



        [AllowAnonymous]
        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser(AuthenticateUser authenticateUser)
        {
            if (authenticateUser is null) return Unauthorized();
            if (!ModelState.IsValid) return Unauthorized();

            var user = await _userManager.FindByNameAsync(authenticateUser.UserName);
            if (user == null) return Unauthorized();

            bool isValidUser = await _userManager.CheckPasswordAsync(user, authenticateUser.Password);

            if (isValidUser)
            {
                string accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                var role = await _userManager.GetRolesAsync(user);

                int? roleID = null;

                switch (role?.FirstOrDefault()?.ToLower())
                {
                    case "supervisor":
                        roleID = (int)RoleDetails.Supervisor;
                        break;
                    case "hr":
                        roleID = (int)RoleDetails.HR;
                        break;
                    case "hod":
                        roleID = (int)RoleDetails.HOD;
                        break;
                    case "employee":
                        roleID = (int)RoleDetails.Employee;
                        break;
                    case null:
                        roleID = (int)RoleDetails.Employee;
                        break;
                }
                var userDetail = new UserDTO
                {
                    EmployeeNumber = user.EmployeeCodeNumber,
                    EmployeeName = user.EmployeeName,
                    GivenName = user.GivenName,
                    Surname = user.Surname,
                    Role = role?.FirstOrDefault(),
                    RoleID = roleID
                };

                var response = new LoginResponse
                {
                    Token = accessToken,
                    UserDetail = userDetail
                };
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }


        private string GenerateAccessToken(Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration["JWT:Audience"],
                Issuer = _configuration["JWT:Issuer"],
                Expires = DateTime.UtcNow.AddMinutes(30),
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyDetail), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(keyDetail),
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameter, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        private string GenerateRefreshToken()
        {

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        [AllowAnonymous]
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDTO)
        {
            var userToBeCreated = new Users
            {
                DepartmentCode = registerUserDTO.DepartmentCode,
                EmployeeNumber = registerUserDTO.EmployeeNumber,
                GivenName = registerUserDTO.GivenName,
                Surname = registerUserDTO.Surname,
                Gender = registerUserDTO.Gender,
                UserName = registerUserDTO.EmployeeCodeNumber,
            };


            var response = await _userManager.CreateAsync(userToBeCreated, registerUserDTO.Password);
            if (response.Succeeded)
            {
                var employeerole = _roleManager.FindByNameAsync("Employee").Result;
                if (employeerole != null)
                {
                    IdentityResult roleresult = await _userManager.AddToRoleAsync(userToBeCreated, employeerole.Name);
                }
                return Ok(new MainResponse
                {
                    IsSuccess = true,
                });
            }
            else
            {
                return ErrorResponse.ReturnErrorResponse(response.Errors?.ToString() ?? "");
            }
        }


        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser(DeleteUserDTO userDetails)
        {

            var existingUser = await _userManager.FindByNameAsync(userDetails.EmployeeNumber);
            if (existingUser != null)
            {
                var response = await _userManager.DeleteAsync(existingUser);

                if (response.Succeeded)
                {
                    return Ok(new MainResponse
                    {
                        IsSuccess = true,
                    });
                }
                else
                {
                    return ErrorResponse.ReturnErrorResponse(response.Errors?.ToString() ?? "");
                }
            }
            else
            {
                return ErrorResponse.ReturnErrorResponse("No User found with this email");
            }
        }
    }
}