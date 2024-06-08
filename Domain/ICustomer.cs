using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;
public interface ICustomer<TAdress, TCompany, TGeo>
    where TAdress : class, IAddress<TGeo>
    where TCompany : class, ICompany
    where TGeo : class, IGeo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public TAdress Adress { get; set; }
    public TCompany Company { get; set; }
}
