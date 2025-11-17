using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using OmniPyme.Data;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Core;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.DTOs;
using static OmniPyme.Web.DTOs.PrivateURoleDTO;

namespace OmniPyme.Web.Services
{
    public interface IRolesService
    {
        public Task<Response<PaginationResponse<PrivateURoleDTO>>> GetPaginationAsync(PaginationRequest request);
        public Task<Response<PrivateURoleDTO>> GetOneAsync(int id);
        public Task<Response<List<PermissionDTO>>> GetPermissionsAsync();
        public Task<Response<PrivateURoleDTO>> CreateAsync(PrivateURoleDTO dto);
        public Task<Response<object>> DeleteAsync(int id);
        public Task<Response<PrivateURoleDTO>> EditAsync(PrivateURoleDTO dto);
        public Task<Response<List<PermissionForRoleDTO>>> GetPermissionsByRoleAsync(int id);
    }

    public class RolesService : CustomQueryableOperations, IRolesService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public RolesService(DataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<PrivateURoleDTO>> CreateAsync(PrivateURoleDTO dto)
        {
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            {
                try
                {
                    PrivateURole role = _mapper.Map<PrivateURole>(dto);
                    await _context.PrivateURoles.AddAsync(role);
                    await _context.SaveChangesAsync();

                    int roleId = role.Id;
                    List<int>? permissionIds = !string.IsNullOrWhiteSpace(dto.PermissionIds)
                        ? JsonConvert.DeserializeObject<List<int>>(dto.PermissionIds)
                        : new List<int>();

                    foreach (int permissionId in permissionIds)
                    {
                        await _context.RolePermissions.AddAsync(new RolePermission
                        {
                            Roleid = roleId,
                            permissionId = permissionId
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return ResponseHelper<PrivateURoleDTO>.MakeResponseSuccess(dto, "Rol creado con éxito");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return ResponseHelper<PrivateURoleDTO>.MakeResponseFail(ex);
                }
            }
        }

        public async Task<Response<PrivateURoleDTO>> EditAsync(PrivateURoleDTO dto)
        {
            try
            {
                if (dto.Name == Env.SUPER_ADMIN_ROLE_NAME)
                    return ResponseHelper<PrivateURoleDTO>.MakeResponseFail($"El rol '{Env.SUPER_ADMIN_ROLE_NAME}' no puede ser editado");

                List<int>? permissionIds = !string.IsNullOrWhiteSpace(dto.PermissionIds)
                    ? JsonConvert.DeserializeObject<List<int>>(dto.PermissionIds)
                    : new List<int>();

                var oldPermissions = await _context.RolePermissions.Where(rp => rp.Roleid == dto.Id).ToListAsync();
                _context.RolePermissions.RemoveRange(oldPermissions);

                foreach (int permId in permissionIds)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission
                    {
                        Roleid = dto.Id,
                        permissionId = permId
                    });
                }

                var roleEntity = _mapper.Map<PrivateURole>(dto);
                _context.PrivateURoles.Update(roleEntity);

                await _context.SaveChangesAsync();
                return ResponseHelper<PrivateURoleDTO>.MakeResponseSuccess(dto, "Rol actualizado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<PrivateURoleDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<PaginationResponse<PrivateURoleDTO>>> GetPaginationAsync(PaginationRequest request)
        {
            IQueryable<PrivateURole> query = _context.PrivateURoles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.Where(r => r.Name.ToLower().Contains(request.Filter.ToLower()));
            }

            query = query.OrderBy(r => r.Id);

            return await GetPaginationAsync<PrivateURole, PrivateURoleDTO>(request, query);
        }

        public async Task<Response<PrivateURoleDTO>> GetOneAsync(int id)
        {
            try
            {
                var role = await _context.PrivateURoles.FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                    return ResponseHelper<PrivateURoleDTO>.MakeResponseFail($"El rol con id '{id}' no existe.");

                var permissions = await _context.Permissions
                    .Select(p => new PermissionForRoleDTO
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Module = p.Module,
                        Selected = _context.RolePermissions.Any(rp => rp.permissionId == p.Id && rp.Roleid == role.Id)
                    }).ToListAsync();

                var dto = new PrivateURoleDTO
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = permissions
                };

                return ResponseHelper<PrivateURoleDTO>.MakeResponseSuccess(dto);
            }
            catch (Exception ex)
            {
                return ResponseHelper<PrivateURoleDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<List<PermissionDTO>>> GetPermissionsAsync()
        {
            return await GetCompleteList<Permission, PermissionDTO>();
        }

        public async Task<Response<List<PermissionForRoleDTO>>> GetPermissionsByRoleAsync(int id)
        {
            try
            {
                var response = await GetOneAsync(id);
                if (!response.IsSuccess)
                    return ResponseHelper<List<PermissionForRoleDTO>>.MakeResponseFail(response.Message);

                return ResponseHelper<List<PermissionForRoleDTO>>.MakeResponseSuccess(response.Result.Permissions);
            }
            catch (Exception ex)
            {
                return ResponseHelper<List<PermissionForRoleDTO>>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                PrivateURole role = await _context.PrivateURoles.FirstOrDefaultAsync(r => r.Id == id);
                if (role == null)
                    return ResponseHelper<object>.MakeResponseFail($"El rol con id '{id}' no existe.");

                if (role.Name == Env.SUPER_ADMIN_ROLE_NAME)
                    return ResponseHelper<object>.MakeResponseFail($"El rol '{Env.SUPER_ADMIN_ROLE_NAME}' no puede ser eliminado.");

                List<RolePermission> rolePermissions = await _context.RolePermissions.Where(rp => rp.Roleid == id).ToListAsync();
                _context.RolePermissions.RemoveRange(rolePermissions);

                _context.PrivateURoles.Remove(role);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ResponseHelper<object>.MakeResponseSuccess(null, "Rol eliminado con éxito");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResponseHelper<object>.MakeResponseFail(ex);
            }
        }
    }
}
