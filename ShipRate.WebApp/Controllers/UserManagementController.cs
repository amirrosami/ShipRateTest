using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.Interfaces;
using ShipRate.Application.Utilities;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using ShipRate.Web.Models;
using System.Linq.Dynamic.Core;

namespace ShipRate.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _db = db;
            _currentUserService = currentUserService;
        }

        // ========== لیست کاربران ==========
        public async Task<IActionResult> Index(int companyId, int? status, Guid roleId, int page = 1, int pageSize = 10, string search = "", int userTypeId = 0)
        {
            var query = _userManager.Users
                .Include(x => x.UserType)
                .Include(x => x.Person)
                .AsQueryable();
            query = query.Where(x => !x.IsSystem);
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.UserName.Contains(search) || x.Email.Contains(search)
                || x.FullName.Contains(search) || x.Person.PersonName.Contains(search) || x.JobTitle.Contains(search) || x.Mobile.Contains(search) || x.PhoneNumber.Contains(search) || x.SalesFilePrefix.Contains(search)
                );
            }
            if (status != null)
            {
                query = query.Where(x => x.IsActive == (status == 1 ? true : false));
            }
            if (roleId != Guid.Empty)
            {
                query = query.Where(x => _db.UserRoles.Any(r => r.UserId == x.Id && r.RoleId == roleId));
            }
            if (companyId != 0)
            {
                query = query.Where(x => _db.UserCompanies.Any(c => c.UserId == x.Id && c.CompanyId == companyId));
            }
            if (userTypeId != 0)
            {
                query = query.Where(x => x.UserTypeId == userTypeId);
            }

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnd = user.LockoutEnd,
                    UserTypeName = user.UserType?.UserTypeName,
                    PersonName = user.Person?.PersonName,
                    JobTitle = user.JobTitle,
                    SalesFilePrefix = user.SalesFilePrefix
                });
            }
            ViewBag.Search = search;
            var pageResult = new WebApp.ViewModels.PagedResult<UserViewModel>
            {
                Items = userViewModels,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
            };
            ViewBag.UserTypes = _db.UserTypes
                .Select(x =>
                new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserTypeName
                }).ToList();






            ViewBag.SelectedUserType = userTypeId;


            var userroles = await _roleManager.Roles.ToListAsync();
            ViewBag.Roles = userroles.Select(x =>
                new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
            ViewBag.SelectedRole = roleId;

            ViewBag.Companies = await _db.Companies
                    .Include(x => x.Person)
                    .OrderBy(t => t.Id)
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Person.PersonName })
                    .ToListAsync();
            ViewBag.SelectedCompany = companyId;
            ViewBag.SelectedStatus = status;
            return View(pageResult);
        }

        // ========== ایجاد کاربر جدید (GET) ==========
        [HttpGet]
        public async Task<IActionResult> Create()
        {

            //var selectedCompanyIds = await _db.UserCompanies
            //    .Where(ct => ct.CustomerId == id && !ct.IsDeleted)
            //    .Select(ct => ct.TradeTypeId)
            //    .ToListAsync();

            var selectedCompanyIds = new List<int>();
            var vm = new UserFormViewModel();
            vm.IsEditMode = false;
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            vm.UserTypes = _db.UserTypes
                .Select(x =>
                new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserTypeName
                }).ToList();

            vm.Companies = await _db.Companies
                    .Include(x => x.Person)
                    .OrderBy(t => t.Id)
                    .Select(t => new CompanyDropdownItem { Id = t.Id, Name = t.Person.PersonName })
                    .ToListAsync();

            vm.TwoFactorMethods = await GetTwoFactorMethodsAsync();
            return View(vm);
        }
        // ========== ایجاد کاربر جدید (POST) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {

            model.IsEditMode = false;
            model.UserTypes = await GetUserTypesAsync();
            model.Companies = await GetCompaniesAsync();
            ViewBag.Roles = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), "Password is required.");
            }

            if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                ModelState.AddModelError(nameof(model.ConfirmPassword), "Confirm password is required.");
            }


            // اعتبارسنجی عکس
            if (model.PicFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(model.PicFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.PicFile), "Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");
                }

                if (model.PicFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(model.PicFile), "Image size must be less than 2 MB.");
                }
            }

            var picBytes = await PictureHelper.ConvertToBytesAsync(model.PicFile);



            if (string.IsNullOrWhiteSpace(model.Mobile) && model.TwoFactorEnabled && model.TwoFactorMethodId == (int)TwoFactorMethodEnum.Sms)
            {
                ModelState.AddModelError(nameof(model.Mobile), "For SMS Mobile is Required");
            }






            if (!ModelState.IsValid)
                return View(model);


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    FirstName = model.FullName,
                    IsActive = true,
                    EmailConfirmed = true,
                    LastName = model.FullName,
                    UserTypeId = model.UserTypeId,
                    PersonId = model.PersonId,
                    CreatedAt = DateTime.Now,
                    JobTitle = model.JobTitle,
                    SalesFilePrefix = model.SalesFilePrefix,
                    PhoneNumber = model.PhoneNumber,
                    Mobile = model.Mobile
                };

                if (model.TwoFactorMethodId == (int)TwoFactorMethodEnum.None)
                {
                    user.TwoFactorEnabled = false;
                    user.TwoFactorMethodId = null;
                }
                else
                {
                    user.TwoFactorEnabled = model.TwoFactorEnabled;
                    user.TwoFactorMethodId = model.TwoFactorMethodId;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);



                    foreach (var item in model.SelectedCompanyIds)
                    {
                        var usercompany = new UserCompany
                        {
                            CompanyId = item,
                            UserId = user.Id,
                            CreateUserId = _currentUserService.UserId,
                            CreatedAt = DateTime.Now
                        };
                        _db.UserCompanies.Add(usercompany);
                    }
                    _db.SaveChanges();
                    TempData["Success"] = $"User {user.UserName} Created Successfuly.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            //var selectedRole = roles.FirstOrDefault();





            var selectedCompanyIds = await _db.UserCompanies
                .Where(x => x.UserId == user.Id)
                .Select(x => x.CompanyId)
                .ToListAsync();

            var personName = await _db.Persons
                .Where(x => x.Id == user.PersonId)
                .Select(x => x.PersonName)
                .FirstOrDefaultAsync();

            var model = new UserFormViewModel
            {
                Id = user.Id,
                IsEditMode = true,
                UserTypeId = user.UserTypeId.Value,
                PersonId = user.PersonId.Value,
                PersonName = personName,
                UserName = user.UserName ?? "",
                Email = user.Email,
                FullName = user.FullName ?? "",
                SelectedRoles = roles.ToList(),
                //RoleName = selectedRole,
                SelectedCompanyIds = selectedCompanyIds,
                JobTitle = user.JobTitle,
                SalesFilePrefix = user.SalesFilePrefix,
                UserTypes = await GetUserTypesAsync(),
                Companies = await GetCompaniesAsync(),
                TwoFactorMethods = await GetTwoFactorMethodsAsync(),
                ExistingPicBase64 = user.Pic != null ? Convert.ToBase64String(user.Pic) : null,
                PhoneNumber = user.PhoneNumber,
                Mobile = user.Mobile,
                TwoFactorMethodId = user.TwoFactorMethodId,
                TwoFactorEnabled = user.TwoFactorEnabled,
            };
            ViewBag.Roles = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();
            return View(model);
        }
        private async Task<List<SelectListItem>> GetUserTypesAsync()
        {
            return _db.UserTypes
                .Select(x =>
                new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.UserTypeName
                }).ToList();
        }
        private async Task<List<SelectListItem>> GetTwoFactorMethodsAsync()
        {
            //return _db.TwoFactorMethods
            //    .Select(x =>
            //    new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.TwoFactorMethodName
            //    }).ToList();
            return new List<SelectListItem>
                                {
                                    new()
                                    {
                                    Value = ((int)TwoFactorMethodEnum.None).ToString(),
                                    Text = "Disabled"
                                    },
                                    new()
                                    {
                                    Value = ((int)TwoFactorMethodEnum.Sms).ToString(),
                                    Text = "SMS"
                                    },
                                    new()
                                    {
                                    Value = ((int)TwoFactorMethodEnum.GoogleAuthenticator).ToString(),
                                    Text = "Google Authenticator",
                                    },
                                    new()
                                    {
                                    Value = ((int)TwoFactorMethodEnum.Email).ToString(),
                                    Text = "Email",
                                    Disabled = true
                                    }
                                };
        }
        private async Task<List<CompanyDropdownItem>> GetCompaniesAsync()
        {
            return await _db.Companies
                    .Include(x => x.Person)
                    .OrderBy(t => t.Id)
                    .Select(t => new CompanyDropdownItem
                    {
                        Id = t.Id,
                        Name = t.Person.PersonName
                    })
                    .ToListAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            model.IsEditMode = true;
            model.UserTypes = await GetUserTypesAsync();
            model.Companies = await GetCompaniesAsync();
            ViewBag.Roles = await _roleManager.Roles.OrderBy(x => x.Name).ToListAsync();


            ModelState.Remove(nameof(model.Password));
            ModelState.Remove(nameof(model.ConfirmPassword));


            if (model.PicFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(model.PicFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(model.PicFile), "Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");
                }

                if (model.PicFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(model.PicFile), "Image size must be less than 2 MB.");
                }
            }

            if (string.IsNullOrWhiteSpace(model.Mobile) && model.TwoFactorEnabled && model.TwoFactorMethodId == (int)TwoFactorMethodEnum.Sms)
            {
                ModelState.AddModelError(nameof(model.Mobile), "For SMS Mobile is Required");
            }





            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id.ToString());

            if (user == null)
                return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.FullName = model.FullName;
            user.PersonId = model.PersonId;
            user.UserTypeId = model.UserTypeId;
            user.JobTitle = model.JobTitle;
            user.SalesFilePrefix = model.SalesFilePrefix;
            user.PhoneNumber = model.PhoneNumber;
            user.Mobile = model.Mobile;


            if (model.RemovePic)
            {
                user.Pic = null;
            }
            else if (model.PicFile != null && model.PicFile.Length > 0)
            {
                user.Pic = await PictureHelper.ConvertToBytesAsync(model.PicFile);
            }

            if (model.TwoFactorMethodId == (int)TwoFactorMethodEnum.None)
            {
                user.TwoFactorEnabled = false;
                user.TwoFactorMethodId = null;
            }
            else
            {
                user.TwoFactorEnabled = model.TwoFactorEnabled;
                user.TwoFactorMethodId = model.TwoFactorMethodId;
            }
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                model.ExistingPicBase64 = user.Pic != null ? Convert.ToBase64String(user.Pic) : null;
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (model.SelectedRoles.Any())
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            }

            if (!string.IsNullOrWhiteSpace(model.RoleName))
            {
                await _userManager.AddToRoleAsync(user, model.RoleName);
            }

            var oldUserCompanies = await _db.UserCompanies
                .Where(x => x.UserId == user.Id)
                .ToListAsync();

            _db.UserCompanies.RemoveRange(oldUserCompanies);

            if (model.SelectedCompanyIds != null && model.SelectedCompanyIds.Any())
            {
                foreach (var companyId in model.SelectedCompanyIds.Distinct())
                {
                    _db.UserCompanies.Add(new UserCompany
                    {
                        UserId = user.Id,
                        CompanyId = companyId
                    });
                }
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }






        // ========== حذف کاربر ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // جلوگیری از حذف خود
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "You can not delete yourself!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "The User deleted successfully.";
            }
            else
            {
                TempData["Error"] = "Error in delete.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ========== غیرفعال/فعال کردن کاربر ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            var status = user.IsActive ? "فعال" : "غیرفعال";
            TempData["Success"] = $"کاربر {status} شد.";

            return RedirectToAction(nameof(Index));
        }

        // ========== تغییر رمز عبور ==========
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(new ResetPasswordViewModel { UserId = id, UserName = user.UserName ?? "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                    return NotFound();

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Password changed successfuly.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        // ========== مدیریت نقش‌ها ==========
        [HttpGet]
        public async Task<IActionResult> Roles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new ApplicationRole(roleName));
                    TempData["Success"] = $"Role {roleName} Created Successfuly.";
                }
                else
                {
                    TempData["Error"] = "This Role already exists!!!!";
                }
            }

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                // جلوگیری از حذف نقش‌های پایه
                if (role.Name == "Admin" || role.Name == "Manager" || role.Name == "Sales Manager" || role.Name == "Sales User" || role.Name == "Customer" || role.Name == "External Party")
                {
                    TempData["Error"] = "Base Roles couldn't be deleted.";
                    return RedirectToAction(nameof(Roles));
                }

                await _roleManager.DeleteAsync(role);
                TempData["Success"] = "Role deleted successfuly.";
            }

            return RedirectToAction(nameof(Roles));
        }






        [HttpGet]
        public async Task<IActionResult> UserPhoto(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Redirect("~/images/no-pic.webp");

            var user = await _userManager.FindByIdAsync(id);

            if (user == null || user.Pic == null || user.Pic.Length == 0)
                return Redirect("~/images/no-pic.webp");

            var contentType = GetImageContentType(user.Pic);

            return File(user.Pic, contentType);
        }

        private static string GetImageContentType(byte[] imageBytes)
        {
            if (imageBytes.Length >= 4 &&
                imageBytes[0] == 0x89 &&
                imageBytes[1] == 0x50 &&
                imageBytes[2] == 0x4E &&
                imageBytes[3] == 0x47)
            {
                return "image/png";
            }

            if (imageBytes.Length >= 3 &&
                imageBytes[0] == 0xFF &&
                imageBytes[1] == 0xD8 &&
                imageBytes[2] == 0xFF)
            {
                return "image/jpeg";
            }

            if (imageBytes.Length >= 6 &&
                imageBytes[0] == 0x47 &&
                imageBytes[1] == 0x49 &&
                imageBytes[2] == 0x46)
            {
                return "image/gif";
            }

            if (imageBytes.Length >= 12 &&
                imageBytes[8] == 0x57 &&
                imageBytes[9] == 0x45 &&
                imageBytes[10] == 0x42 &&
                imageBytes[11] == 0x50)
            {
                return "image/webp";
            }

            return "application/octet-stream";
        }








        // در کلاس UserManagementController این متدها را اضافه کنید:

        //public async Task<IActionResult> ManageClaims(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //    {
        //        TempData["Error"] = "کاربر یافت نشد";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    var userClaims = await _userManager.GetClaimsAsync(user);

        //    var model = new UserClaimsViewModel
        //    {
        //        UserId = user.Id,
        //        UserName = user.UserName ?? "",
        //        Claims = new List<ClaimItem>
        //{
        //    new ClaimItem
        //    {
        //        Type = AppClaims.CanEditCountries,
        //        DisplayName = "ویرایش کشورها",
        //        IsSelected = userClaims.Any(c => c.Type == AppClaims.CanEditCountries)
        //    },
        //    new ClaimItem
        //    {
        //        Type = AppClaims.CanDeleteCountries,
        //        DisplayName = "حذف کشورها",
        //        IsSelected = userClaims.Any(c => c.Type == AppClaims.CanDeleteCountries)
        //    },
        //    new ClaimItem
        //    {
        //        Type = AppClaims.CanEditCities,
        //        DisplayName = "ویرایش شهرها",
        //        IsSelected = userClaims.Any(c => c.Type == AppClaims.CanEditCities)
        //    },
        //    new ClaimItem
        //    {
        //        Type = AppClaims.CanDeleteCities,
        //        DisplayName = "حذف شهرها",
        //        IsSelected = userClaims.Any(c => c.Type == AppClaims.CanDeleteCities)
        //    },
        //    new ClaimItem
        //    {
        //        Type = AppClaims.CanViewReports,
        //        DisplayName = "مشاهده گزارش‌ها",
        //        IsSelected = userClaims.Any(c => c.Type == AppClaims.CanViewReports)
        //    }
        //}
        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //[Authorize(Policy = "CanManageUsers")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ManageClaims(UserClaimsViewModel model, List<string> selectedClaims)
        //{
        //    var user = await _userManager.FindByIdAsync(model.UserId);
        //    if (user == null)
        //    {
        //        TempData["Error"] = "کاربر یافت نشد";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // حذف تمام Claims فعلی
        //    var currentClaims = await _userManager.GetClaimsAsync(user);
        //    await _userManager.RemoveClaimsAsync(user, currentClaims);

        //    // افزودن Claims جدید
        //    if (selectedClaims != null && selectedClaims.Any())
        //    {
        //        foreach (var claimType in selectedClaims)
        //        {
        //            await _userManager.AddClaimAsync(user,
        //                new System.Security.Claims.Claim(claimType, AppClaims.TrueValue));
        //        }
        //    }

        //    TempData["Success"] = "دسترسی‌های کاربر با موفقیت به‌روزرسانی شد";
        //    return RedirectToAction(nameof(Index));
        //}




    }
}
