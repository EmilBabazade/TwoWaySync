using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities;
public class AddressEntity : IAddress<GeoEntity>, IUnique
{
    public int RowId { get; set; }
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public GeoEntity Geo { get; set; }
    public int GeoRowId { get; set; }
}
