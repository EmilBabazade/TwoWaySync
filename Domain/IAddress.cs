namespace Domain;

public interface IAddress<TGeo> where TGeo : class, IGeo
{
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public TGeo Geo { get; set; }
}
