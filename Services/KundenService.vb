Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports Sauberfix 
Imports System

Namespace Services
    Public Class KundenService
        Private ReadOnly _db As AppDbContext
        Public Sub New(db As AppDbContext)
            _db = db
        End Sub

        Public Function CreateKunde(input As CreateKundeDto) As KundeResponseDto
            ' Validierung: Mindestens Vorname und Nachname müssen angegeben werden
            If String.IsNullOrWhiteSpace(input.Vorname) Then
                Throw New ArgumentException("Vorname ist erforderlich")
            End If
            If String.IsNullOrWhiteSpace(input.Nachname) Then
                Throw New ArgumentException("Nachname ist erforderlich")
            End If
            If String.IsNullOrWhiteSpace(input.Plz) Then
                Throw New ArgumentException("PLZ ist erforderlich")
            End If
            If String.IsNullOrWhiteSpace(input.Stadt) Then
                Throw New ArgumentException("Stadt ist erforderlich")
            End If

            Dim ort = GetOrCreateOrt(input.Plz, input.Stadt)

            Dim k As New Kunde() With {
                .Vorname = input.Vorname,
                .Nachname = input.Nachname,
                .Firma = input.Firma,
                .Strasse = input.Strasse,
                .Email = input.Email,
                .Telefon = input.Telefon,
                .Ort = ort
            }
            _db.Kunden.Add(k)
            _db.SaveChanges()
            Return MapToDto(k)
        End Function

        Public Function UpdateKunde(id As Integer, input As CreateKundeDto) As KundeResponseDto
            ' Validierung
            If String.IsNullOrWhiteSpace(input.Vorname) Then
                Throw New ArgumentException("Vorname ist erforderlich")
            End If
            If String.IsNullOrWhiteSpace(input.Nachname) Then
                Throw New ArgumentException("Nachname ist erforderlich")
            End If
            If String.IsNullOrWhiteSpace(input.Plz) Then
                Throw New ArgumentException("PLZ ist erforderlich")
            End If
            If String.IsNullOrWhiteSpace(input.Stadt) Then
                Throw New ArgumentException("Stadt ist erforderlich")
            End If

            Dim k = _db.Kunden.Include(Function(x) x.Ort).FirstOrDefault(Function(x) x.Id = id)
            If k Is Nothing Then Throw New ArgumentException("Kunde nicht gefunden")

            If k.Ort.Plz <> input.Plz Then
                k.Ort = GetOrCreateOrt(input.Plz, input.Stadt)
            End If

            k.Vorname = input.Vorname
            k.Nachname = input.Nachname
            k.Firma = input.Firma
            k.Strasse = input.Strasse
            k.Email = input.Email
            k.Telefon = input.Telefon
            
            _db.SaveChanges()
            Return MapToDto(k)
        End Function

        Public Function GetAllKunden() As List(Of KundeResponseDto)
            Dim daten = _db.Kunden.Include(Function(x) x.Ort).ToList()
            Dim result As New List(Of KundeResponseDto)
            For Each k In daten
                result.Add(MapToDto(k))
            Next
            Return result
        End Function

        Public Sub DeleteKunde(id As Integer)
            Dim k = _db.Kunden.Find(id)
            If k IsNot Nothing Then
                _db.Kunden.Remove(k)
                _db.SaveChanges()
            End If
        End Sub

        Private Function GetOrCreateOrt(plz As String, stadt As String) As Ort
            Dim ort = _db.Orte.FirstOrDefault(Function(o) o.Plz = plz)
            If ort Is Nothing Then
                ort = New Ort() With { .Plz = plz, .StadtName = stadt }
                _db.Orte.Add(ort)
                _db.SaveChanges()
            End If
            Return ort
        End Function

        Private Shared Function MapToDto(k As Kunde) As KundeResponseDto
            Return New KundeResponseDto() With {
                .Id = k.Id,
                .Vorname = k.Vorname,
                .Nachname = k.Nachname,
                .Firma = k.Firma,
                .Plz = If(k.Ort IsNot Nothing, k.Ort.Plz, ""),
                .Stadt = If(k.Ort IsNot Nothing, k.Ort.StadtName, ""),
                .Strasse = k.Strasse,
                .Email = k.Email,
                .Telefon = k.Telefon
            }
        End Function
    End Class
End Namespace
