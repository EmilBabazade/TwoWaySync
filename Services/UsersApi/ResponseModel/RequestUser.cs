using System.Text;

namespace Services.UsersApi.ResponseModel;

public class RequestUser
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public Company Company { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var prop in typeof(RequestUser).GetProperties())
        {
            sb.Append(prop.Name + ": " + prop.GetValue(this) + "\n");
        }
        return sb.ToString();
    }
}