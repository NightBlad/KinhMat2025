using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Shopping_Cart_2.Models;
using Shopping_Cart_2.ViewModels;

public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    public IActionResult Create()
    {
        ViewBag.Roles = _roleManager.Roles.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.UserName, Email = model.Email, EmailConfirmed = model.EmailConfirmed };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.Role))
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        ViewBag.Roles = _roleManager.Roles.ToList();
        return View(model);
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var model = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Role = userRoles.FirstOrDefault()
        };

        ViewBag.Roles = _roleManager.Roles.ToList();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.EmailConfirmed = model.EmailConfirmed;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (!passwordChangeResult.Succeeded)
                    {
                        foreach (var error in passwordChangeResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        ViewBag.Roles = _roleManager.Roles.ToList();
                        return View(model);
                    }
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                if (!string.IsNullOrEmpty(model.Role) && !userRoles.Contains(model.Role))
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, model.Role);
                }
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        ViewBag.Roles = _roleManager.Roles.ToList();
        return View(model);
    }


    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(user);
    }
}
