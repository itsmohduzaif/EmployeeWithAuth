using HRManagement.Entities;

namespace HRManagement.JwtFeatures
{
    public interface IJwtHandler
    {
        string CreateToken(User user, IList<string> roles);
    }
}
