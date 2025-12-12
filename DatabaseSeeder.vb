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
