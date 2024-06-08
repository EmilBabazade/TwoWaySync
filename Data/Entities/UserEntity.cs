using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities;
public class UserEntity : IUnique, IUser<AddressEntity, CompanyEntity, GeoEntity>
{
    public int RowId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public AddressEntity Adress { get; set; }
    public CompanyEntity Company { get; set; }
}
