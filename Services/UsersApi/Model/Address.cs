using Domain;

namespace Services.UsersApi.Model;
public class Address : IAddress<Geo>
{
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public Geo Geo { get; set; }
}
