
namespace StancaBlogApi.Profiles;

public class BlogPostProfile : Profile
{
    public BlogPostProfile()
    {
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(d => d.CategoryName, m => m.MapFrom(s => s.Category.Name))
            .ForMember(d => d.UserName, m => m.MapFrom(s => s.User.UserName))
            .ForMember(d => d.Comments, m => m.MapFrom(s => s.Comments));
    }
}
