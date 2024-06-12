using Common;

namespace Services.UsersApi.ResponseModel;

// this should be poco classes but i already wrote the pretty print logic :)
public class Address : PrettyPropToString
{
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string Zipcode { get; set; }
    public Geo Geo { get; set; }
}
