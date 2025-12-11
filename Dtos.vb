' --- KUNDEN ---
Public Class CreateKundeDto
    Public Property Vorname As String
    Public Property Nachname As String
    Public Property Strasse As String
    Public Property Plz As String 
    Public Property Stadt As String
    Public Property Email As String
    Public Property Telefon As String
End Class

Public Class KundeResponseDto
    Public Property Id As Integer
    Public Property Vorname As String
    Public Property Nachname As String
    Public Property Plz As String
    Public Property Stadt As String
    ' --- NEU HINZUGEFÜGT ---
    Public Property Strasse As String
    Public Property Email As String
    Public Property Telefon As String
End Class

' --- MITARBEITER ---
Public Class CreateMitarbeiterDto
    Public Property Username As String
    Public Property Password As String
    Public Property Vorname As String
    Public Property Nachname As String
    Public Property Rolle As String = "User"
End Class

Public Class MitarbeiterResponseDto
    Public Property Id As Integer
    Public Property Username As String
    Public Property Vorname As String
    Public Property Nachname As String
    Public Property Rolle As String
End Class

' --- TERMINE ---
Public Class CreateTerminDto
    Public Property DatumUhrzeit As DateTime
    Public Property Endzeit As DateTime
    Public Property Beschreibung As String
    Public Property KundeId As Integer
    Public Property MitarbeiterId As Integer
End Class

Public Class TerminResponseDto
    Public Property Id As Integer
    Public Property DatumUhrzeit As DateTime
    Public Property Endzeit As DateTime
    Public Property Beschreibung As String
    Public Property Status As String
    Public Property KundeName As String
    Public Property MitarbeiterName As String
    Public Property ErinnerungVerschickt As Boolean
End Class

' --- LOGIN ---
Public Class LoginDto
    Public Property Username As String
    Public Property Password As String
End Class

Public Class LoginResponseDto
    Public Property Token As String
    Public Property Username As String
    Public Property Rolle As String
End Class
