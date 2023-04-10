using Core.Consts;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Web.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAuthRepository _authService;
        private readonly IEmailService _emailService;
            
        public AccountsController(IAuthRepository authService,IEmailService emailService)
        {
            _authService  = authService ?? throw new ArgumentNullException(nameof(authService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signup([FromBody] RegisterationDto model)
        { 
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            if (!result.IsAuthenticate)
                throw new BadRequestException(result.Message);

            var filePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Templates\\Welcome.html";
            var str = new StreamReader(filePath);

            var mailText = str.ReadToEnd();
            str.Close();


            await _emailService.SendEmailAsync(model.Email, "Welcome to My Ecoommerce.Net2023", mailText);

            return Ok(result);
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Signin([FromBody] LoginingDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            if (!result.IsAuthenticate)
                throw new BadRequestException(result.Message);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> PostRole([FromBody]AddRoleModel model) 
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddRoleAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([Required]string email)
        {
           int code =  await _authService.GenerateResetPassordOTP(email);

            await _emailService.SendEmailAsync(email, "OTP for ResetPassword", $"{code} this code for reset passowrd.");
           
            return Ok("please check your email..");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPAssword(ResetPasswordDto model)
        {
            var credentials = await _authService.ResetPassword(model);

            if (credentials == null)
                throw new BadRequestException("please check your entered data");

            var result = await _authService.LoginAsync(credentials);

            if (!result.IsAuthenticate)
                throw new BadRequestException(result.Message);

            return Ok(result);
        }
    }
    
}

