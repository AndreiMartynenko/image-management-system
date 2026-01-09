using HealthcareIMS.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class RolesController : Controller
{
    // اکشن برای نمایش صفحه (GET)
    [HttpGet]
    public IActionResult AddRoleToUser()
    {
        return View();
    }

    // اکشن برای پردازش فرم (POST)
    [HttpPost]
    public IActionResult AddRoleToUser(string email, string role)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
        {
            return BadRequest("Email or role cannot be empty.");
        }

        // منطق اضافه کردن نقش به کاربر
        return Ok($"Role '{role}' added to user '{email}' successfully.");
    }
}

