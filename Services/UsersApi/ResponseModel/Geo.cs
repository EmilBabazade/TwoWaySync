using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.UsersApi.ResponseModel;
// this should be poco classes but i already wrote the pretty print logic :)

public class Geo : PrettyPropToString
{
    public string Lat { get; set; }
    public string Lng { get; set; }
}
