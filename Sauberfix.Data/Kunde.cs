using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sauberfix.Data
{
    public class Kunde
    {
        public int Id { get; set; }

        [Required]
        public string Vorname { get; set; } = string.Empty;

        [Required]
        public string Nachname { get; set; } = string.Empty;

        public string Firma { get; set; } = string.Empty;

        public string Strasse { get; set; } = string.Empty;
        
        // --- ÄNDERUNG START (3. Normalform) ---
        // Statt PLZ und Stadt als Text speichern wir nur die ID des Ortes
        public int OrtId { get; set; }
        public Ort? Ort { get; set; }
        // --- ÄNDERUNG ENDE ---

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public string Telefon { get; set; } = string.Empty;

        public List<Termin> Termine { get; set; } = new();
    }
}
