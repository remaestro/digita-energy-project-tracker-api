using DigitaEnergy.ProjectTracker.Application.DTOs.Invitations;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IInvitationService
{
    Task<InvitationDto> CreateInvitationAsync(CreateInvitationDto dto, Guid invitedByUserId);
    Task<IEnumerable<InvitationDto>> GetPendingInvitationsAsync();
    Task<IEnumerable<InvitationDto>> GetAllInvitationsAsync();
    Task<ValidateInvitationDto> ValidateTokenAsync(string token);
    Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto);
    Task<bool> CancelInvitationAsync(Guid invitationId);
    Task<bool> ResendInvitationAsync(Guid invitationId);
}
