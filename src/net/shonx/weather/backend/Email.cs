namespace net.shonx.weather.backend;

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

public class Email
{
    public string Value { get; }
    public int Zipcode { get; internal set;}

    public Email(string Email, int Zipcode)
    {
        if (new EmailAddressAttribute().IsValid(Email))
            Value = Email;
        else
            throw new FormatException();
        this.Zipcode = Zipcode;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
