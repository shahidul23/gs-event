using System;
using Microsoft.AspNetCore.Identity;

namespace GSEvent.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string Address {get; set;} = string.Empty;
    
}
