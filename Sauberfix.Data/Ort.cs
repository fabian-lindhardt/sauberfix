using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Sauberfix.Data
{
    public class Ort
    {
        public int Id { get; set; }

        [Required]
        [StringLength(5)] // Deutsche PLZ sind 5-stellig
        public string Plz { get; set; } = string.Empty;

        [Required]
        public string StadtName { get; set; } = string.Empty;

        // Navigation: Ein Ort kann viele Kunden haben
        // (Wichtig für EF Core, um die Beziehung zu erkennen)
        public List<Kunde> Kunden { get; set; } = new();
    }
}
