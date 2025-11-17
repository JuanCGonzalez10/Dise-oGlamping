namespace OmniPyme.Web.Services
{
    public interface IStorageService
    {
        public Task DeleteFileAsync(string path, string container);
        public Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType);
        public Task<string> UpdateFileAsync(byte[] content, string extension, string container, string path, string contentType);

    }
}
