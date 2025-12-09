using System;
using System.ComponentModel.DataAnnotations;

namespace Sauberfix.Data
{
    public enum TerminStatus
    {
        Geplant,
        Erledigt,
        Storniert
    }

    public class Termin
    {
        public int Id { get; set; }
        public DateTime DatumUhrzeit { get; set; }
        public string Beschreibung { get; set; } = string.Empty;
        public TerminStatus Status { get; set; } = TerminStatus.Geplant;

        // --- NEUES FELD ---
        public bool ErinnerungVerschickt { get; set; } = false;

        public int KundeId { get; set; }
        public Kunde? Kunde { get; set; }

        public int? MitarbeiterId { get; set; }
        public Mitarbeiter? Mitarbeiter { get; set; }
        
        public DateTime ErstelltAm { get; set; } = DateTime.UtcNow;
    }
}
