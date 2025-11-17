using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;
using Serilog;
using ClaimsUser = System.Security.Claims.ClaimsPrincipal;

namespace OmniPyme.Web.Services
{
    public interface IUsersService
    {
        public Task<IdentityResult> AddUserAsync(Users users, string password);
        public bool CurrentUserIsAuthenticated();
        public Task<bool> CurrentUserIsAuthorizedAsync(string permission, string module);
        public Task<IdentityResult> ConfirmEmailAsync(Users users, string token);
        public Task<string> GenerateEmailConfirmationTokenAsync(Users users);
        public Task<Users> GetUserAsync(string email);
        public Task<SignInResult> LoginAsync(LoginDTO dto);
        public Task<Response<UserTokenDTO>> LoginApiAsync(LoginDTO dto);
        public Task LogoutAsync();
        public Task<Response<PaginationResponse<UsersDTO>>> GetPaginationAsync(PaginationRequest request);
        public Task<Response<UsersDTO>> CreateAsync(UsersDTO dto);
        public Task<Response<object>> DeleteAsync(string id);
        public Task<Users?> GetUserAsync(Guid id);
        public Task<Response<UsersDTO>> UpdateUserAsync(UsersDTO dto);
        public Task<int> UpdateUserAsync(AccountUserDTO dto);
        public Task<bool> CheckPasswordAsync(Users user, string currentPassword);
        public Task<string> GeneratePasswordResetTokenAsync(Users user);
        public Task<IdentityResult> ResetPasswordAsync(Users user, string resetToken, string newPassword);
        public Task<int> CountByRoleAsync(string role);
    }

    public class UsersService : CustomQueryableOperations, IUsersService
    {
        private readonly DataContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IStorageService _localStorageService;
        private readonly IStorageService _azureStorageService;
        private readonly string _container = "users";
        private const string DefaultUserImageUrl = "https://localhost:7045/users/0fb1b2a9-992a-4992-b70e-ee409baf034a.jpg";
        private readonly IConfiguration _configuration;

        public UsersService(DataContext context,
                            UserManager<Users> userManager,
                            SignInManager<Users> signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            IMapper mapper,
                            [FromKeyedServices("local")] IStorageService localStorageService,
                            [FromKeyedServices("azure")] IStorageService azureStorageService,
                            IConfiguration configuration)
            : base(context, mapper)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _localStorageService = localStorageService;
            _azureStorageService = azureStorageService;
            _configuration = configuration;
        }

        public async Task<IdentityResult> AddUserAsync(Users users, string password)
        {
            return await _userManager.CreateAsync(users, password);
        }

