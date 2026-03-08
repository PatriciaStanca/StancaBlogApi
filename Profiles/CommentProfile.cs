
namespace StancaBlogApi.Profiles;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(d => d.UserName, m => m.MapFrom(s => s.User.UserName));
    }
}
