
namespace StancaBlogApi.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.UserId, m => m.MapFrom(s => s.Id));
    }
}
