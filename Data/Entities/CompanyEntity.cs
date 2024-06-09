using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities;
public class CompanyEntity : IUnique, ICompany
{
    public int RowId { get; set; }
    public string Name { get; set; }
    public string CatchPhrase { get; set; }
    public string Bs { get; set; }
}
