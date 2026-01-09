namespace Breizh360.Data;

/// <summary>
/// Options de configuration pour le module <c>Breizh360.Data</c>.
/// </summary>
public sealed class Breizh360DataOptions
{
    /// <summary>
    /// Nom de la connection string (section <c>ConnectionStrings</c>) à utiliser.
    /// Par défaut : <c>Postgres</c>.
    /// </summary>
    public string ConnectionStringName { get; set; } = "Postgres";

    /// <summary>
    /// Active les erreurs détaillées EF Core (utile en DEV).
    /// </summary>
    public bool EnableDetailedErrors { get; set; }

    /// <summary>
    /// Active le logging de données sensibles EF Core (utile en DEV uniquement).
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; }

    /// <summary>
    /// Active le pooling DbContext (optionnel). Laisser <c>false</c> par défaut.
    /// </summary>
    public bool UseDbContextPooling { get; set; }
}
