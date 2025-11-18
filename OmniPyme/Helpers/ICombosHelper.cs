using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;

namespace OmniPyme.Web.Helpers
{
    public interface ICombosHelper
    {
        public Task<IEnumerable<SelectListItem>> GetComboCliente(int selectedId = 0);
        public Task<IEnumerable<SelectListItem>> GetComboProductCategories(int selectedId = 0);
        public Task<IEnumerable<SelectListItem>> GetComboProducts(int selectedId = 0);
        public Task<IEnumerable<SelectListItem>> GetComboRoles();

        public Task<IEnumerable<SelectListItem>> GetComboUsers(string selectedID = "");

        public IEnumerable<SelectListItem> GetComboPaymentMethods(string selectedName = "");
    }

    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboCliente(int selectedId = 0)
        {
            List<SelectListItem> list = await _context.Clients
                .Select(c => new SelectListItem
                {
                    Text = $"{c.FirstName} {c.LastName}",
                    Value = c.Id.ToString(),
                    Selected = c.Id == selectedId
                }).ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un cliente...]",
                Value = ""
            });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboProductCategories(int selectedId = 0)
        {
            List<SelectListItem> list = await _context.ProductCategories
                .Select(pc => new SelectListItem
                {
                    Text = pc.ProductCategoryName,
                    Value = pc.Id.ToString(),
                    Selected = pc.Id == selectedId
                }).ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una categoría...]",
                Value = ""
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboProducts(int selectedId = 0)
        {
            List<SelectListItem> list = await _context.Products
                .Select(p => new SelectListItem
                {
                    Text = p.ProductName,
                    Value = p.Id.ToString(),
                    Selected = p.Id == selectedId
                }).ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un producto...]",
                Value = ""
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboRoles()
        {
            List<SelectListItem> list = await _context.PrivateURoles.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un rol..]",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboUsers(string selectedId = "")
        {
            List<SelectListItem> list = await _context.Users
                .Select(u => new SelectListItem
                {
                    Text = $"{u.FirstName} {u.LastName} ({u.Email})",
                    Value = u.Id,
                    Selected = u.Id == selectedId
                })
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un usuario...]",
                Value = ""
            });

            return list;
        }
        public IEnumerable<SelectListItem> GetComboPaymentMethods(string selectedName = "")
        {
            // Lista estática de métodos de pago (ajustar si vienen de BD)
            var methods = new List<string> { "Transferencia Bancaria", "Tarjeta de Crédito", "Efectivo" };

            List<SelectListItem> list = methods
                .Select(m => new SelectListItem
                {
                    Text = m,
                    Value = m,
                    Selected = m == selectedName
                }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione método de pago...]",
                Value = "",
                Disabled = true,
                Selected = string.IsNullOrEmpty(selectedName)
            });

            return list;
        }

    }
}
