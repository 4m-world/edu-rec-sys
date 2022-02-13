namespace CodeMatrix.Mepd.Application.Identity.Users;

/// <summary>
/// Toggle user status request
/// </summary>
public class ToggleUserStatusRequest
{
    /// <summary>
    /// Gets or sets a value indicates weather the user is activer or not
    /// </summary>
    public bool ActivateUser { get; set; }

    /// <summary>
    /// Gets or ses user identifier
    /// </summary>
    public string UserId { get; set; }
}
