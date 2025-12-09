Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports Sauberfix 
Imports System.Security.Cryptography
Imports System.Text
Imports System

Namespace Services
    Public Class MitarbeiterService
        Private ReadOnly _db As AppDbContext

        Public Sub New(db As AppDbContext)
            _db = db
        End Sub

        Public Function CreateMitarbeiter(input As CreateMitarbeiterDto) As MitarbeiterResponseDto
            If String.IsNullOrEmpty(input.Username) Or String.IsNullOrEmpty(input.Password) Then 
                Throw New ArgumentException("Pflichtfelder fehlen.")
            End If

            If _db.Mitarbeiter.Any(Function(existing) existing.Username = input.Username) Then 
                Throw New ArgumentException("Username vergeben.")
            End If
            
            Dim hash = HashPassword(input.Password)
            
            Dim newM As New Mitarbeiter() With { 
                .Username = input.Username, 
                .PasswordHash = hash, 
                .Vorname = input.Vorname, 
                .Nachname = input.Nachname, 
                .Rolle = input.Rolle 
            }
            
            _db.Mitarbeiter.Add(newM)
            _db.SaveChanges()
            Return MapToDto(newM)
        End Function

        Public Function UpdateMitarbeiter(id As Integer, input As CreateMitarbeiterDto) As MitarbeiterResponseDto
            Dim m = _db.Mitarbeiter.Find(id)
            If m Is Nothing Then Throw New ArgumentException("Mitarbeiter nicht gefunden")

            m.Vorname = input.Vorname
            m.Nachname = input.Nachname
            m.Rolle = input.Rolle
            
            If Not String.IsNullOrEmpty(input.Password) Then
                m.PasswordHash = HashPassword(input.Password)
            End If
            
            _db.SaveChanges()
            Return MapToDto(m)
        End Function

        Public Function GetAllMitarbeiter() As List(Of MitarbeiterResponseDto)
            ' 1. Daten aus der DB laden (SQL wird hier ausgeführt)
            Dim entities = _db.Mitarbeiter.ToList()
            
            ' 2. Ergebnis-Liste erstellen
            Dim dtos As New List(Of MitarbeiterResponseDto)()
            
            ' 3. Manuell mappen (sicherste Methode)
            For Each m In entities
                dtos.Add(MapToDto(m))
            Next
            
            Return dtos
        End Function
        
        Public Sub DeleteMitarbeiter(id As Integer)
            Dim m = _db.Mitarbeiter.Find(id)
            If m IsNot Nothing Then
                _db.Mitarbeiter.Remove(m)
                _db.SaveChanges()
            End If
        End Sub

        ' --- WICHTIG: Shared (Statisch) gemacht ---
        Private Shared Function HashPassword(pw As String) As String
            Using sha256 As SHA256 = SHA256.Create()
                Return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(pw)))
            End Using
        End Function

        ' --- WICHTIG: Shared (Statisch) gemacht ---
        Private Shared Function MapToDto(m As Mitarbeiter) As MitarbeiterResponseDto
            Return New MitarbeiterResponseDto() With { 
                .Id = m.Id, 
                .Username = m.Username, 
                .Vorname = m.Vorname, 
                .Nachname = m.Nachname, 
                .Rolle = m.Rolle 
            }
        End Function
    End Class
End Namespace
