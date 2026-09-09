using System;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Models;

public class ApplicationUser : IdentityUser
{
    public string Address {get; set;} = string.Empty;
}
