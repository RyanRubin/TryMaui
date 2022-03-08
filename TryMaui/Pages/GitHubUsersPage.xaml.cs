using TryMaui.Data;

namespace TryMaui;

public partial class GitHubUsersPage : ContentPage
{
    public GitHubUsersPage()
    {
        InitializeComponent();

        using (var blogContext = new BloggingContext())
        {
            var theBlogs = blogContext.Blogs.ToList();

            if (!theBlogs.Any())
            {
                blogContext.Add(new Blog { Url = "a.com" });
                blogContext.Add(new Blog { Url = "b.com" });
                blogContext.Add(new Blog { Url = "c.com" });

                blogContext.SaveChanges();
            }
        }
    }

    private void TestButton_Clicked(object sender, EventArgs e)
    {
        using (var blogContext = new BloggingContext())
        {
            var theBlogs = blogContext.Blogs.ToList();

            var firstUrl = theBlogs.Select(o => o.Url).FirstOrDefault();
        }
    }
}