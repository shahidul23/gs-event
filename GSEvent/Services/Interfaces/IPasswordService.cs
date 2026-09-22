using System;
using GSEvent.DTOs.Auth;

namespace GSEvent.Services.Interfaces;

public interface IPasswordService
{
    Task<string> ChangePasswordAsync(ChangePasswordDto request);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<string> ResetPasswordAsync(ResetPasswordDto request);
}
