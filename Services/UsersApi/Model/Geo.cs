using Domain;

namespace Services.UsersApi.Model;
public class Geo : IGeo
{
    public string Lat { get; set; }
    public string Lng { get; set; }
}
