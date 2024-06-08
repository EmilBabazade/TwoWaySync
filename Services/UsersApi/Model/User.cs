using Domain;

namespace Services.UsersApi.Model;
public class User : IUser<Address, Company, Geo>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public Address Adress { get; set; }
    public Company Company { get; set; }
}
