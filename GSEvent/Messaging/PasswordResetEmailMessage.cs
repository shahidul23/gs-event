using System;

namespace GSEvent.Messaging;

public class PasswordResetEmailMessage
{
    public string Email {get; set;} = string.Empty;
    public string Username {get; set;} = string.Empty;
    public string Token {get; set;} = string.Empty;
}
