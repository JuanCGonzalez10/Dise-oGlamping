using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationsService _reservationsService;
        private readonly ICombosHelper _combosHelper;
        private readonly INotyfService _notyf;
        private readonly IUsersService _usersService;

        public ReservationsController(
            IReservationsService reservationsService,
            ICombosHelper combosHelper,
            INotyfService notyf,
            IUsersService usersService)
        {
            _reservationsService = reservationsService;
            _combosHelper = combosHelper;
            _notyf = notyf;
            _usersService = usersService;
        }

        // =====================================================
        // INDEX
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "ShowReservation", module: "Reservation")]
        [Authorize()]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<ReservationDTO>> response =
                await _reservationsService.GetPaginationAsync(request);

            return View(response.Result);
        }

        // =====================================================
        // CREATE (GET)
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "CreateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ReservationDTO dto = new ReservationDTO
            {
                Products = await _combosHelper.GetComboProducts(),
                Users = await _combosHelper.GetComboUsers()
            };

            return View(dto);
        }

        // =====================================================
        // CREATE (POST)
        // =====================================================
        [HttpPost]
        [CustomAuthorize(permission: "CreateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación.");

                dto.Products = await _combosHelper.GetComboProducts();
                dto.Users = await _combosHelper.GetComboUsers();

                return View(dto);
            }

            Response<ReservationDTO> response = await _reservationsService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error(response.Message);

            dto.Products = await _combosHelper.GetComboProducts();
            dto.Users = await _combosHelper.GetComboUsers();

            return View(dto);
        }

        // =====================================================
        // EDIT (GET)
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "UpdateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            Response<ReservationDTO> response = await _reservationsService.GetOneAsync(id);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message);
                return RedirectToAction(nameof(Index));
            }

            ReservationDTO dto = response.Result;

            // Cargar combos
            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(dto.UserId);

            return View(dto);
        }

        // =====================================================
        // EDIT (POST)
        // =====================================================
        [HttpPost]
        [CustomAuthorize(permission: "UpdateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Edit(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación.");

                dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
                dto.Users = await _combosHelper.GetComboUsers(dto.UserId);

                return View(dto);
            }

            Response<ReservationDTO> response = await _reservationsService.EditAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error(response.Message);

            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(dto.UserId);

            return View(dto);
        }

        // =====================================================
        // DELETE
        // =====================================================
        [HttpPost]
        [CustomAuthorize(permission: "DeleteReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Response<object> response = await _reservationsService.DeleteAsync(id);

            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
            }
            else
            {
                _notyf.Error(response.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CREATE FROM PRODUCT (GET) – para rol CLIENTE
        // =====================================================
        // =====================================================
        // CREATE FROM PRODUCT (GET) – Para rol CLIENTE
        // =====================================================
        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateFromProduct(int productId)
        {
            // Usuario logueado
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                _notyf.Error("No se pudo identificar al usuario.");
                return RedirectToAction("Index", "Home");
            }

            // Cargar combos, pero con product y user fijados
            var dto = new ReservationDTO
            {
                ProductId = productId,
                UserId = user.Id,
                Products = await _combosHelper.GetComboProducts(productId),  // bloqueado en vista
                Users = await _combosHelper.GetComboUsers(user.Id),          // bloqueado en vista
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(1)
            };

            return View(dto);
        }


        // =====================================================
        // CREATE FROM PRODUCT (POST)
        // =====================================================
        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CreateFromProduct(ReservationDTO dto)
        {
            // Seguridad: el UserId NO debe venir del cliente
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                _notyf.Error("Usuario inválido.");
                return RedirectToAction(nameof(Index));
            }

            dto.UserId = user.Id;

            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe corregir los errores.");
                dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
                dto.Users = await _combosHelper.GetComboUsers(user.Id);
                return View(dto);
            }

            var response = await _reservationsService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success("Reserva creada correctamente.");
                return RedirectToAction(nameof(Index)); // listado del cliente
            }

            _notyf.Error(response.Message);

            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(user.Id);

            return View(dto);
        }


    }
}
