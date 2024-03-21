
using System.ComponentModel.DataAnnotations;

public class Email

{
    public Email(string email, int zipcode)
    {
        if (new EmailAddressAttribute().IsValid(email))
            this.email = email;
        this.zipcode = zipcode;
    }
    public string email { set; get; }

    public int zipcode { set; get; }

    public override string ToString()
    {
        return email + zipcode;
    }
}
