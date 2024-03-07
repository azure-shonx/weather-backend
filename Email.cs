
using System.ComponentModel.DataAnnotations;

public class Email

{
    public Email(string email)
    {
        if (new EmailAddressAttribute().IsValid(email))
            this.email = email;
    }
    public string email { get; }

    public override string ToString()
    {
        return email;
    }
}
