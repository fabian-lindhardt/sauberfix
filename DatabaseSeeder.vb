Imports Sauberfix.Data
Imports System.Security.Cryptography
Imports System.Text
Imports Microsoft.EntityFrameworkCore

Public Class DatabaseSeeder
    Public Shared Sub SeedDatabase(db As AppDbContext)
        Console.WriteLine(">>> Checking if database needs seeding...")
        
        If db.Mitarbeiter.Any() Then
            Console.WriteLine(">>> Database already contains users. Skipping seed.")
            Return
        End If

        Console.WriteLine(">>> Seeding database with initial admin user...")

        Dim password As String = "admin123"
        Dim passwordHash As String = String.Empty
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
            passwordHash = Convert.ToBase64String(bytes)
        End Using

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
End Class