        public async Task<bool> CheckPasswordAsync(Users user, string currentPassword)
        {
            return await _userManager.CheckPasswordAsync(user, currentPassword);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(Users users, string token)
        {
            return await _userManager.ConfirmEmailAsync(users, token);
        }

        public async Task<int> CountByRoleAsync(string role)
        {
            return await _context.Users.CountAsync(u => u.PrivateURole.Name == role);
        }

        public async Task<Response<UsersDTO>> CreateAsync(UsersDTO dto)
        {
            try
            {
                Users users = _mapper.Map<Users>(dto);
                users.Id = Guid.NewGuid().ToString();
                users.Photo = DefaultUserImageUrl;

                IdentityResult result = await AddUserAsync(users, dto.Document);
                string token = await GenerateEmailConfirmationTokenAsync(users);
                await ConfirmEmailAsync(users, token);

                return ResponseHelper<UsersDTO>.MakeResponseSuccess(_mapper.Map<UsersDTO>(users), "Usuario creado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<UsersDTO>.MakeResponseFail(ex);
            }
        }

        public bool CurrentUserIsAuthenticated()
        {
            ClaimsUser? user = _httpContextAccessor.HttpContext?.User;
            return user?.Identity != null && user.Identity.IsAuthenticated;
        }

        public async Task<bool> CurrentUserIsAuthorizedAsync(string permission, string module)
        {
            ClaimsUser? claimsUser = _httpContextAccessor.HttpContext?.User;

            // Valida si hay sesión
            if (claimsUser is null)
            {
                return false;
            }

            string? userName = claimsUser.Identity!.Name;

            Users? users = await GetUserAsync(userName);

            if (users is null)
            {
                return false;
            }

            if (users.PrivateURole.Name == Env.SUPER_ADMIN_ROLE_NAME)
            {
                return true;
            }

            return await _context.Permissions.Include(p => p.RolePermissions)
                                             .AnyAsync(p => (p.Module == module && p.Name == permission)
                                                            && p.RolePermissions.Any(rp => rp.Roleid == users.PrivateURoleId));
        }

        public async Task<Response<object>> DeleteAsync(string id)
        {
            Users? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.ToString());
            if (user == null)
            {
                return new Response<object>
                {
                    IsSuccess = false,
                    Message = $"El usuario con id: {id} no existe"
                };
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new Response<object>
            {
                IsSuccess = true,
                Message = "Usuario eliminado con éxito"
            };
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(Users users)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(users);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Users user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<Response<PaginationResponse<UsersDTO>>> GetPaginationAsync(PaginationRequest request)
        {

            IQueryable<Users> query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(b => b.FirstName.ToLower().Contains(request.Filter.ToLower())
                                      || b.LastName.ToLower().Contains(request.Filter.ToLower())
                                      || b.Document.Contains(request.Filter)
                                      || b.Email.ToLower().Contains(request.Filter.ToLower())
                                      || b.PhoneNumber.Contains(request.Filter));
            }

            query = query.OrderBy(b => b.Id);

            return await GetPaginationAsync<Users, UsersDTO>(request, query);
        }

        public async Task<Users> GetUserAsync(string email)
        {
            Users? users = await _context.Users.Include(u => u.PrivateURole)
                                             .FirstOrDefaultAsync(u => u.Email == email);

            return users;
        }

        public async Task<Users?> GetUserAsync(Guid id)
        {
            return await _context.Users.Include(u => u.PrivateURole)
                                       .FirstOrDefaultAsync(u => u.Id == id.ToString());
        }

        public async Task<SignInResult> LoginAsync(LoginDTO dto)
        {
            return await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(Users user, string resetToken, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        }

        public async Task<Response<UsersDTO>> UpdateUserAsync(UsersDTO dto)
        {
            try
            {
                //User user = _mapper.Map<User>(dto);

                Guid id = Guid.Parse(dto.Id!);
                Users users = await GetUserAsync(id);
                users.PhoneNumber = dto.PhoneNumber;
                users.Document = dto.Document;
                users.FirstName = dto.FirstName;
                users.LastName = dto.LastName;
                users.PrivateURoleId = dto.PrivateURoleId;

                _context.Users.Update(users);

                await _context.SaveChangesAsync();

                return ResponseHelper<UsersDTO>.MakeResponseSuccess(dto, "Usuario actualizado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<UsersDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<int> UpdateUserAsync(AccountUserDTO dto)
        {
            try
            {
                Users users = await GetUserAsync(dto.Id);
                users.PhoneNumber = dto.PhoneNumber;
                users.Document = dto.Document;
                users.FirstName = dto.FirstName;
                users.LastName = dto.LastName;
                

                if (dto.Photo is not null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await dto.Photo.CopyToAsync(ms);
                        byte[] content = ms.ToArray();
                        string extension = Path.GetExtension(dto.Photo.FileName);

                        string oldPhoto = users.Photo;

                        // Guardar la nueva imagen
                        string newPhotoUrl = await _localStorageService.SaveFileAsync(
                            content,
                            extension,
                            _container,
                            dto.Photo.ContentType
                        );

                        // Solo eliminar la imagen anterior si no es la default
                        if (!string.IsNullOrEmpty(oldPhoto) && !oldPhoto.Equals(DefaultUserImageUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            await _localStorageService.DeleteFileAsync(oldPhoto, _container);
                        }

                        users.Photo = newPhotoUrl;
                    }
                }

                _context.Users.Update(users);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return 0;
            }
        }

        public async Task<Response<UserTokenDTO>> LoginApiAsync(LoginDTO dto)
        {
            try
            {
                SignInResult result = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, false);

                if (result.Succeeded)
                {
                    Users user = await _userManager.FindByEmailAsync(dto.Email);
                    UserTokenDTO tokenDTO = await BuildTokenAsync(dto.Email, user.Id);

                    return ResponseHelper<UserTokenDTO>.MakeResponseSuccess(tokenDTO, "Token obtenido con éxito");
                }

                return ResponseHelper<UserTokenDTO>.MakeResponseFail("Credenciales inválidas.");
            }
            catch (Exception ex)
            {
                return ResponseHelper<UserTokenDTO>.MakeResponseFail(ex);
            }
        }

        private async Task<UserTokenDTO> BuildTokenAsync(string email, string id)
        {
            List<Claim> claims = [
                new (ClaimTypes.Name, email),
                new (ClaimTypes.Email, email),
                new (ClaimTypes.NameIdentifier, id),
                new ("userid", id),
            ];
            Users identityUser = await _userManager.FindByEmailAsync(email);
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expiration = DateTime.UtcNow.AddYears(120);
            JwtSecurityToken token = new JwtSecurityToken(issuer: _configuration["Jwt:Issuer"],
                                                          audience: _configuration["Jwt:Audience"],
                                                          claims: claims,
                                                          expires: expiration,
                                                          signingCredentials: creds);
            return new UserTokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}