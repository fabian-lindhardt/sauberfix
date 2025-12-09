using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sauberfix.Data
{
    public class Mitarbeiter
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        // Hier speichern wir später KEIN Klartext-Passwort, sondern einen Hash!
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Rolle: "Admin" (Verwaltung) oder "User" (Mitarbeiter)
        public string Rolle { get; set; } = "User"; 

        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;

        // Ein Mitarbeiter hat viele Termine zugewiesen bekommen
        public List<Termin> Termine { get; set; } = new();
    }
}
