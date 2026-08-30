public class Customer
{
    public string id { get; set; }
    public string? name { get; set; }
    public string? address { get; set; }
    public string? headquarters { get; set; }
    public string? country { get; set; }
    public string? telephone_1 { get; set; }
    public string? email_1 { get; set; }
    public string? cre_date { get; set; }
    public string? cha_date { get; set; }

    public Customer()
    {
    }
    public Customer(string ParId)
    {
        id = ParId;
    }
}