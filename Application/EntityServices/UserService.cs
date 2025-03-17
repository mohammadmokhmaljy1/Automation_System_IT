using Infrastructure.Core.Consts;
using Infrastructure.Core.DTOs.Requestes;
using Infrastructure.Core.DTOs.Responses;
using Infrastructure.Core.Entities;
using Infrastructure.Core.Interfaces.Application.EntityServices;
using Infrastructure.Core.Interfaces.Application.UtilityServices;
using Infrastructure.Core.Interfaces.Infrastructure;
using IT_Automation.API.Application.UtilityServices; 
using Microsoft.AspNetCore.Identity;

namespace IT_Automation.API.Application.EntityServices
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IVerificationService _verificationService;
        private readonly IUnitOfWork _db;
        private readonly IHttpContextAccessor _httpContext;

        public UserService(
            UserManager<AppUser> userManager,
            IJwtService jwtService,
            IVerificationService verificationService,
            IUnitOfWork db,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _verificationService = verificationService;
            _db = db;
            _httpContext = httpContextAccessor;
        }



        public async Task<GeneralResponse> GetUserById(Guid UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<GeneralResponse> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                return GeneralResponse.NotFound("user not found.");

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!result)
                return GeneralResponse.BadRequest("Invalid password.");

            var token = await _jwtService.GenerateToken(user, _userManager);
            var data = new LoginResponseDto() { Token = token };
            return GeneralResponse.Ok("Login successful.", data);
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
                return GeneralResponse.BadRequest("Email is already in use.");

            if (await _userManager.FindByNameAsync(registerRequest.UserName) != null)
                return GeneralResponse.BadRequest("Username is already taken.");

            var user = new AppUser
            {
                Email = registerRequest.Email,
                EmailConfirmed = false,
                PhoneNumber = registerRequest.Phone,
                UserName = registerRequest.UserName
            };
            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded)
                return GeneralResponse.BadRequest($"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRoleAsync(user, Roles.User.GetValue());
            await _verificationService.SendVerificationCodeAsync(user.Id, user.Email, "Email Verification");

            var tokne = await _jwtService.GenerateToken(user, _userManager);
            var data = new LoginResponseDto() { Token = tokne };
            return GeneralResponse.Ok("User created. Please check your email for verification code.", data);
        }

        public async Task<GeneralResponse> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordRequestDto.userId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordRequestDto.resetToken, resetPasswordRequestDto.newPassword);
            return result.Succeeded
                ? GeneralResponse.Ok("Password reset successfully.")
                : GeneralResponse.BadRequest("Failed to reset password.");
        }

        public async Task<GeneralResponse> SendPasswordResetCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return GeneralResponse.NotFound("Email not found.");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _verificationService.SendVerificationCodeAsync(user.Id, user.Email, "Password Reset", resetToken);

            return GeneralResponse.Ok("Password reset token sent to your email.");
        }

        public async Task<GeneralResponse> VerifyEmailAsync(VerifyEmailRequestDto verifyEmailRequestDto)
        {
            if (!await _verificationService.VerifyCodeAsync(verifyEmailRequestDto.userId, verifyEmailRequestDto.verificationCode))
                return GeneralResponse.BadRequest("Invalid verification code.");

            var user = await _userManager.FindByIdAsync(verifyEmailRequestDto.userId.ToString());
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return GeneralResponse.Ok("Email successfully verified.");
        }
    }

}
