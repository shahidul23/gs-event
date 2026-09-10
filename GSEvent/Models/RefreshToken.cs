using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GSEvent.Models;

public class RefreshToken
{
    public int Id {get; set;}
    public string UserId {get; set;} = string.Empty;
    public string Token {get; set;} = string.Empty;
    public string JwtId {get; set;} = string.Empty;
    public bool IsRevoked {get; set;}
    public DateTime CreatedAt {get; set;}
    public DateTime ExpiredAt {get; set;}

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User {get; set;}
}
