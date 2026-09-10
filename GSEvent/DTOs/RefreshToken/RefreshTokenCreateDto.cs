using System;

namespace GSEvent.DTOs.RefreshToken;

public class RefreshTokenCreateDto
{
    public string UserId {get; set;} = string.Empty;
    public string Token {get; set;} = string.Empty;
    public string JwtId {get; set;} = string.Empty;
    public DateTime CreatedAt {get; set;}
    public DateTime ExpiredAt {get; set;}   
}
