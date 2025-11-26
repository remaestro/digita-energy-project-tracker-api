using DigitaEnergy.ProjectTracker.Application.DTOs.Auth;
using DigitaEnergy.ProjectTracker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitaEnergy.ProjectTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "PROJECT_MANAGER")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les utilisateurs (PROJECT_MANAGER uniquement)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { Data = users, TotalCount = users.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
            return StatusCode(500, new { Message = "Erreur serveur lors de la récupération des utilisateurs" });
        }
    }

    /// <summary>
    /// Récupère un utilisateur par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"Utilisateur avec l'ID {id} introuvable" });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {UserId}", id);
            return StatusCode(500, new { Message = "Erreur serveur lors de la récupération de l'utilisateur" });
        }
    }

    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(RegisterRequestDto request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de l'utilisateur");
            return StatusCode(500, new { Message = "Erreur serveur lors de la création de l'utilisateur" });
        }
    }

    /// <summary>
    /// Met à jour un utilisateur existant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserDto request)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, request);
            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur {UserId}", id);
            return StatusCode(500, new { Message = "Erreur serveur lors de la mise à jour de l'utilisateur" });
        }
    }

    /// <summary>
    /// Supprime un utilisateur
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de l'utilisateur {UserId}", id);
            return StatusCode(500, new { Message = "Erreur serveur lors de la suppression de l'utilisateur" });
        }
    }

    /// <summary>
    /// Récupère tous les Stream Leads (pour affectation de tâches)
    /// </summary>
    [HttpGet("stream-leads")]
    [Authorize] // Accessible par tous les utilisateurs authentifiés
    public async Task<ActionResult<IEnumerable<UserDto>>> GetStreamLeads()
    {
        try
        {
            var streamLeads = await _userService.GetStreamLeadsAsync();
            return Ok(new { Data = streamLeads, TotalCount = streamLeads.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des Stream Leads");
            return StatusCode(500, new { Message = "Erreur serveur lors de la récupération des Stream Leads" });
        }
    }
}
