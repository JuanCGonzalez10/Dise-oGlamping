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
        // INDEX (ADMIN)
        // =====================================================
        [HttpGet]
        [Authorize]  // <-- solo requiere estar logueado
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            var response = await _reservationsService.GetPaginationAsync(request);
            return View(response.Result);
        }

        // =====================================================
        // MY RESERVATIONS (TURISTA / CLIENTE)
        // =====================================================
        [HttpGet]
        [Authorize] // SIN PERMISOS – cualquier usuario loguea|do la ve
        [HttpGet]
        [Authorize]  // turista puede entrar
        public async Task<IActionResult> MyReservations([FromQuery] PaginationRequest request)
        {
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);

            if (user == null)
            {
                _notyf.Error("No se pudo identificar al usuario.");
                return RedirectToAction("Index", "Home");
            }

            var response = await _reservationsService.GetPaginationAsync(request);
            return View(response.Result);
        }


        // =====================================================
        // CREATE (ADMIN)
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "CreateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var productPricesMap = await _reservationsService.GetProductPricesMapAsync();

            var dto = new ReservationDTO
            {
                Products = await _combosHelper.GetComboProducts(),
                Users = await _combosHelper.GetComboUsers(),
                ProductPricesMap = productPricesMap,
                PaymentMethods = _combosHelper.GetComboPaymentMethods(),
                CheckIn = DateTime.Now.Date,
                CheckOut = DateTime.Now.Date.AddDays(1)
            };

            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "CreateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Status))
                dto.Status = "PendientePago";

            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación.");
                dto.Products = await _combosHelper.GetComboProducts();
                dto.Users = await _combosHelper.GetComboUsers();
                dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
                dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
                return View(dto);
            }

            var response = await _reservationsService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error(response.Message);
            dto.Products = await _combosHelper.GetComboProducts();
            dto.Users = await _combosHelper.GetComboUsers();
            dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
            dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
            return View(dto);
        }

        // =====================================================
        // EDIT (ADMIN)
        // =====================================================
        [HttpGet]
        //[CustomAuthorize(permission: "UpdateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _reservationsService.GetOneAsync(id);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message);
                return RedirectToAction(nameof(Index));
            }

            var dto = response.Result;
            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(dto.UserId);
            dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
            dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();

            return View(dto);
        }

        [HttpPost]
        //[CustomAuthorize(permission: "UpdateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Edit(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación.");
                dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
                dto.Users = await _combosHelper.GetComboUsers(dto.UserId);
                dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
                dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
                return View(dto);
            }

            var response = await _reservationsService.EditAsync(dto);
            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error(response.Message);
            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(dto.UserId);
            dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
            dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);

            return View(dto);
        }

        // =====================================================
        // DELETE (ADMIN + TURISTA)
        // =====================================================
        [HttpPost]
        [Authorize] // TURISTA puede eliminar lo suyo
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _reservationsService.DeleteAsync(id);

            if (response.IsSuccess)
                _notyf.Success(response.Message);
            else
                _notyf.Error(response.Message);

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CREATE FROM PRODUCT (GET) – CLIENTE / TURISTA
        // =====================================================
        [HttpGet]
        [Authorize]  // SIN CustomAuthorize, lo puede abrir cualquiera logueado
        public async Task<IActionResult> CreateFromProduct(int productId)
        {
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);

            if (user == null)
            {
                _notyf.Error("No se pudo identificar al usuario.");
                return RedirectToAction("Index", "Home");
            }

            var pricePerNight = await _reservationsService.GetProductPriceAsync(productId);

            if (!pricePerNight.HasValue || pricePerNight.Value <= 0)
            {
                _notyf.Error("El glamping seleccionado no tiene un precio válido.");
                return RedirectToAction("Index", "Products");
            }

            var dto = new ReservationDTO
            {
                ProductId = productId,
                UserId = user.Id,
                Products = await _combosHelper.GetComboProducts(productId),
                Users = await _combosHelper.GetComboUsers(user.Id),
                PricePerNight = pricePerNight.Value,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(1),
                PaymentMethod = "Transferencia Cliente (Por Defecto)"
            };

            return View(dto);
        }

        // =====================================================
        // CREATE FROM PRODUCT (POST)
        // =====================================================
        [HttpPost]
        [Authorize] // SIN permisos, cualquiera logueado puede reservar
        public async Task<IActionResult> CreateFromProduct(ReservationDTO dto)
        {
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);

            if (user == null)
            {
                _notyf.Error("Usuario inválido.");
                return RedirectToAction(nameof(MyReservations));
            }

            dto.UserId = user.Id;

            var pricePerNight = await _reservationsService.GetProductPriceAsync(dto.ProductId);

            if (!pricePerNight.HasValue || pricePerNight.Value <= 0)
            {
                _notyf.Error("El glamping no tiene un precio válido.");
                dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
                dto.Users = await _combosHelper.GetComboUsers(user.Id);
                return View(dto);
            }

            dto.PricePerNight = pricePerNight.Value;

            if (string.IsNullOrEmpty(dto.PaymentMethod))
                dto.PaymentMethod = "Transferencia Cliente (Por Defecto)";

            if (string.IsNullOrEmpty(dto.Status))
                dto.Status = "PendientePago";

            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe corregir los errores.");
                dto.PricePerNight = pricePerNight.Value;
                dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
                dto.Users = await _combosHelper.GetComboUsers(user.Id);
                return View(dto);
            }

            var response = await _reservationsService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success("Reserva creada correctamente.");
                return RedirectToAction("Index", "Reservations");
            }

            _notyf.Error(response.Message);
            dto.PricePerNight = pricePerNight.Value;
            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(user.Id);

            return View(dto);
        }
    }
}
