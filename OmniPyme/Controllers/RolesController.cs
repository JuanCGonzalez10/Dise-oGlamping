using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;
using static OmniPyme.Web.DTOs.PrivateURoleDTO;

namespace OmniPyme.Web.Controllers
{
    public class RolesController : Controller
    {
        private readonly IRolesService _rolesService;
        private readonly INotyfService _notifyService;

        public RolesController(IRolesService rolesService, INotyfService notifyService)
        {
            _rolesService = rolesService;
            _notifyService = notifyService;
        }

        [HttpGet]
        [CustomAuthorize(permission: "showRoles", module: "Roles")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {

            Response<PaginationResponse<PrivateURoleDTO>> response = await _rolesService.GetPaginationAsync(request);
            if (!response.IsSuccess)
            {
                _notifyService.Error(response.Message);
                return RedirectToAction("Index", "Home");
            }
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "createRoles", module: "Roles")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            Response<List<PermissionDTO>> permissionsResponse = await _rolesService.GetPermissionsAsync();

            if (!permissionsResponse.IsSuccess)
            {
                _notifyService.Error(permissionsResponse.Message);
                return RedirectToAction(nameof(Index));
            }



            PrivateURoleDTO dto = new PrivateURoleDTO
            {
                Permissions = permissionsResponse.Result.Select(p => new PermissionForRoleDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Module = p.Module,
                    Selected = false
                }).ToList(),


            };

            return View(dto);
        }



        [HttpPost]
        [CustomAuthorize(permission: "createRoles", module: "Roles")]
        [Authorize]
        public async Task<IActionResult> Create(PrivateURoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");

                Response<List<PermissionDTO>> permissionResponse1 = await _rolesService.GetPermissionsAsync();

                dto.Permissions = permissionResponse1.Result.Select(p => new PermissionForRoleDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Module = p.Module,
                    Selected = false,
                }).ToList();



                return View(dto);
            }

            Response<PrivateURoleDTO> createResponse = await _rolesService.CreateAsync(dto);

            if (createResponse.IsSuccess)
            {
                _notifyService.Success(createResponse.Message);
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error(createResponse.Message);

            Response<List<PermissionDTO>> pemrissionResponse2 = await _rolesService.GetPermissionsAsync();

            dto.Permissions = pemrissionResponse2.Result.Select(p => new PermissionForRoleDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Module = p.Module,
            }).ToList();



            return View(dto);
        }


        [HttpPost]
        [CustomAuthorize(permission: "updateRoles", module: "Roles")]
        [Authorize]
        public async Task<IActionResult> Edit(PrivateURoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");

                Response<List<PermissionForRoleDTO>> permissionsByRoleResponse = await _rolesService.GetPermissionsByRoleAsync(dto.Id);
                dto.Permissions = permissionsByRoleResponse.Result.ToList();

                return View(dto);
            }

            Response<PrivateURoleDTO> editResponse = await _rolesService.EditAsync(dto);

            if (editResponse.IsSuccess)
            {
                _notifyService.Success(editResponse.Message);
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error(editResponse.Message);

            Response<List<PermissionForRoleDTO>> permissionsByRoleResponse2 = await _rolesService.GetPermissionsByRoleAsync(dto.Id);
            dto.Permissions = permissionsByRoleResponse2.Result.ToList();

            return View(dto);
        }

        [HttpGet]
        [CustomAuthorize(permission: "updateRoles", module: "Roles")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            Response<PrivateURoleDTO> response = await _rolesService.GetOneAsync(id);

            if (!response.IsSuccess)
            {
                _notifyService.Error(response.Message);
                return RedirectToAction(nameof(Index));
            }

            return View(response.Result);
        }


        [HttpPost]
        [CustomAuthorize(permission: "DeleteRoles", module: "Roles")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Response<object> response = await _rolesService.DeleteAsync(id);

            if (response.IsSuccess)
            {
                _notifyService.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notifyService.Error(response.Message);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
