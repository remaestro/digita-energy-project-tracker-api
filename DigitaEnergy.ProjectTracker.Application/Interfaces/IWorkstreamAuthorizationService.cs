using DigitaEnergy.ProjectTracker.Domain.Enums;

namespace DigitaEnergy.ProjectTracker.Application.Interfaces;

public interface IWorkstreamAuthorizationService
{
    /// <summary>
    /// Vérifie si l'utilisateur a accès à un workstream spécifique
    /// </summary>
    bool HasAccessToWorkstream(Guid userId, Workstream workstream);
    
    /// <summary>
    /// Obtient la liste des workstreams accessibles pour l'utilisateur (vue lecture - tous pour STREAM_LEAD)
    /// </summary>
    Task<List<Workstream>> GetAccessibleWorkstreamsAsync(Guid userId);
    
    /// <summary>
    /// Obtient la liste des workstreams filtrés pour l'utilisateur (vue filtrée - assignés uniquement pour STREAM_LEAD)
    /// </summary>
    Task<List<Workstream>> GetFilteredWorkstreamsAsync(Guid userId);
    
    /// <summary>
    /// Vérifie si l'utilisateur peut modifier une ressource dans un workstream donné
    /// </summary>
    Task<bool> CanModifyInWorkstreamAsync(Guid userId, Workstream workstream);
    
    /// <summary>
    /// Vérifie si l'utilisateur est un PROJECT_MANAGER (accès total)
    /// </summary>
    Task<bool> IsProjectManagerAsync(Guid userId);
    
    /// <summary>
    /// Vérifie si l'utilisateur est un STREAM_LEAD
    /// </summary>
    Task<bool> IsStreamLeadAsync(Guid userId);
    
    /// <summary>
    /// Vérifie si l'utilisateur est un TEAM_MEMBER
    /// </summary>
    Task<bool> IsTeamMemberAsync(Guid userId);
}
