namespace ASP_32.Services.Storage
{
    public interface IStorageService
    {
        String Save(IFormFile file);
        byte[]? Load(String filename);
    }
}
