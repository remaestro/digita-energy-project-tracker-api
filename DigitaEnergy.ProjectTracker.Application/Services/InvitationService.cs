using DigitaEnergy.ProjectTracker.Application.DTOs.Invitations;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Entities;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using DigitaEnergy.ProjectTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class InvitationService : IInvitationService
{
    private readonly ProjectTrackerDbContext _context;
    private readonly IEmailService _emailService;

    public InvitationService(ProjectTrackerDbContext context, IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<InvitationDto> CreateInvitationAsync(CreateInvitationDto dto, Guid invitedByUserId)
    {
        // Vérifier que l'email n'existe pas déjà
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Un utilisateur avec cet email existe déjà.");
        }

        // Vérifier qu'il n'y a pas déjà une invitation en attente pour cet email
        var existingInvitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Email == dto.Email && i.Status == InvitationStatus.Pending);
        
        if (existingInvitation != null)
        {
            throw new InvalidOperationException("Une invitation est déjà en attente pour cet email.");
        }

        // Créer l'invitation
        var invitation = new UserInvitation
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            InvitedByUserId = invitedByUserId,
            Role = dto.Role,
            AssignedWorkstreams = dto.AssignedWorkstreams,
            Token = Guid.NewGuid().ToString("N"), // Token unique sans tirets
            Status = InvitationStatus.Pending,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _context.UserInvitations.Add(invitation);
        await _context.SaveChangesAsync();

        // Récupérer les infos de l'inviteur pour le DTO
        var invitedBy = await _context.Users.FindAsync(invitedByUserId);

        // Envoyer l'email d'invitation
        try
        {
            var invitedByName = invitedBy != null ? $"{invitedBy.FirstName} {invitedBy.LastName}" : "L'équipe";
            await _emailService.SendInvitationEmailAsync(
                invitation.Email,
                invitation.FirstName ?? "",
                invitation.LastName ?? "",
                invitation.Token,
                invitedByName
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[INVITATION] Failed to send invitation email: {ex.Message}");
            // On ne bloque pas le processus si l'email échoue
        }

        return new InvitationDto
        {
            Id = invitation.Id,
            Email = invitation.Email,
            FirstName = invitation.FirstName,
            LastName = invitation.LastName,
            InvitedByUserId = invitation.InvitedByUserId,
            InvitedByName = invitedBy != null ? $"{invitedBy.FirstName} {invitedBy.LastName}" : null,
            Role = invitation.Role,
            AssignedWorkstreams = invitation.AssignedWorkstreams,
            Status = invitation.Status,
            ExpiresAt = invitation.ExpiresAt,
            CreatedAt = invitation.CreatedAt,
            AcceptedAt = invitation.AcceptedAt
        };
    }

    public async Task<IEnumerable<InvitationDto>> GetPendingInvitationsAsync()
    {
        var invitations = await _context.UserInvitations
            .Include(i => i.InvitedBy)
            .Where(i => i.Status == InvitationStatus.Pending && i.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invitations.Select(i => new InvitationDto
        {
            Id = i.Id,
            Email = i.Email,
            FirstName = i.FirstName,
            LastName = i.LastName,
            InvitedByUserId = i.InvitedByUserId,
            InvitedByName = i.InvitedBy != null ? $"{i.InvitedBy.FirstName} {i.InvitedBy.LastName}" : null,
            Role = i.Role,
            AssignedWorkstreams = i.AssignedWorkstreams,
            Status = i.Status,
            ExpiresAt = i.ExpiresAt,
            CreatedAt = i.CreatedAt,
            AcceptedAt = i.AcceptedAt
        });
    }

    public async Task<IEnumerable<InvitationDto>> GetAllInvitationsAsync()
    {
        var invitations = await _context.UserInvitations
            .Include(i => i.InvitedBy)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return invitations.Select(i => new InvitationDto
        {
            Id = i.Id,
            Email = i.Email,
            FirstName = i.FirstName,
            LastName = i.LastName,
            InvitedByUserId = i.InvitedByUserId,
            InvitedByName = i.InvitedBy != null ? $"{i.InvitedBy.FirstName} {i.InvitedBy.LastName}" : null,
            Role = i.Role,
            AssignedWorkstreams = i.AssignedWorkstreams,
            Status = i.Status,
            ExpiresAt = i.ExpiresAt,
            CreatedAt = i.CreatedAt,
            AcceptedAt = i.AcceptedAt
        });
    }

    public async Task<ValidateInvitationDto> ValidateTokenAsync(string token)
    {
        var invitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Token == token);

        if (invitation == null)
        {
            return new ValidateInvitationDto
            {
                IsValid = false,
                Message = "Lien d'invitation invalide."
            };
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            return new ValidateInvitationDto
            {
                IsValid = false,
                Message = "Cette invitation a déjà été utilisée ou annulée."
            };
        }

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.Expired;
            await _context.SaveChangesAsync();

            return new ValidateInvitationDto
            {
                IsValid = false,
                Message = "Cette invitation a expiré."
            };
        }

        return new ValidateInvitationDto
        {
            IsValid = true,
            Email = invitation.Email,
            FirstName = invitation.FirstName,
            LastName = invitation.LastName,
            Role = invitation.Role,
            AssignedWorkstreams = invitation.AssignedWorkstreams
        };
    }

    public async Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto)
    {
        var invitation = await _context.UserInvitations
            .FirstOrDefaultAsync(i => i.Token == dto.Token && i.Status == InvitationStatus.Pending);

        if (invitation == null || invitation.ExpiresAt < DateTime.UtcNow)
        {
            return false;
        }

        // Vérifier que l'utilisateur n'existe pas déjà
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == invitation.Email);
        if (existingUser != null)
        {
            return false;
        }

        // Créer le nouvel utilisateur
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = invitation.Email,
            FirstName = invitation.FirstName ?? string.Empty,
            LastName = invitation.LastName ?? string.Empty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = invitation.Role,
            AssignedWorkstreams = invitation.AssignedWorkstreams != null && invitation.AssignedWorkstreams.Any() 
                ? string.Join(",", invitation.AssignedWorkstreams) 
                : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Mettre à jour l'invitation
        invitation.Status = InvitationStatus.Accepted;
        invitation.AcceptedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Envoyer l'email de bienvenue
        try
        {
            await _emailService.SendWelcomeEmailAsync(
                user.Email,
                user.FirstName
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[INVITATION] Failed to send welcome email: {ex.Message}");
        }

        return true;
    }

    public async Task<bool> CancelInvitationAsync(Guid invitationId)
    {
        var invitation = await _context.UserInvitations.FindAsync(invitationId);
        
        if (invitation == null || invitation.Status != InvitationStatus.Pending)
        {
            return false;
        }

        invitation.Status = InvitationStatus.Cancelled;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ResendInvitationAsync(Guid invitationId)
    {
        var invitation = await _context.UserInvitations.FindAsync(invitationId);
        
        if (invitation == null || invitation.Status != InvitationStatus.Pending)
        {
            return false;
        }

        // Générer un nouveau token et prolonger l'expiration
        invitation.Token = Guid.NewGuid().ToString("N");
        invitation.ExpiresAt = DateTime.UtcNow.AddDays(7);
        
        await _context.SaveChangesAsync();

        // TODO: Envoyer l'email avec le nouveau lien
        
        return true;
    }
}
