namespace Domain.User;

public class User : IUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public string StreetAddress { get; set; }
    public string ApartmentSuite { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public string CompanyName { get; set; }
    public string CompanyCatchPhrase { get; set; }
    public string CompanyBs { get; set; }
}
