Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports BCrypt.Net

Public Class DatabaseSeeder
    Public Shared Sub SeedDatabase(db As AppDbContext)
        Console.WriteLine(">>> Checking if database needs seeding...")

        ' Fix Endzeit for existing appointments
        FixEndzeitForExistingAppointments(db)

        If db.Mitarbeiter.Any() Then
            Console.WriteLine(">>> Database already contains users. Skipping seed.")
            Return
        End If

        Console.WriteLine(">>> Seeding database with initial admin user...")

        Dim password As String = "admin123"
        Dim passwordHash As String = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12))

        Dim admin As New Mitarbeiter With {
            .Username = "admin",
            .PasswordHash = passwordHash,
            .Rolle = "Admin",
            .Vorname = "System",
            .Nachname = "Administrator"
        }

        db.Mitarbeiter.Add(admin)
        db.SaveChanges()

        Console.WriteLine(">>> SUCCESS: Admin user created!")
        Console.WriteLine(">>>   Username: admin")
        Console.WriteLine(">>>   Password: admin123")
    End Sub

    Public Shared Sub SeedSampleData(db As AppDbContext)
        Console.WriteLine(">>> Starting SAMPLE DATA seeding...")

        ' 1. Orte erstellen
        If Not db.Orte.Any() Then
            Console.WriteLine(">>> Creating sample locations (Orte)...")
            Dim orte = New List(Of Ort) From {
                New Ort With {.Plz = "10115", .StadtName = "Berlin"},
                New Ort With {.Plz = "20095", .StadtName = "Hamburg"},
                New Ort With {.Plz = "80331", .StadtName = "München"},
                New Ort With {.Plz = "50667", .StadtName = "Köln"},
                New Ort With {.Plz = "60311", .StadtName = "Frankfurt am Main"}
            }
            db.Orte.AddRange(orte)
            db.SaveChanges()
        End If
        Dim allOrte = db.Orte.ToList()

        ' 2. Kunden erstellen
        If db.Kunden.Count() < 10 Then
            Console.WriteLine(">>> Creating 50 sample customers...")
            Dim vornamen = {"Max", "Maria", "Julia", "Felix", "David", "Laura", "Thomas", "Lisa", "Michael", "Sarah"}
            Dim nachnamen = {"Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann"}
            
            Dim rand As New Random()
            Dim kundenListe As New List(Of Kunde)

            For i As Integer = 1 To 50
                Dim vn = vornamen(rand.Next(vornamen.Length))
                Dim nn = nachnamen(rand.Next(nachnamen.Length))
                Dim ort = allOrte(rand.Next(allOrte.Count))

                ' E-Mail Format: vorname.nachname@flairtec.de
                ' Wir hängen optional eine Zufallszahl an, falls wir Duplikate vermeiden wollen, 
                ' aber bei 50 Leuten und 100 Kombinationen kann es Kollisionen geben.
                ' Der User wollte "catch-all", also machen wir es einfach.
                ' Damit es Unique bleibt (falls Constraint), hängen wir ggf. Index an, 
                ' aber der User-Wunsch war explizit vorname.nachname@flairtec.de. 
                ' Wir machen es ohne Index, hoffen auf Zufall, oder bauen simple Logik.
                
                Dim emailBase = $"{vn}.{nn}".ToLower()
                Dim email = $"{emailBase}@flairtec.de"
                
                ' Einfacher Dubletten-Check im Speicher für diesen Lauf
                Dim counter = 1
                Dim finalEmail = email
                While kundenListe.Any(Function(k) k.Email = finalEmail)
                    finalEmail = $"{emailBase}{counter}@flairtec.de"
                    counter += 1
                End While

                kundenListe.Add(New Kunde With {
                    .Vorname = vn,
                    .Nachname = nn,
                    .Email = finalEmail,
                    .Telefon = "0123456789",
                    .Strasse = "Musterstraße " & rand.Next(1, 100),
                    .Ort = ort
                })
            Next
            db.Kunden.AddRange(kundenListe)
            db.SaveChanges()
        End If

        ' 3. Mitarbeiter erstellen
        If db.Mitarbeiter.Count() < 5 Then
            Console.WriteLine(">>> Creating 5 sample employees...")
            Dim passwordHash = BCrypt.Net.BCrypt.HashPassword("user123", BCrypt.Net.BCrypt.GenerateSalt(12))
            
            For i As Integer = 1 To 5
                db.Mitarbeiter.Add(New Mitarbeiter With {
                    .Username = $"user{i}",
                    .PasswordHash = passwordHash,
                    .Rolle = "User",
                    .Vorname = $"Mitarbeiter",
                    .Nachname = $"{i}"
                })
            Next
            db.SaveChanges()
        End If

        ' 4. Termine erstellen
        If db.Termine.Count() < 10 Then
            Console.WriteLine(">>> Creating 50 sample appointments...")
            Dim kunden = db.Kunden.ToList()
            Dim mitarbeiter = db.Mitarbeiter.ToList()
            Dim rand As New Random()

            For i As Integer = 1 To 50
                Dim k = kunden(rand.Next(kunden.Count))
                Dim m = mitarbeiter(rand.Next(mitarbeiter.Count))
                
                ' Termin in den nächsten 30 Tagen
                Dim daysOffset = rand.Next(1, 30)
                Dim hour = rand.Next(8, 17)
                Dim start = Date.Today.AddDays(daysOffset).AddHours(hour)

                db.Termine.Add(New Termin With {
                    .DatumUhrzeit = start.ToUniversalTime(),
                    .Endzeit = start.AddHours(1).ToUniversalTime(),
                    .Beschreibung = $"Reinigung bei {k.Firma} ({k.Nachname})",
                    .Status = TerminStatus.Geplant,
                    .Kunde = k,
                    .Mitarbeiter = m,
                    .ErinnerungVerschickt = False,
                    .ErinnerungVorlaufMinuten = If(rand.NextDouble() > 0.5, 1440, 2880) ' 1 oder 2 Tage
                })
            Next
            db.SaveChanges()
        End If

        Console.WriteLine(">>> SAMPLE DATA seeding completed.")
    End Sub

    Private Shared Sub FixEndzeitForExistingAppointments(db As AppDbContext)
        Dim invalidTermine = db.Termine.Where(Function(t) t.Endzeit < t.DatumUhrzeit Or t.Endzeit = New DateTime(1, 1, 1)).ToList()

        If invalidTermine.Count > 0 Then
            Console.WriteLine($">>> Fixing Endzeit for {invalidTermine.Count} appointments...")

            For Each termin In invalidTermine
                termin.Endzeit = termin.DatumUhrzeit.AddMinutes(60)
            Next

            db.SaveChanges()
            Console.WriteLine(">>> Endzeit fixed for all appointments!")
        End If
    End Sub
End Class
