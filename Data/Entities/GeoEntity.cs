using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities;
public class GeoEntity : IGeo, IUnique
{
    public int RowId { get; set; }
    public string Lat { get; set; }
    public string Lng { get; set; }
}
