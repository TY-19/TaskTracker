namespace TaskTracker.Domain.Common;

/// <summary>
/// Represents the set of default user's roles names in the application.
/// Is user to seed default data and ensure authorization by roles.
/// </summary>
public static class DefaultRolesNames
{
    /// <summary>
    /// Default role of the system administrator
    /// </summary>
    public const string DEFAULT_ADMIN_ROLE = "Administrator";
    /// <summary>
    /// Default role of the manager
    /// </summary>
    public const string DEFAULT_MANAGER_ROLE = "Manager";
    /// <summary>
    /// Default role of the common employee
    /// </summary>
    public const string DEFAULT_EMPLOYEE_ROLE = "Employee";
}
