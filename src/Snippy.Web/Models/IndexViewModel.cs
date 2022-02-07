using Snippy.Models;

namespace Snippy.Web.Models;

public class IndexViewModel
{
    public Owner AuthenticatedUser { get; set; }
    public string Title { get; set; }
    public string Platform { get; set; }
    public string Message { get; set; }
}
