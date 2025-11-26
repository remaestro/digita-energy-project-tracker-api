namespace DigitaEnergy.ProjectTracker.Domain.Enums;

public static class WorkstreamExtensions
{
    private static readonly Dictionary<Workstream, string> WorkstreamToDbValue = new()
    {
        { Workstream.GenieCivil, "Génie civil" },
        { Workstream.PostesSources, "Postes Sources (ext.)" },
        { Workstream.PostesDeLivraison, "Postes de Livraison (PL)" },
        { Workstream.LiaisonsHTA, "Liaisons HTA" },
        { Workstream.MiniStations, "Mini‑stations" },
        { Workstream.Teleconduite, "Téléconduite/SCADA" },
        { Workstream.PostesDeRepartition, "Postes de Répartition (PR)" },
        { Workstream.EssaisEtMES, "Essais & MES" },
        { Workstream.Etudes, "Études" },
        { Workstream.Achat, "Achat" }
    };

    private static readonly Dictionary<string, Workstream> DbValueToWorkstream = new()
    {
        { "Génie civil", Workstream.GenieCivil },
        { "Postes Sources (ext.)", Workstream.PostesSources },
        { "Postes de Livraison (PL)", Workstream.PostesDeLivraison },
        { "Liaisons HTA", Workstream.LiaisonsHTA },
        { "Mini‑stations", Workstream.MiniStations },  // U+2011 non-breaking hyphen
        { "Mini-stations", Workstream.MiniStations },  // U+002D regular hyphen
        { "Ministations", Workstream.MiniStations },   // No hyphen
        { "Mini stations", Workstream.MiniStations },  // Space
        { "Téléconduite/SCADA", Workstream.Teleconduite },
        { "Postes de Répartition (PR)", Workstream.PostesDeRepartition },
        { "Essais & MES", Workstream.EssaisEtMES },
        { "Études", Workstream.Etudes },
        { "Achat", Workstream.Achat }
    };

    /// <summary>
    /// Converts an enum value to its database string representation
    /// </summary>
    public static string ToDbValue(this Workstream workstream)
    {
        return WorkstreamToDbValue.TryGetValue(workstream, out var value) 
            ? value 
            : workstream.ToString();
    }

    /// <summary>
    /// Converts a database string value to its enum representation
    /// </summary>
    public static Workstream? FromDbValue(string dbValue)
    {
        return DbValueToWorkstream.TryGetValue(dbValue, out var workstream) 
            ? workstream 
            : null;
    }

    /// <summary>
    /// Tries to parse a database string value to its enum representation
    /// </summary>
    public static bool TryFromDbValue(string dbValue, out Workstream workstream)
    {
        return DbValueToWorkstream.TryGetValue(dbValue, out workstream);
    }
}
