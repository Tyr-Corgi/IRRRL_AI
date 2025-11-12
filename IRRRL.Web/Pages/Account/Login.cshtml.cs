using System.ComponentModel.DataAnnotations;
using IRRRL.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IRRRL.Web.Pages.Account;

[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public void OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        _logger.LogInformation("Login POST started for email: {Email}", Input?.Email ?? "NULL");
        
        try
        {
                if (ModelState.IsValid && Input != null)
                {
                    _logger.LogInformation("ModelState is valid, attempting sign in");
                    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                _logger.LogInformation("SignIn result - Succeeded: {Succeeded}, IsLockedOut: {IsLockedOut}, IsNotAllowed: {IsNotAllowed}", 
                    result.Succeeded, result.IsLockedOut, result.IsNotAllowed);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Login successful, redirecting to /Home");
                    // Login successful - redirect
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return Redirect("/Home");
                }
                if (result.IsLockedOut)
                {
                    ErrorMessage = "Account is locked out.";
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        _logger.LogWarning("ModelState error: {Error}", error.ErrorMessage);
                    }
                }
                ErrorMessage = "Please check your input and try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during login");
            ErrorMessage = $"Login error: {ex.Message}";
        }

        _logger.LogInformation("Returning to Page with error: {Error}", ErrorMessage);
        return Page();
    }
}

