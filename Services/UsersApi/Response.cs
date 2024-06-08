using Services.UsersApi.Model;

namespace Services.UsersApi;
public class Response
{
    public ICollection<User> Users { get; set; }
}
