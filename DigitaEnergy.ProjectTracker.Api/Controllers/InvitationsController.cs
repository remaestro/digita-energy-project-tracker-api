using DigitaEnergy.ProjectTracker.Application.DTOs.Invitations;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvitationsController : ControllerBase
{
    private readonly IInvitationService _invitationService;

    public InvitationsController(IInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found"));
    }

    private UserRole GetCurrentUserRole()
    {
        var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        return Enum.Parse<UserRole>(roleClaim ?? throw new UnauthorizedAccessException("User role not found"));
    }

    [HttpPost]
    [Authorize(Roles = "PROJECT_MANAGER")]
    public async Task<ActionResult<InvitationDto>> CreateInvitation([FromBody] CreateInvitationDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var invitation = await _invitationService.CreateInvitationAsync(dto, userId);
            return Ok(invitation);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("pending")]
    [Authorize(Roles = "PROJECT_MANAGER")]
    public async Task<ActionResult<IEnumerable<InvitationDto>>> GetPendingInvitations()
    {
        var invitations = await _invitationService.GetPendingInvitationsAsync();
        return Ok(invitations);
    }

    [HttpGet]
    [Authorize(Roles = "PROJECT_MANAGER")]
    public async Task<ActionResult<IEnumerable<InvitationDto>>> GetAllInvitations()
    {
        var invitations = await _invitationService.GetAllInvitationsAsync();
        return Ok(invitations);
    }

    [HttpGet("validate/{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<ValidateInvitationDto>> ValidateToken(string token)
    {
        var result = await _invitationService.ValidateTokenAsync(token);
        
        if (!result.IsValid)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("accept")]
    [AllowAnonymous]
    public async Task<ActionResult> AcceptInvitation([FromBody] AcceptInvitationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
        {
            return BadRequest(new { message = "Le mot de passe doit contenir au moins 8 caractères." });
        }

        var success = await _invitationService.AcceptInvitationAsync(dto);
        
        if (!success)
        {
            return BadRequest(new { message = "Impossible d'accepter l'invitation. Le lien est invalide ou a expiré." });
        }

        return Ok(new { message = "Compte créé avec succès. Vous pouvez maintenant vous connecter." });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "PROJECT_MANAGER")]
    public async Task<ActionResult> CancelInvitation(Guid id)
    {
        var success = await _invitationService.CancelInvitationAsync(id);
        
        if (!success)
        {
            return NotFound(new { message = "Invitation introuvable ou déjà traitée." });
        }

        return Ok(new { message = "Invitation annulée." });
    }

    [HttpPost("{id}/resend")]
    [Authorize(Roles = "PROJECT_MANAGER")]
    public async Task<ActionResult> ResendInvitation(Guid id)
    {
        var success = await _invitationService.ResendInvitationAsync(id);
        
        if (!success)
        {
            return NotFound(new { message = "Invitation introuvable ou déjà traitée." });
        }

        return Ok(new { message = "Invitation renvoyée avec succès." });
    }
}
