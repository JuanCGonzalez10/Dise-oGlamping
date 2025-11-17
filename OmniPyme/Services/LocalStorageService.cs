
namespace OmniPyme.Web.Services
{
    public class LocalStorageService : IStorageService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task DeleteFileAsync(string path, string container)
        {
            if (path is not null)
            {
                string fileName = Path.GetFileName(path);
                string fileDirectory = Path.Combine(_env.WebRootPath, container, fileName);

                if (File.Exists(fileDirectory))
                {
                    File.Delete(fileDirectory);
                }
            }
        }

        public async Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType)
        {
            string fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(_env.WebRootPath, container);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string path = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(path, content);

            string currentUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            string dbUrl = Path.Combine(currentUrl, container, fileName).Replace("\\", "/");

            return dbUrl;
        }

        public async Task<string> UpdateFileAsync(byte[] content, string extension, string container, string path, string contentType)
        {
            await DeleteFileAsync(path, container);
            return await SaveFileAsync(content, extension, container, contentType);
        }
    }
}
