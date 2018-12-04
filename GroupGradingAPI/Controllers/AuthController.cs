using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GroupGradingAPI.Data;
using GroupGradingAPI.Models;
using GroupGradingAPI.ViewModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GroupGradingAPI.Controllers
{
    [EnableCors("AllAccessCors")]
    [Route("api")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly GradingContext _context;

        /**
         * AuthController
         *
         * Constructor
         *
         * @param GradingContext context - database context
         * @param UserManager<IdentityUser> userManger - manages user identities
         * @param IConfiguration configuration - key/value configuration properties
         */
        public AuthController(GradingContext context, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
        }

        /**
         * Inserts a NEW teacher into database.
         * @param RegistationModel model - database context
         * @return Ok - returns Ok if user inputs all the correct information
         * @return BadRequest - returns when the information is duplicated or when registration is unsuccessful
         */
        [HttpPost("teacher/register")]
        public async Task<ActionResult> InsertTeacher([FromBody] RegistationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State Invalid");
            }
            var userTest = await _userManager.FindByNameAsync(model.UserName);
            var emailTest = await _userManager.FindByEmailAsync(model.Email);
            if (userTest != null || emailTest != null )
            {
                return BadRequest("User already exists");
            }

            try
            {
                Guid newGuid = Guid.NewGuid();
                var user = new Instructor
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    SecurityStamp = newGuid.ToString(),
                    InstructorRoleId = "Teacher"
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                await _userManager.AddToRoleAsync(user, "Teacher");
                await _context.SaveChangesAsync();
                return Ok(new { Username = user.UserName, response = true });
            }
            catch (Exception e)
            {
                return Ok(new { Error = e.Message, response = false });
            }
        }

        /**
         * Inserts a NEW student into database.
         * @param RegistationModel model - database context
         * @return Ok - returns Ok if user inputs all the correct information
         * @return BadRequest - returns when the information is duplicated or when registration is unsuccessful
         */
        [HttpPost("student/register")]
        public async Task<ActionResult> InsertStudent([FromBody] RegistationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Model State Invalid");
            }
            var userTest = await _userManager.FindByNameAsync(model.UserName);
            var emailTest = await _userManager.FindByEmailAsync(model.Email);
            if (userTest != null || emailTest != null)
            {
                return BadRequest("User already exists");
            }

            try
            {
                Guid newGuid = Guid.NewGuid();
                var user = new Student
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.UserName,
                    SecurityStamp = newGuid.ToString(),
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);

                }

                await _userManager.AddToRoleAsync(user, "Student");
                await _context.SaveChangesAsync();
                return Ok(new { Username = user.UserName, response = true });
            }
            catch (Exception e)
            {
                return Ok(new { Error = e.Message, response = false });
            }
        }

        /**
         * Login for an existing user.
         * @param CredentialsModel model - database context
         * @return Unauthorized - returns unauthorized if username and/or password is incorrect
         * @return Ok - returns a new token if the username AND password is correct and exist
         */
         ///
         /// <summary>
         /// Logs a user in
         /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult> Login(CredentialsModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var claim = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, "Token");
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    claimsIdentity.AddClaim(new Claim("roles", role));
                }
                claimsIdentity.AddClaim(new Claim("uid", user.UserName));
                var signinKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]));

                int expiryInMinutes = Convert.ToInt32(_configuration["Jwt:ExpiryInMinutes"]);

                var token = new JwtSecurityToken(
                  issuer: _configuration["Jwt:Site"],
                  audience: _configuration["Jwt:Site"],
                  claims: claimsIdentity.Claims,
                  expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                  signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(
                  new
                  {
                      uId = user.Id,
                      token = new JwtSecurityTokenHandler().WriteToken(token),
                      expiration = token.ValidTo
                  });
            }
            return Unauthorized();
        }
    }
}
