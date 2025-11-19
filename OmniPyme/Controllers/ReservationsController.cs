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
        // INDEX (GENERAL / ADMINISTRATIVO)
        // Permiso: ShowReservation
        // Muestra todas las reservas a usuarios con este permiso (ej. Admin, Gerente).
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
        // MY RESERVATIONS (CLIENTE / TURISTA)
        // Permiso: ShowMyReservations
        // Muestra solo las reservas del usuario logueado.
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "ShowReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> MyReservations([FromQuery] PaginationRequest request)
        {
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);

            if (user == null)
            {
                _notyf.Error("No se pudo identificar al usuario.");
                return RedirectToAction("Index", "Home");
            }

            
            Response<PaginationResponse<ReservationDTO>> response =
                await _reservationsService.GetPaginationAsync( request); 
            return View(response.Result);
        }


        // =====================================================
        // CREATE (GET) - ADMINISTRATIVO
        // Permiso: CreateReservation
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "CreateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var productPricesMap = await _reservationsService.GetProductPricesMapAsync();

            ReservationDTO dto = new ReservationDTO
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

        // =====================================================
        // CREATE (POST) - ADMINISTRATIVO
        // Permiso: CreateReservation
        // =====================================================
        [HttpPost]
        [CustomAuthorize(permission: "CreateReservation", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Status))
            {
                dto.Status = "PendientePago";
            }
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación.");

                dto.Products = await _combosHelper.GetComboProducts();
                dto.Users = await _combosHelper.GetComboUsers();
                dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
                dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
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
            dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
            dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
            return View(dto);
        }

        // =====================================================
        // EDIT (GET)
        // Permiso: UpdateReservation
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
            dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);
            dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();

            return View(dto);
        }

        // =====================================================
        // EDIT (POST)
        // Permiso: UpdateReservation
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
                dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
                dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);

                return View(dto);
            }

            Response<ReservationDTO> response = await _reservationsService.EditAsync(dto);
            Console.WriteLine($"EDITA? -> {response.IsSuccess} | MSG: {response.Message}");
            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error(response.Message);

            // Si el servicio falla, recargar los combos y volver a la vista de edición.
            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(dto.UserId);
            dto.ProductPricesMap = await _reservationsService.GetProductPricesMapAsync();
            dto.PaymentMethods = _combosHelper.GetComboPaymentMethods(dto.PaymentMethod);

            return View(dto);
        }

        // =====================================================
        // DELETE
        // Permiso: DeleteReservation
        // Este permiso lo tiene tanto el Admin como el Turista (para sus propias reservas).
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

            // Redireccionamos a la lista general si es admin o si el cliente no puede ver su lista
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CREATE FROM PRODUCT (GET) – CLIENTE / TURISTA
        // Permiso: CreateReservationFromProduct
        // =====================================================
        [HttpGet]
        [CustomAuthorize(permission: "CreateReservationFromProduct", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> CreateFromProduct(int productId)
        {
            // Usuario logueado
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

            // Cargar combos, pero con product y user fijados
            var dto = new ReservationDTO
            {
                ProductId = productId,
                UserId = user.Id,
                Products = await _combosHelper.GetComboProducts(productId),  // bloqueado en vista
                Users = await _combosHelper.GetComboUsers(user.Id),
                PricePerNight = pricePerNight.Value,
                CheckIn = DateTime.Now,
                CheckOut = DateTime.Now.AddDays(1),
                PaymentMethod = "Transferencia Cliente (Por Defecto)"

            };

            return View(dto);
        }


        // =====================================================
        // CREATE FROM PRODUCT (POST) - CLIENTE / TURISTA
        // Permiso: CreateReservationFromProduct
        // =====================================================
        [HttpPost]
        [CustomAuthorize(permission: "CreateReservationFromProduct", module: "Reservation")]
        [Authorize]
        public async Task<IActionResult> CreateFromProduct(ReservationDTO dto)
        {
            if (dto == null)
            {
                _notyf.Error("Error: No se pudieron procesar los datos de la reserva. Inténtelo de nuevo.");
                return RedirectToAction("Index", "Products"); // Redirigir a una página segura
            }
            // Seguridad: el UserId NO debe venir del cliente, se toma del usuario logueado
            var user = await _usersService.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                _notyf.Error("Usuario inválido.");
                // Redirigir al listado general del cliente (MyReservations)
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
            

            // CRÍTICO: Necesario re-llenar PricePerNight en el DTO para que el servicio pueda calcular el Total
            dto.PricePerNight = pricePerNight.Value;
            if (string.IsNullOrEmpty(dto.PaymentMethod))
            {
                dto.PaymentMethod = "Transferencia Cliente (Por Defecto)";
            }
            if (string.IsNullOrEmpty(dto.Status))
            {
                dto.Status = "PendientePago";
            }

            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe corregir los errores.");
                // CRÍTICO: Necesario re-llenar PricePerNight si la validación falla para que la vista lo use
                dto.PricePerNight = pricePerNight.Value;
                dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
                dto.Users = await _combosHelper.GetComboUsers(user.Id);
                return View(dto);
            }

            var response = await _reservationsService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success("Reserva creada correctamente.");
                return RedirectToAction("Index","Reservations");
            }

            _notyf.Error(response.Message);

            dto.PricePerNight = pricePerNight.Value;
            dto.Products = await _combosHelper.GetComboProducts(dto.ProductId);
            dto.Users = await _combosHelper.GetComboUsers(user.Id);

            return View(dto);
        }
    }
}