using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;


namespace OmniPyme.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService _usersService;
        private readonly INotyfService _notifyService;
        private readonly ICombosHelper _combosHelper;
        private readonly IMapper _mapper;

        public UsersController(IUsersService sectionsService, INotyfService notifyService, ICombosHelper combosHelper, IMapper mapper)
        {
            _usersService = sectionsService;
            _notifyService = notifyService;
            _combosHelper = combosHelper;
            _mapper = mapper;
        }

        [HttpGet]
        [CustomAuthorize(permission: "ShowUsers", module: "Users")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<UsersDTO>> response = await _usersService.GetPaginationAsync(request);
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "CreateUsers", module: "Users")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            IEnumerable<SelectListItem> items = await _combosHelper.GetComboRoles();

            UsersDTO dto = new UsersDTO
            {
                privateURoles = items,
            };

            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "CreateUsers", module: "Users")]
        [Authorize]
        public async Task<IActionResult> Create(UsersDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                dto.privateURoles = await _combosHelper.GetComboRoles();
                return View(dto);
            }

            Response<UsersDTO> response = await _usersService.CreateAsync(dto);

            if (!response.IsSuccess)
            {
                _notifyService.Error(response.Message);
                dto.privateURoles = await _combosHelper.GetComboRoles();
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [CustomAuthorize(permission: "UpdateUsers", module: "Users")]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (Guid.Empty.Equals(id))
            {
                return NotFound();
            }
            Users user = await _usersService.GetUserAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            UsersDTO dto = _mapper.Map<UsersDTO>(user);
            dto.privateURoles = await _combosHelper.GetComboRoles();

            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "UpdateUsers", module: "Users")]
        [Authorize]
        public async Task<IActionResult> Edit(UsersDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notifyService.Error("Debe ajustar los errores de validación");
                dto.privateURoles = await _combosHelper.GetComboRoles();
                return View(dto);
            }

            Response<UsersDTO> response = await _usersService.UpdateUserAsync(dto);

            if (!response.IsSuccess)
            {
                _notifyService.Error(response.Message);
                dto.privateURoles = await _combosHelper.GetComboRoles();
                return View(dto);
            }

            _notifyService.Success(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [CustomAuthorize(permission: "DeleteUsers", module: "Users")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
            {
                _notifyService.Error("ID de usuario inválido.");
                return RedirectToAction(nameof(Index));
            }
            //Validar que haya por lo menos un administrador
            Users userToDelete = await _usersService.GetUserAsync(userId);
            if (userToDelete == null)
            {
                _notifyService.Error("Usuario no encontrado.");
                return RedirectToAction(nameof(Index));
            }

            // Validar si es administrador y si hay más de uno
            if (userToDelete.PrivateURole?.Name == "Admin") // Ajusta si usas una propiedad diferente
            {
                var adminCount = await _usersService.CountByRoleAsync("Admin");
                if (adminCount <= 1)
                {
                    _notifyService.Error("Debe haber al menos un administrador en el sistema.");
                    return RedirectToAction(nameof(Index));
                }
            }


            //proceder a la eliminacion
            Response<object> response = await _usersService.DeleteAsync(id);

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