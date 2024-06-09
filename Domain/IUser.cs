namespace Domain;
public interface IUser<TAdress, TCompany, TGeo>
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
    public TAdress Address { get; set; }
    public TCompany Company { get; set; }
}
