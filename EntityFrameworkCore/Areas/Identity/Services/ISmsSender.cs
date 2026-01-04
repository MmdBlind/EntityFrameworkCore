namespace EntityFrameworkCore.Areas.Identity.Services
{
    public interface ISmsSender
    {
        Task<string> SendAuthSmsAsync(string code, string phoneNumber);

        Task<string> SendAuthSmsPackageAsync(List<string> phoneNumber, string message);
    }
}
