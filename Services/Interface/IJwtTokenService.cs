using Nethues_ChatboxWebApp.Models;

namespace Nethues_ChatboxWebApp.Services.Interface
{
    public interface IJwtTokenService
    {
        string CreateToken(User user);
        RefreshToken CreateRefreshToken(int userId);
       public  string NumberToWords(int number);
    }
}
