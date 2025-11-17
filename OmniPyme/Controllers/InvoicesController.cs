using AspNetCoreHero.ToastNotification.Abstractions;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;
using OmniPyme.Web.ViewModels;



namespace OmniPyme.Web.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly IInvoicesService _invoicesService;
        private readonly ISalesService _salesService;
        private readonly ISaleDetailService _saleDetailService;
        private readonly INotyfService _notyfService;
        private readonly ICombosHelper _combosHelper;
        private readonly IProductsService _productsService;

        public InvoicesController(IInvoicesService invoicesService, INotyfService notyfService, ICombosHelper combosHelper, ISalesService salesService, ISaleDetailService saleDetailService, IProductsService productsService)
        {
            _invoicesService = invoicesService;
            _notyfService = notyfService;
            _combosHelper = combosHelper;
            _salesService = salesService;
            _saleDetailService = saleDetailService;
            _productsService = productsService;
        }

        [HttpGet]
        [CustomAuthorize(permission: "ShowSale", module: "Sale")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<InvoiceDTO>> response = await _invoicesService.GetPaginationAsync(request);
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "CreateSale", module: "Sale")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            // Obtener el último registro ordenado por InvoiceNumber
            string nextInvoiceNumber = await _invoicesService.GetNextInvoiceNumberAsync();

            List<ProductDTO> products = await _productsService.GetProductListAsync();

            InvoiceViewModel viewModel = new InvoiceViewModel
            {
                InvoiceNumber = nextInvoiceNumber,
                InvoiceDate = DateTime.Now,
                SaleDate = DateTime.Now,
                Clients = await _combosHelper.GetComboCliente(),
                ProductList = products,
            };
            return View(viewModel);
        }

        [HttpPost]
        [CustomAuthorize(permission: "CreateSale", module: "Sale")]
        [Authorize]
        public async Task<IActionResult> Create(InvoiceViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                _notyfService.Error("Debe ajustar los errores de validacion");
                viewModel.Clients = await _combosHelper.GetComboCliente();
                return View(viewModel);
            }

            //Mapea de InvoiceCreateViewModel a SaleDTO
            SaleDTO dto = new SaleDTO
            {
                IdClient = viewModel.IdClient,
                SaleDate = viewModel.SaleDate,
                SaleTotal = (decimal)viewModel.SaleTotal,
                SalePaymentMethod = viewModel.SalePaymentMethod
            };

            // Verificar que hay detalles de venta
            if (viewModel.SaleDetails == null || !viewModel.SaleDetails.Any())
            {
                _notyfService.Error("La factura debe tener al menos un producto");
                viewModel.Clients = await _combosHelper.GetComboCliente();
                return View(viewModel);
            }

            Response<SaleDTO> saleResponse = await _salesService.CreateAsync(dto);

            if (!saleResponse.IsSuccess)
            {
                _notyfService.Error("Error al crear la venta: " + saleResponse.Message);
                viewModel.Clients = await _combosHelper.GetComboCliente();
                return View(viewModel);
            }

            viewModel.IdSale = saleResponse.Result.Id;
            viewModel.SaleCode = saleResponse.Result.SaleCode;

            if (viewModel.IdSale == null)
            {
                _notyfService.Error("Error al crear la venta: " + saleResponse.Message);
                viewModel.Clients = await _combosHelper.GetComboCliente();
                return View(viewModel);
            }

            int detailCounter = 1;

            foreach (SaleDetailViewModel detail in viewModel.SaleDetails)
            {
                detail.IdSale = viewModel.IdSale ?? 0;

                string generatedDetailCode = $"{viewModel.SaleCode}-{detailCounter}";

                await _saleDetailService.CreateAsync(new SaleDetailDTO
                {
                    IdSale = detail.IdSale,
                    SaleDetailCode = generatedDetailCode,
                    SaleDetailProductCode = detail.IdProduct,
                    SaleDetailProductQuantity = detail.SaleDetailProductQuantity,
                    SaleDetailProductPrice = (decimal)detail.SaleDetailProductPrice,
                    SaleDetailSubtotal = (decimal)detail.SaleDetailSubtotal,
                    SaleDetailProductTax = (decimal)detail.SaleDetailProductTax
                });
                detailCounter++;
            }

            Response<InvoiceDTO> response = await _invoicesService.CreateAsync(new InvoiceDTO
            {
                InvoiceNumber = viewModel.InvoiceNumber,
                InvoiceDate = viewModel.InvoiceDate,
                IdSale = viewModel.IdSale ?? 0
            });

            if (!response.IsSuccess)
            {
                _notyfService.Error("Error al crear la factura: " + response.Message);
                viewModel.Clients = await _combosHelper.GetComboCliente();
                return View(viewModel);
            }

            _notyfService.Success("Factura creada correctamente");
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorize(permission: "DeleteSale", module: "Sale")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            //Response<object> response = await _invoicesService.DeleteAsync(id);
            //if (!response.IsSuccess)
            //{
            //    _notyfService.Error(response.Message);
            //}
            //else
            //{
            //    _notyfService.Success(response.Message);
            //}
            //return RedirectToAction("Index");

            // Obtener la factura para saber su IdSale asociado
            Response<InvoiceDTO> invoiceResponse = await _invoicesService.GetOneAsync(id);

            if (!invoiceResponse.IsSuccess)
            {
                _notyfService.Error("Error al obtener la factura: " + invoiceResponse.Message);
                return RedirectToAction("Index");
            }

            int idSale = invoiceResponse.Result.IdSale;

            //// Eliminar los detalles de la venta
            //Response<List<SaleDetailDTO>> detailResponse = await _saleDetailService.GetBySaleIdAsync(idSale);
            //if (detailResponse.IsSuccess)
            //{
            //    foreach (SaleDetailDTO detail in detailResponse.Result)
            //    {
            //        await _saleDetailService.DeleteAsync(detail.Id);
            //    }
            //}

            // Eliminar la venta
            Response<object> saleDeleteResponse = await _salesService.DeleteAsync(idSale);
            if (!saleDeleteResponse.IsSuccess)
            {
                _notyfService.Error(saleDeleteResponse.Message);
                return RedirectToAction("Index");
            }

            //// Eliminar la factura
            //Response<object> invoiceDeleteResponse = await _invoicesService.DeleteAsync(id);

            // Enviar la notificación de éxito o error
            if (!saleDeleteResponse.IsSuccess)
            {
                _notyfService.Error(saleDeleteResponse.Message);
            }
            else
            {
                _notyfService.Success(saleDeleteResponse.Message);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [CustomAuthorize(permission: "ViewSale", module: "Sale")]
        [Authorize]
        public async Task<IActionResult> View([FromRoute] int id)
        {
            Response<InvoiceDTO> invoiceResponse = await _invoicesService.GetOneAsync(id);

            if (!invoiceResponse.IsSuccess)
            {
                _notyfService.Error("Error al obtener la factura: " + invoiceResponse.Message);
                return RedirectToAction("Index");
            }

            Response<List<SaleDetailDTO>> detailResponse = await _saleDetailService.GetBySaleIdAsync(invoiceResponse.Result.IdSale);
            if (!detailResponse.IsSuccess)
            {
                _notyfService.Error("Error al obtener los detalles de la venta: " + detailResponse.Message);
                return RedirectToAction("Index");
            }

            List<ProductDTO> productList = await _productsService.GetProductListAsync();

            int idClient = invoiceResponse.Result.Sale.IdClient;
            IEnumerable<SelectListItem> clientes = await _combosHelper.GetComboCliente();
            SelectListItem clienteItem = clientes.FirstOrDefault(c => c.Value == idClient.ToString());
            string clientName = clienteItem?.Text ?? "Cliente no encontrado";

            InvoiceViewModel viewModel = new InvoiceViewModel
            {
                IdInvoice = invoiceResponse.Result.Id,
                InvoiceNumber = invoiceResponse.Result.InvoiceNumber,
                InvoiceDate = invoiceResponse.Result.InvoiceDate,
                IdSale = invoiceResponse.Result.IdSale,
                SaleCode = invoiceResponse.Result.Sale.SaleCode,
                SaleDate = invoiceResponse.Result.Sale.SaleDate,
                SaleTotal = (double)invoiceResponse.Result.Sale.SaleTotal,
                SalePaymentMethod = invoiceResponse.Result.Sale.SalePaymentMethod,
                IdClient = invoiceResponse.Result.Sale.IdClient,
                ClientName = clientName,
                SaleDetails = detailResponse.Result.Select(d => new SaleDetailViewModel
                {
                    IdSaleDetail = d.Id,
                    IdSale = d.IdSale,
                    SaleDetailCode = d.SaleDetailCode,
                    IdProduct = d.SaleDetailProductCode,
                    SaleDetailProductCode = d.SaleDetailProductCode,
                    SaleDetailProductQuantity = d.SaleDetailProductQuantity,
                    SaleDetailProductPrice = (double)d.SaleDetailProductPrice,
                    SaleDetailProductTax = (double)d.SaleDetailProductTax,
                    SaleDetailSubtotal = (double)d.SaleDetailSubtotal
                }).ToList(),
                ProductList = productList
            };

            return View(viewModel);
        }

    }
}
