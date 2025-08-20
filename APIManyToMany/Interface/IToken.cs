using APIManyToMany.Models;

namespace APIManyToMany.Interface
{
    public interface IToken
    {
        string GenerateToken(User user);
    }
}
