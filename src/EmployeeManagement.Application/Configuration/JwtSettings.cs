namespace EmployeeManagement.Application.Configuration;

/// <summary>
/// Strongly-typed configuration POCO for JSON Web Token (JWT) settings.
/// Bound from "JwtSettings" section in appsettings.json.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Secret symmetric key for HMAC-SHA256 signing (minimum 256 bits / 32 characters).
    /// </summary>
    public string SecretKey { get; set; } = "SuperSecretKeyForDotNetTrainingCapstoneGate2MustBeLongEnough256Bit!";

    /// <summary>
    /// Expected token issuer (API name).
    /// </summary>
    public string Issuer { get; set; } = "EmployeeManagementApi";

    /// <summary>
    /// Expected token audience (client applications).
    /// </summary>
    public string Audience { get; set; } = "EmployeeManagementClients";

    /// <summary>
    /// Token lifetime in minutes before expiration.
    /// </summary>
    public int ExpiryMinutes { get; set; } = 60;
}
