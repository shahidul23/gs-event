using System;

namespace GSEvent.Services.Interfaces;

public interface ICurrentUserService
{
    string? UserId {get;}
    string? UserName {get;}
    string? Email {get;}
    string? Phone {get;}
    string? FullName{get;}
    string? Role {get;}
    bool IsAuthenticated {get;}
}
