namespace StancaBlogApi.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
