using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShipRate.Application.DTOs;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Security;
using ShipRate.Web.ViewModels.PermissionGroupsManagement;
namespace ShipRate.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PermissionGroupController : Controller
    {
        private readonly IPermissionGroupService _groupService;
        private readonly IPermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionGroupController(
            IPermissionGroupService groupService,
            IPermissionService permissionService,
            UserManager<ApplicationUser> userManager)
        {
            _groupService = groupService;
            _permissionService = permissionService;
            _userManager = userManager;
        }

        // GET: /PermissionGroup
        // [Authorize(Policy = "PermissionManagement.View")]
        public async Task<IActionResult> Index()
        {
            var groupDtos = await _groupService.GetAllGroupsAsync();

            var viewModels = groupDtos.Select(dto => new PermissionGroupListViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                PermissionCount = dto.PermissionCount
            }).ToList();

            return View(viewModels);
        }

        // GET: /PermissionGroup/Details/5
        // [Authorize(Policy = "PermissionManagement.View")]
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _groupService.GetGroupDetailAsync(id);
            if (dto == null)
                return NotFound();

            var viewModel = new PermissionGroupDetailViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                Permissions = dto.Permissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Category = p.Category
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /PermissionGroup/Create
        // [Authorize(Policy = "PermissionManagement.Manage")]
        public async Task<IActionResult> Create()
        {
            var allPermissions = await _permissionService.GetAllPermissionsAsync();

            var viewModel = new CreatePermissionGroupViewModel
            {
                AvailablePermissions = allPermissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Category = p.Category
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /PermissionGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Policy = "PermissionManagement.Manage")]
        public async Task<IActionResult> Create(CreatePermissionGroupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // بارگذاری مجدد لیست Permissions در صورت خطا
                var allPermissions = await _permissionService.GetAllPermissionsAsync();
                viewModel.AvailablePermissions = allPermissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Category = p.Category
                }).ToList();

                return View(viewModel);
            }

            var dto = new CreatePermissionGroupDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                PermissionIds = viewModel.SelectedPermissionIds
            };

            var success = await _groupService.CreateGroupAsync(dto);

            if (!success)
            {
                ModelState.AddModelError("", "Group name already exists!!!!");

                var allPermissions = await _permissionService.GetAllPermissionsAsync();
                viewModel.AvailablePermissions = allPermissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Category = p.Category
                }).ToList();

                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Permissions Group Created Succesfuly.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /PermissionGroup/Edit/5
        //  [Authorize(Policy = "PermissionManagement.Manage")]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _groupService.GetGroupDetailAsync(id);
            if (dto == null)
                return NotFound();

            var allPermissions = await _permissionService.GetAllPermissionsAsync();

            var viewModel = new EditPermissionGroupViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                SelectedPermissionIds = dto.Permissions.Select(p => p.Id).ToList(),
                AvailablePermissions = allPermissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Category = p.Category
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /PermissionGroup/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Policy = "PermissionManagement.Manage")]
        public async Task<IActionResult> Edit(EditPermissionGroupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var allPermissions = await _permissionService.GetAllPermissionsAsync();
                viewModel.AvailablePermissions = allPermissions.Select(p => new PermissionViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    DisplayName = p.DisplayName,
                    Category = p.Category
                }).ToList();

                return View(viewModel);
            }

            var dto = new UpdatePermissionGroupDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                IsActive = viewModel.IsActive,
                PermissionIds = viewModel.SelectedPermissionIds
            };

            var success = await _groupService.UpdateGroupAsync(dto);

            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Permission group updated successfuly!!!!!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /PermissionGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Policy = "PermissionManagement.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _groupService.DeleteGroupAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Permission Group Not Found!!!!";
            }
            else
            {
                TempData["SuccessMessage"] = "Permission Group Deleted Successfuly.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /PermissionGroup/ToggleStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // [Authorize(Policy = "PermissionManagement.Manage")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var success = await _groupService.ToggleGroupStatusAsync(id);

            if (!success)
            {
                TempData["ErrorMessage"] = "Permission Group Not Found";
            }
            else
            {
                TempData["SuccessMessage"] = "Status Changed Successfuly";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManageUsers(int id)
        {
            var group = await _groupService.GetGroupDetailAsync(id);
            if (group == null)
                return NotFound();

            // دریافت تمام کاربران
            var allUsers = _userManager.Users.ToList();

            // دریافت کاربران فعلی این Group
            var currentUsers = await _groupService.GetGroupUsersAsync(id);

            var viewModel = new AssignGroupToUserViewModel
            {
                PermissionGroupId = group.Id,
                PermissionGroupName = group.Name,
                AvailableUsers = allUsers.Select(u => new UserSelectViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? "",
                    Email = u.Email ?? ""
                }).ToList(),
                CurrentUsers = currentUsers.Select(cu => new UserGroupViewModel
                {
                    UserId = cu.UserId,
                    UserName = cu.UserName,
                    Email = cu.Email,
                    AssignedAt = cu.AssignedAt
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUser(int permissionGroupId, Guid userId)
        {
            var dto = new AssignPermissionGroupToUserDto
            {
                PermissionGroupId = permissionGroupId,
                UserId = userId
            };

            var result = await _groupService.AssignGroupToUserAsync(dto);

            if (result)
                TempData["SuccessMessage"] = "Permission Group assigned to user successfuly.";
            else
                TempData["ErrorMessage"] = "Error in Assign Group.";

            return RedirectToAction(nameof(ManageUsers), new { id = permissionGroupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUser(int permissionGroupId, Guid userId)
        {
            var result = await _groupService.RemoveGroupFromUserAsync(permissionGroupId, userId);

            if (result)
                TempData["SuccessMessage"] = "User removed from group.";
            else
                TempData["ErrorMessage"] = "Error in removing user from group.";

            return RedirectToAction(nameof(ManageUsers), new { id = permissionGroupId });
        }



    }
}
