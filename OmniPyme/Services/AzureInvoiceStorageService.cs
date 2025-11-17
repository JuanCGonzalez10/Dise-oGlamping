
namespace OmniPyme.Web.Services
{
    public class AzureInvoiceStorageService : IStorageService
    {
        public Task DeleteFileAsync(string path, string container)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveFileAsync(byte[] content, string extension, string container, string contentType)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateFileAsync(byte[] content, string extension, string container, string path, string contentType)
        {
            throw new NotImplementedException();
        }
    }
}
