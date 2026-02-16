using StancaBlogApi.Models;

namespace StancaBlogApi.Core.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(User user);
}
