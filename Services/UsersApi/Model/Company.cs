using Domain;

namespace Services.UsersApi.Model;
public class Company : ICompany
{
    public string Name { get; set; }
    public string CatchPhrase { get; set; }
    public string Bs { get; set; }
}
