using DigitaEnergy.ProjectTracker.Application.Interfaces;
using DigitaEnergy.ProjectTracker.Domain.Enums;
using DigitaEnergy.ProjectTracker.Domain.Interfaces;

namespace DigitaEnergy.ProjectTracker.Application.Services;

public class WorkstreamAuthorizationService : IWorkstreamAuthorizationService
{
    private readonly IUserRepository _userRepository;

    public WorkstreamAuthorizationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> IsProjectManagerAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.Role == UserRole.PROJECT_MANAGER;
    }

    public async Task<bool> IsStreamLeadAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.Role == UserRole.STREAM_LEAD;
    }

    public async Task<bool> IsTeamMemberAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.Role == UserRole.TEAM_MEMBER;
    }

    public bool HasAccessToWorkstream(Guid userId, Workstream workstream)
    {
        // Cette méthode sera appelée de manière synchrone après avoir récupéré l'utilisateur
        throw new NotImplementedException("Use GetAccessibleWorkstreamsAsync instead");
    }

    public async Task<List<Workstream>> GetAccessibleWorkstreamsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return new List<Workstream>();
        }

        // PROJECT_MANAGER a accès à tous les workstreams
        if (user.Role == UserRole.PROJECT_MANAGER)
        {
            return Enum.GetValues<Workstream>().ToList();
        }

        // STREAM_LEAD peut voir tous les workstreams (pour la vue Gantt)
        if (user.Role == UserRole.STREAM_LEAD)
        {
            return Enum.GetValues<Workstream>().ToList();
        }

        // TEAM_MEMBER a accès en lecture à tous les workstreams
        if (user.Role == UserRole.TEAM_MEMBER)
        {
            return Enum.GetValues<Workstream>().ToList();
        }

        return new List<Workstream>();
    }

    public async Task<List<Workstream>> GetFilteredWorkstreamsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return new List<Workstream>();
        }

        // PROJECT_MANAGER voit tous les workstreams
        if (user.Role == UserRole.PROJECT_MANAGER)
        {
            return Enum.GetValues<Workstream>().ToList();
        }

        // STREAM_LEAD voit uniquement ses workstreams assignés (pour les onglets tâches/risques/jalons)
        if (user.Role == UserRole.STREAM_LEAD)
        {
            if (string.IsNullOrWhiteSpace(user.AssignedWorkstreams))
            {
                return new List<Workstream>();
            }

            var workstreamNames = user.AssignedWorkstreams.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var workstreams = new List<Workstream>();

            foreach (var name in workstreamNames)
            {
                if (Enum.TryParse<Workstream>(name.Trim(), out var workstream))
                {
                    workstreams.Add(workstream);
                }
            }

            return workstreams;
        }

        // TEAM_MEMBER voit tous les workstreams
        if (user.Role == UserRole.TEAM_MEMBER)
        {
            return Enum.GetValues<Workstream>().ToList();
        }

        return new List<Workstream>();
    }

    public async Task<bool> CanModifyInWorkstreamAsync(Guid userId, Workstream workstream)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return false;
        }

        // PROJECT_MANAGER peut tout modifier
        if (user.Role == UserRole.PROJECT_MANAGER)
        {
            return true;
        }

        // STREAM_LEAD peut modifier uniquement dans ses workstreams
        if (user.Role == UserRole.STREAM_LEAD)
        {
            var accessibleWorkstreams = await GetAccessibleWorkstreamsAsync(userId);
            return accessibleWorkstreams.Contains(workstream);
        }

        // TEAM_MEMBER ne peut pas modifier (sauf ses propres tâches - à gérer séparément)
        return false;
    }
}
