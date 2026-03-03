using KMKKM.PortalClient.Infrastructure.Identity;
using KMKKM.PortalClient.Infrastructure.Options;
using KMKKM.PortalClient.Infrastructure.Services;
using KMKKM.PortalClient.Web.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;

namespace KMKKM.PortalClient.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISecurityAuditService _securityAuditService;
    private readonly PasswordResetOptions _passwordResetOptions;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        ISecurityAuditService securityAuditService,
        IOptions<PasswordResetOptions> passwordResetOptions,
        ILogger<AccountController> logger)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _securityAuditService = securityAuditService;
        _passwordResetOptions = passwordResetOptions.Value;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || !user.IsActive)
        {
            _securityAuditService.Write("sign_in_denied", false, model.Email, details: "Unknown or inactive account.");
            ModelState.AddModelError(string.Empty, "Usuario ou senha invalidos.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            _securityAuditService.Write("sign_in_locked_out", false, user.Email, user.Id, "Account locked after repeated failures.");
            ModelState.AddModelError(string.Empty, "Conta temporariamente bloqueada por tentativas invalidas. Tente novamente mais tarde ou redefina sua senha.");
            return View(model);
        }

        if (!result.Succeeded)
        {
            _securityAuditService.Write("sign_in_failed", false, user.Email, user.Id, "Invalid password.");
            ModelState.AddModelError(string.Empty, "Usuario ou senha invalidos.");
            return View(model);
        }

        _securityAuditService.Write("sign_in_succeeded", true, user.Email, user.Id);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        _securityAuditService.Write("sign_out", true, User.Identity?.Name);
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!_passwordResetOptions.Enabled)
        {
            ViewBag.ResetFlowDisabled = true;
            return View("ForgotPasswordConfirmation");
        }

        string? resetLink = null;
        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user is not null && user.IsActive)
        {
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            resetLink = BuildResetLink(user.Email!, encodedToken);
            _logger.LogInformation("Password reset link generated for {Email}: {ResetLink}", user.Email, resetLink);
            _securityAuditService.Write("password_reset_requested", true, user.Email, user.Id);
        }
        else
        {
            _securityAuditService.Write("password_reset_requested", false, model.Email, details: "Unknown or inactive account.");
        }

        ViewBag.ResetLink = _passwordResetOptions.ExposeResetLinkInResponse ? resetLink : null;
        return View("ForgotPasswordConfirmation");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPassword(string? email = null, string? token = null)
    {
        if (!_passwordResetOptions.Enabled || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        return View(new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!_passwordResetOptions.Enabled)
        {
            return RedirectToAction(nameof(ForgotPassword));
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || !user.IsActive)
        {
            _securityAuditService.Write("password_reset_completed", false, model.Email, details: "Unknown or inactive account.");
            return View("ResetPasswordConfirmation");
        }

        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
        }
        catch (FormatException)
        {
            _securityAuditService.Write("password_reset_completed", false, model.Email, user.Id, "Invalid token format.");
            ModelState.AddModelError(string.Empty, "O link de redefinicao e invalido.");
            return View(model);
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            _securityAuditService.Write("password_reset_completed", false, user.Email, user.Id, string.Join("; ", result.Errors.Select(x => x.Description)));
            return View(model);
        }

        await _userManager.UpdateSecurityStampAsync(user);
        _securityAuditService.Write("password_reset_completed", true, user.Email, user.Id);
        return View("ResetPasswordConfirmation");
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ApplicationUser? user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            _securityAuditService.Write("password_changed", false, user.Email, user.Id, string.Join("; ", result.Errors.Select(x => x.Description)));
            return View(model);
        }

        await _signInManager.RefreshSignInAsync(user);
        _securityAuditService.Write("password_changed", true, user.Email, user.Id);
        ViewBag.PasswordChanged = true;

        return View(new ChangePasswordViewModel());
    }

    [HttpGet]
    [Authorize]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private string BuildResetLink(string email, string encodedToken)
    {
        string? origin = string.IsNullOrWhiteSpace(_passwordResetOptions.PublicOrigin)
            ? null
            : _passwordResetOptions.PublicOrigin.TrimEnd('/');

        if (!string.IsNullOrWhiteSpace(origin))
        {
            return $"{origin}/Account/ResetPassword?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(encodedToken)}";
        }

        return Url.Action(
                   nameof(ResetPassword),
                   "Account",
                   new { email, token = encodedToken },
                   Request.Scheme,
                   Request.Host.ToUriComponent())
               ?? string.Empty;
    }
}
