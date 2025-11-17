using AspNetCoreHero.ToastNotification.Abstractions;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;
using OmniPyme.Web.Core.Attributes;
using System.Runtime.ConstrainedExecution;

namespace OmniPyme.Web.Controllers
{

    public class ClientsController : Controller
    {
        private readonly IClientsService _clientsService;
        private readonly INotyfService _notyfService;

        public ClientsController(IClientsService clientsService, INotyfService notyfService)
        {
            _clientsService = clientsService;
            _notyfService = notyfService;
        }

        [HttpGet]
        [CustomAuthorize(permission: "showClient", module: "Client")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<ClientDTO>> response = await _clientsService.GetPaginationAsync(request);
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "createClient", module: "Client")]
        [Authorize]
        public IActionResult Create()
        {
            ClientDTO model = new ClientDTO
            {
                RegisterDate = DateTime.Now,
                LastPurchaseDate = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [CustomAuthorize(permission: "createClient", module: "Client")]
        [Authorize]
        public async Task<IActionResult> Create(ClientDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Debe ajustar los errores de validación");
                return View(dto);
            }
            Response<ClientDTO> response = await _clientsService.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyfService.Success(response.Message);
                return RedirectToAction(nameof(Index));

            }

            _notyfService.Error(response.Message);
            return View(dto);
        }

        [HttpGet]
        [CustomAuthorize(permission: "updateClient", module: "Client")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Debe ajustar los errores de validación");
                return RedirectToAction(nameof(Index));
            }
            Response<ClientDTO> response = await _clientsService.GetOneAsync(id);

            if (response.IsSuccess)
            {
                return View(response.Result);
            }

            _notyfService.Error(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [CustomAuthorize(permission: "updateClient", module: "Client")]
        [Authorize]
        public async Task<IActionResult> Edit(ClientDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyfService.Error("Debe ajustar los errores de validación");
                return View(dto);
            }
            Response<ClientDTO> response = await _clientsService.EditAsync(dto);

            if (response.IsSuccess)
            {
                _notyfService.Success(response.Message);
                return RedirectToAction(nameof(Index));

            }

            _notyfService.Error(response.Message);
            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "deleteClient", module: "Client")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Response<object> response = await _clientsService.DeleteAsync(id);

            if (response.IsSuccess)
            {
                _notyfService.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notyfService.Error(response.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
