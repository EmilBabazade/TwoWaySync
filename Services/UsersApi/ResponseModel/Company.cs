using Common;

namespace Services.UsersApi.ResponseModel;

// this should be poco classes but i already wrote the pretty print logic :)

public class Company : PrettyPropToString
{
    public string Name { get; set; }
    public string CatchPhrase { get; set; }
    public string Bs { get; set; }
}
