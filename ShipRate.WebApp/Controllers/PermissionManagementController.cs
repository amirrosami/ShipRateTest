using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Infrastructure.Data;
using ShipRate.Web.ViewModels.PermissionManagement;

namespace ShipRate.Web.Controllers
{
    //[Authorize(Policy = "Permissions.Manage")] // یا Role = "Admin"
    [Authorize(Roles = "Admin")]
    public class PermissionManagementController : Controller
    {
        private readonly IPermissionManagementService _permissionService;
        private readonly ILogger<PermissionManagementController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;


        public PermissionManagementController(
            IPermissionManagementService permissionService,
            ILogger<PermissionManagementController> logger,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _permissionService = permissionService;
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }

        /// <summary>
        /// GET: لیست کاربران
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var users = await _permissionService.GetAllUsersAsync();
                var modelUsers = MapUserListItemsToViewModel(users);
                return View(modelUsers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in users fetching");
                TempData["Error"] = "Error in users fetching";
                return View(new List<UserListItemViewModel>());
            }
        }

        /// <summary>
        /// GET: صفحه مدیریت دسترسی‌های یک کاربر
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ManagePermissions(Guid id)
        {
            try
            {

                var userPermissions = await _permissionService.GetUserPermissionsAsync(id);
                var viewModel = MapToViewModel(userPermissions);

                var assignedGroupIds = await _db.UserPermissionGroups
                    .Where(x => x.UserId == id && !x.IsDeleted)
                    .Select(x => x.PermissionGroupId)
                    .ToListAsync();

                viewModel.UserPermissionGroups = await _db.PermissionGroups
                    .AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AssignablePermissionGroupViewModel
                    {
                        PermissionGroupId = x.Id,
                        GroupName = x.Name,
                        Description = x.Description,
                        IsAssigned = assignedGroupIds.Contains(x.Id)
                    })
                    .ToListAsync();



                if (viewModel == null)
                {
                    return NotFound();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errors in Permissions load for User :{UserId}", id);
                TempData["Error"] = "Errors in Permissions load for User";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: ذخیره تغییرات دسترسی‌ها
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPermissions(AssignPermissionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invali data";
                return RedirectToAction(nameof(ManagePermissions), new { id = model.UserId });
            }

            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["Error"] = "The User is not Authenticated";
                    return RedirectToAction(nameof(Index));
                }

                var changesCount = await _permissionService.AssignPermissionsAsync(
                    model.UserId,
                    model.SelectedPermissionIds,
                    model.UserId
                );



                changesCount += await _permissionService.AssignPermissionGroupsAsync(
                        model.UserId,
                        model.SelectedPermissionGroupIds ?? [],
                        Guid.Parse(currentUserId));

                if (changesCount > 0)
                {
                    TempData["Success"] = $"{changesCount} Changes applied successfuly";
                    var user = await _userManager.FindByIdAsync(model.UserId.ToString());

                    if (user == null)
                    {
                        throw new InvalidOperationException("User not found.");
                    }

                    // 5) تغییر SecurityStamp برای بی‌اعتبار کردن Cookie قبلی
                    var stampResult = await _userManager.UpdateSecurityStampAsync(user);
                }
                else
                {
                    TempData["Info"] = "No Changes applied!!!";
                }




                return RedirectToAction(nameof(ManagePermissions), new { id = model.UserId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ذخیره دسترسی‌های کاربر {UserId}", model.UserId);
                TempData["Error"] = "خطا در ذخیره تغییرات";
                return RedirectToAction(nameof(ManagePermissions), new { id = model.UserId });
            }
        }

        /// <summary>
        /// POST: لغو تمام دسترسی‌های یک کاربر
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RevokeAll(Guid id)
        {


            try
            {
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(currentUserId))
                {
                    TempData["Error"] = "The User is not authenticated";
                    return RedirectToAction(nameof(Index));
                }

                var revokedCount = await _permissionService.RevokeAllPermissionsAsync(id, currentUserId);

                if (revokedCount > 0)
                {
                    TempData["Success"] = $"{revokedCount} Access canceled successfuly!!!!!";
                }
                else
                {
                    TempData["Info"] = "The user has not any acrtive permission";
                }

                return RedirectToAction(nameof(ManagePermissions), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Cancel permissions for User :{UserId}", id);
                TempData["Error"] = "Error in Cancel permissions";
                return RedirectToAction(nameof(ManagePermissions), new { id });
            }
        }

        public List<UserListItemViewModel> MapUserListItemsToViewModel(List<UserListItemDto> list)
        {
            var newList = new List<UserListItemViewModel>();
            foreach (var item in list)
            {
                newList.Add(
                    new UserListItemViewModel
                    {
                        Email = item.Email,
                        FullName = item.FullName,
                        IsActive = item.IsActive,
                        PermissionsCount = item.PermissionsCount,
                        UserId = item.UserId,
                        UserName = item.UserName
                    }
                    );
            }
            return newList;
        }

        public List<UserPermissionViewModel> MapUserPermissionsToViewModel(List<UserPermissionsDto> list)
        {
            var newList = new List<UserPermissionViewModel>();
            foreach (var item in list)
            {
                newList.Add(
                    new UserPermissionViewModel
                    {
                        Email = item.Email,
                        UserId = item.UserId,
                        UserName = item.UserName,
                        PermissionGroups = null
                    }
                    );
            }
            return newList;
        }
        public List<PermissionItemViewModel> MapPermissionsListToViewModel(List<PermissionDto> list)
        {
            var newList = new List<PermissionItemViewModel>();
            foreach (var item in list)
            {
                newList.Add(
                    new PermissionItemViewModel
                    {
                        DisplayName = item.DisplayName,
                        Description = item.Description,
                        //IsActive = item.IsActive,
                        Name = item.Name,
                    }
                    );
            }
            return newList;
        }


        private AssignPermissionsViewModel MapToViewModel(UserPermissionsDto dto)
        {
            var viewModel = new AssignPermissionsViewModel
            {
                UserId = dto.UserId,
                UserName = dto.UserName,
                Email = dto.Email,
                SelectedPermissionIds = dto.AssignedPermissionIds
            };

            // گروه‌بندی دسترسی‌ها براساس Category
            var groups = dto.AllPermissions
                .GroupBy(p => p.Category ?? "سایر")
                .Select(g => new PermissionGroupViewModel
                {
                    GroupName = g.Key,
                    Permissions = g.Select(p => new PermissionItemViewModel
                    {
                        PermissionId = p.Id,
                        Name = p.Name,
                        DisplayName = p.DisplayName,
                        Description = p.Description,
                        IsAssigned = dto.AssignedPermissionIds.Contains(p.Id)
                    }).ToList()
                }).ToList();

            viewModel.PermissionGroups = groups;

            return viewModel;
        }
    }
}