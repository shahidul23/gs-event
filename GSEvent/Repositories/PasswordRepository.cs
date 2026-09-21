using System;
using System.Text;
using GSEvent.DTOs.Auth;
using GSEvent.Enums;
using GSEvent.Exceptions;
using GSEvent.Messaging;
using GSEvent.Models;
using GSEvent.RabbitMQ.Service.Interface;
using GSEvent.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace GSEvent.Repositories;

public class PasswordRepository : IPasswordRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;

    public PasswordRepository(
        UserManager<ApplicationUser> userManager,
        IRabbitMqPublisher rabbitMqPublisher)
    {
        _userManager = userManager;
        _rabbitMqPublisher = rabbitMqPublisher;
    }
    public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, ChangePasswordDto request)
    {
        return await _userManager.ChangePasswordAsync(
            user,
            request.OldPassword,
            request.NewPassword
        );
    }

    public async Task<bool> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new BadRequestException("User not found.");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken  = WebEncoders.Base64UrlEncode(
            Encoding.UTF8.GetBytes(token)
        );
        await _rabbitMqPublisher.PublishAsync(
            RabbitMqQueue.PasswordReset,
            new PasswordResetEmailMessage
            {
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                Token = encodedToken 
            }
        );
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto reset)
    {
        var user = await _userManager.FindByEmailAsync(reset.Email);
        if (user == null)
        {
            throw new BadRequestException(
                "Invalid password reset request."
            );
        }
        string decodedToken;
        try
        {
            decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(reset.Token)
            );
        }
        catch (FormatException)
        {
            throw new BadRequestException("Invalid password reset token.");
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            decodedToken,
            reset.NewPassword
        );
        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x =>
                    $"{x.Code}: {x.Description}"
                )
            );

            throw new BadRequestException(errors);
        }

        return true;
    }
}
