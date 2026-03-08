
namespace StancaBlogApi.Core.Interfaces;

public interface IJwtService
{
    string Generate(User user);
}
