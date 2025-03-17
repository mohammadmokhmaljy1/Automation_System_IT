using Infrastructure.Core.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace IT_Automation.API.Application.UtilityServices
{
    public static class Helper
    {
        public static Guid GetUserId(this IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);

            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }
        public static string GetValue(this Enum enumValue)
        {
            // Get the enum type
            var enumType = enumValue.GetType();

            // Get the member information for the enum value
            var field = enumType.GetField(enumValue.ToString());

            // Look for the EnumMember attribute
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();

            // Return the EnumMember value if found, otherwise fall back to the enum value's name
            return attribute?.Value ?? enumValue.ToString();
        }
        public static IActionResult ToActionResult(this GeneralResponse response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }
        public static MailMessage AddRecipient(this MailMessage mailMessage, string toEmail)
        {
            mailMessage.To.Add(toEmail);
            return mailMessage;
        }
    }

}
