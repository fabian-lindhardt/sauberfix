Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports Sauberfix 

Namespace Services
    Public Class TerminService
        Private ReadOnly _db As AppDbContext
        Public Sub New(db As AppDbContext)
            _db = db
        End Sub

        Public Function CreateTermin(input As CreateTerminDto) As TerminResponseDto
            Dim termin As New Termin() With {
                .DatumUhrzeit = input.DatumUhrzeit, .Beschreibung = input.Beschreibung,
                .KundeId = input.KundeId, .MitarbeiterId = input.MitarbeiterId, .Status = TerminStatus.Geplant, .ErstelltAm = DateTime.UtcNow
            }
            _db.Termine.Add(termin)
            _db.SaveChanges()
            return GetOne(termin.Id)
        End Function

        ' --- NEU: UPDATE ---
        Public Function UpdateTermin(id As Integer, input As CreateTerminDto) As TerminResponseDto
            Dim t = _db.Termine.Find(id)
            If t Is Nothing Then Throw New ArgumentException("Termin nicht gefunden")

            t.DatumUhrzeit = input.DatumUhrzeit
            t.Beschreibung = input.Beschreibung
            t.KundeId = input.KundeId
            t.MitarbeiterId = input.MitarbeiterId
            ' Status lassen wir hier erstmal, könnte man auch updatebar machen
            
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

        Private Function GetOne(id As Integer) As TerminResponseDto
             Dim t = _db.Termine.Include(Function(x) x.Kunde).Include(Function(x) x.Mitarbeiter).FirstOrDefault(Function(x) x.Id = id)
             Return MapToDto(t)
        End Function

        Private Function MapToDto(t As Termin) As TerminResponseDto
            Return New TerminResponseDto() With {
                .Id = t.Id, .DatumUhrzeit = t.DatumUhrzeit, .Beschreibung = t.Beschreibung, .Status = t.Status.ToString(),
                .KundeName = If(t.Kunde IsNot Nothing, t.Kunde.Vorname & " " & t.Kunde.Nachname, "?"),
                .MitarbeiterName = If(t.Mitarbeiter IsNot Nothing, t.Mitarbeiter.Vorname & " " & t.Mitarbeiter.Nachname, "?"),
                .ErinnerungVerschickt = t.ErinnerungVerschickt
            }
        End Function
    End Class
End Namespace
