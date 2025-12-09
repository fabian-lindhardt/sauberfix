Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports Sauberfix
Imports Microsoft.Extensions.Configuration
Imports Microsoft.IdentityModel.Tokens
Imports System.IdentityModel.Tokens.Jwt
Imports System.Security.Claims
Imports System.Text
Imports System.Security.Cryptography

Namespace Services
    Public Class AuthService
        Private ReadOnly _db As AppDbContext
        Private ReadOnly _config As IConfiguration

        Public Sub New(db As AppDbContext, config As IConfiguration)
            _db = db
            _config = config
        End Sub

        Public Function Login(input As LoginDto) As LoginResponseDto
            ' 1. User suchen
            Dim user = _db.Mitarbeiter.FirstOrDefault(Function(m) m.Username = input.Username)
            If user Is Nothing Then Return Nothing ' User nicht gefunden

            ' 2. Passwort prüfen (Hash vergleichen)
            Dim inputHash As String = String.Empty
            Using sha256 As SHA256 = SHA256.Create()
                Dim bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input.Password))
                inputHash = Convert.ToBase64String(bytes)
            End Using

            If user.PasswordHash <> inputHash Then Return Nothing ' Passwort falsch

            ' 3. JWT Token bauen (Der digitale Ausweis)
            Dim tokenHandler As New JwtSecurityTokenHandler()
            Dim key = Encoding.UTF8.GetBytes(_config("JwtSettings:Key"))
            
            ' Was steht im Ausweis? (Claims)
            Dim claims As New List(Of Claim) From {
                New Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                New Claim(ClaimTypes.Name, user.Username),
                New Claim(ClaimTypes.Role, user.Rolle)
            }

            Dim tokenDescriptor As New SecurityTokenDescriptor With {
                .Subject = New ClaimsIdentity(claims),
                .Expires = DateTime.UtcNow.AddHours(8), ' Gültig für einen Arbeitstag
                .Issuer = _config("JwtSettings:Issuer"),
                .Audience = _config("JwtSettings:Audience"),
                .SigningCredentials = New SigningCredentials(New SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            }

            Dim token = tokenHandler.CreateToken(tokenDescriptor)
            Dim tokenString = tokenHandler.WriteToken(token)

            Return New LoginResponseDto() With {
                .Token = tokenString,
                .Username = user.Username,
                .Rolle = user.Rolle
            }
        End Function
    End Class
End Namespace
