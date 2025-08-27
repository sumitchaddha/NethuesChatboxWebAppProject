using Nethues_ChatboxWebApp.Models;

namespace Nethues_ChatboxWebApp.Services.Interface
{
    public interface IUserRepository
    {
        User? ValidateUser(string username, string password);
        public void SaveRefreshToken(RefreshToken token);
        User CreateUser(string username, string password, string FirstName, string LastName);
        User? GetUserById(int userId);

        public void LogAction(int userId, string action, string result);
        RefreshToken? GetRefreshToken(string token);
        User? GetByUsername(string username);
        public void UpdateRefreshToken(int userId);

        public void UpdatePassword(int userId, string newPasswordHash);
        List<object> GetUserActionsHistory(int userId);
    }
}
