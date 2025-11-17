using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;

namespace OmniPyme.Web.Services
{
    public class CustomQueryableOperations
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CustomQueryableOperations(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<TDTO>> CreateAsync<TEntity, TDTO>(TDTO dto)
        {
            try
            {
                TEntity entity = _mapper.Map<TEntity>(dto);

                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();

                // Asignar el ID generado al DTO
                TDTO resultDTO = _mapper.Map<TDTO>(entity);

                return ResponseHelper<TDTO>.MakeResponseSuccess(resultDTO, "Registro creado con éxito");
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("IX_Clients_DNI"))
                {
                    return ResponseHelper<TDTO>.MakeResponseFail("El documento ya está registrado en el sistema.");
                }

                return ResponseHelper<TDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<object>> DeleteAsync<TEntity>(int id) where TEntity : class, IId
        {
            try
            {
                TEntity? entity = await _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);

                if (entity is null)
                {
                    return ResponseHelper<object>.MakeResponseFail($"No existe el registro con id {id}");
                }

                _context.Remove(entity);
                await _context.SaveChangesAsync();

                return ResponseHelper<object>.MakeResponseSuccess("Registro eliminado con éxito");

            }
            catch (Exception ex)
            {
                return ResponseHelper<object>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<TDTO>> EditAsync<TEntity, TDTO>(TDTO dto, int id) where TEntity : class, IId
        {
            try
            {
                if (id == 0)
                {
                    return ResponseHelper<TDTO>.MakeResponseFail("El ID es requerido.");
                }

                var trackedEntity = await _context.Set<TEntity>().FindAsync(id);
                if (trackedEntity == null)
                {
                    return ResponseHelper<TDTO>.MakeResponseFail($"No se encontró la entidad con ID {id}.");
                }
                _mapper.Map(dto, trackedEntity);
                await _context.SaveChangesAsync();

                return ResponseHelper<TDTO>.MakeResponseSuccess(dto, "Registro actualizado con éxito");
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("IX_Clients_DNI"))
                {
                    return ResponseHelper<TDTO>.MakeResponseFail("El documento ya está registrado en el sistema.");
                }

                return ResponseHelper<TDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<TDTO>> GetOneAsync<TEntity, TDTO>(int id) where TEntity : class, IId where TDTO : class
        {
            try
            {
                TEntity? entity = await _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id);

                if (entity is null)
                {
                    return ResponseHelper<TDTO>.MakeResponseFail($"No existe el registro con id {id}");
                }

                TDTO dto = _mapper.Map<TDTO>(entity);

                return ResponseHelper<TDTO>.MakeResponseSuccess(dto, "Registro eliminado con éxito");

            }
            catch (Exception ex)
            {
                return ResponseHelper<TDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<PaginationResponse<TDTO>>> GetPaginationAsync<TEntity, TDTO>(PaginationRequest request, IQueryable<TEntity> query = null)
        where TEntity : class
        where TDTO : class
        {
            try
            {
                if (query is null)
                {
                    query = _context.Set<TEntity>();
                }

                PagedList<TEntity> list = await PagedList<TEntity>.ToPagedListAsync(query, request);

                PaginationResponse<TDTO> response = new PaginationResponse<TDTO>
                {
                    List = _mapper.Map<PagedList<TDTO>>(list),
                    TotalRecords = list.TotalRecords,
                    RecordsPerPage = list.RecordsPerPage,
                    CurrentPage = list.CurrentPage,
                    TotalPages = list.TotalPages,
                    Filter = request.Filter,
                };

                return ResponseHelper<PaginationResponse<TDTO>>.MakeResponseSuccess(response);
            }
            catch (Exception ex)
            {
                return ResponseHelper<PaginationResponse<TDTO>>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<List<TDTO>>> GetCompleteList<TEntity, TDTO>(IQueryable<TEntity> query = null)
            where TEntity : class
            where TDTO : class
        {
            try
            {
                if (query is null)
                {
                    query = _context.Set<TEntity>();
                }

                List<TEntity> list = await query.ToListAsync();
                List<TDTO> dtoList = _mapper.Map<List<TDTO>>(list);

                return ResponseHelper<List<TDTO>>.MakeResponseSuccess(dtoList);
            }
            catch (Exception ex)
            {
                return ResponseHelper<List<TDTO>>.MakeResponseFail(ex);
            }
        }
    }
}
