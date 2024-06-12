using Common;
using System.Text;

namespace Services.UsersApi.ResponseModel;

// this should be poco classes but i already wrote the pretty print logic :)

public class RequestUser : PrettyPropToString
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public Company Company { get; set; }
}