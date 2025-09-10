using ASP_32.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace ASP_32.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;
        public IActionResult Item(String id)
        {
            String ext = Path.GetExtension(id);
            String mimeType = ext switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png,",
                _ => "applicatrion/octet-stream"

            };
            var bytes = _storageService.Load(id);
            return bytes == null ? NotFound() : File(bytes, mimeType);
        }
    }
}
