using Infrastructure.Core.DTOs.Requestes;
using Infrastructure.Core.Interfaces.Application.EntityServices;
using IT_Automation.API.Application.UtilityServices;
using Microsoft.AspNetCore.Mvc;

namespace IT_Automation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync(LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await userService.LoginAsync(loginRequest);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await userService.RegisterAsync(registerRequest);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await userService.ResetPasswordAsync(resetPasswordRequestDto);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("SendPasswordResetCode")]
        public async Task<IActionResult> SendPasswordResetCodeAsync(string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await userService.SendPasswordResetCodeAsync(email);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("VerifyEmail")]
        public async Task<IActionResult> VerifyEmailAsync(VerifyEmailRequestDto verifyEmailRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var Result = await userService.VerifyEmailAsync(verifyEmailRequestDto);
                var Response = Result.ToActionResult();
                return Response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
