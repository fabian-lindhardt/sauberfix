Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports Sauberfix 
Imports System

Namespace Services
    Public Class TerminService
        Private ReadOnly _db As AppDbContext

        Public Sub New(db As AppDbContext)
            _db = db
        End Sub

        Public Function CreateTermin(input As CreateTerminDto) As TerminResponseDto
            Dim kunde = _db.Kunden.Find(input.KundeId)
            Dim mitarbeiter = _db.Mitarbeiter.Find(input.MitarbeiterId)

            If kunde Is Nothing Or mitarbeiter Is Nothing Then
                Throw New ArgumentException("Kunde oder Mitarbeiter nicht gefunden.")
            End If

            CheckKollision(input.MitarbeiterId, input.DatumUhrzeit, input.Endzeit, Nothing)

            Dim termin As New Termin() With {
                .DatumUhrzeit = input.DatumUhrzeit,
                .Endzeit = input.Endzeit,
                .Beschreibung = input.Beschreibung,
                .KundeId = input.KundeId,
                .MitarbeiterId = input.MitarbeiterId,
                .Status = TerminStatus.Geplant,
                .ErstelltAm = DateTime.UtcNow
            }

            _db.Termine.Add(termin)
            _db.SaveChanges()
            return GetOne(termin.Id)
        End Function

        Public Function UpdateTermin(id As Integer, input As CreateTerminDto) As TerminResponseDto
            Dim t = _db.Termine.Find(id)
            If t Is Nothing Then Throw New ArgumentException("Termin nicht gefunden")

            CheckKollision(input.MitarbeiterId, input.DatumUhrzeit, input.Endzeit, id)

            t.DatumUhrzeit = input.DatumUhrzeit
            t.Endzeit = input.Endzeit
            t.Beschreibung = input.Beschreibung
            t.KundeId = input.KundeId
            t.MitarbeiterId = input.MitarbeiterId

            _db.SaveChanges()
            Return GetOne(t.Id)
        End Function

        Public Function GetAllTermine(uid As Integer, role As String) As List(Of TerminResponseDto)
            Dim query = _db.Termine.Include(Function(t) t.Kunde).Include(Function(t) t.Mitarbeiter).AsQueryable()
            If role <> "Admin" Then query = query.Where(Function(t) t.MitarbeiterId = uid)
            
            Dim liste = query.ToList()
            Dim res As New List(Of TerminResponseDto)
            For Each t In liste
                res.Add(MapToDto(t))
            Next
            Return res
        End Function

        Public Sub DeleteTermin(id As Integer)
            Dim t = _db.Termine.Find(id)
            If t IsNot Nothing Then
                _db.Termine.Remove(t)
                _db.SaveChanges()
            End If
        End Sub

        Private Sub CheckKollision(mitarbeiterId As Integer, startNeu As DateTime, endeNeu As DateTime, ignoreTerminId As Integer?)
            Dim query = _db.Termine.Where(Function(t) t.MitarbeiterId = mitarbeiterId AndAlso t.Status <> TerminStatus.Storniert)

            If ignoreTerminId.HasValue Then
                query = query.Where(Function(t) t.Id <> ignoreTerminId.Value)
            End If

            Dim existierendeTermine = query.ToList()

            For Each t In existierendeTermine
                Dim startAlt = t.DatumUhrzeit
                Dim endeAlt = t.Endzeit

                If startNeu < endeAlt AndAlso endeNeu > startAlt Then
                    Throw New ArgumentException($"Kollision! Der Mitarbeiter hat bereits einen Termin von {startAlt.ToString("HH:mm")} bis {endeAlt.ToString("HH:mm")}.")
                End If
            Next
        End Sub

        Private Function GetOne(id As Integer) As TerminResponseDto
             Dim t = _db.Termine.Include(Function(x) x.Kunde).Include(Function(x) x.Mitarbeiter).FirstOrDefault(Function(x) x.Id = id)
             Return MapToDto(t)
        End Function

        Private Function MapToDto(t As Termin) As TerminResponseDto
            Return New TerminResponseDto() With {
                .Id = t.Id,
                .DatumUhrzeit = t.DatumUhrzeit,
                .Endzeit = t.Endzeit,
                .Beschreibung = t.Beschreibung,
                .Status = t.Status.ToString(),
                .KundeId = t.KundeId,
                .KundeName = If(t.Kunde IsNot Nothing, t.Kunde.Vorname & " " & t.Kunde.Nachname, "?"),
                .KundeFirma = If(t.Kunde IsNot Nothing, t.Kunde.Firma, ""),
                .MitarbeiterId = t.MitarbeiterId,
                .MitarbeiterName = If(t.Mitarbeiter IsNot Nothing, t.Mitarbeiter.Vorname & " " & t.Mitarbeiter.Nachname, "?"),
                .ErinnerungVerschickt = t.ErinnerungVerschickt
            }
        End Function
    End Class
End Namespace
