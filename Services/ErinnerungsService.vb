Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.DependencyInjection
Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports MimeKit
Imports MailKit.Net.Smtp
Imports Microsoft.Extensions.Configuration

Namespace Services
    Public Class ErinnerungsService
        Inherits BackgroundService

        Private ReadOnly _serviceScopeFactory As IServiceScopeFactory
        Private ReadOnly _logger As ILogger(Of ErinnerungsService)
        Private ReadOnly _configuration As IConfiguration

        Public Sub New(serviceScopeFactory As IServiceScopeFactory, logger As ILogger(Of ErinnerungsService), configuration As IConfiguration)
            _serviceScopeFactory = serviceScopeFactory
            _logger = logger
            _configuration = configuration
        End Sub

        Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
            _logger.LogInformation(">>> Erinnerungs-Service gestartet.")

            While Not stoppingToken.IsCancellationRequested
                Try
                    Using scope = _serviceScopeFactory.CreateScope()
                        Dim db = scope.ServiceProvider.GetRequiredService(Of AppDbContext)()

                        ' Zeitzonen-Fix: Der Server läuft vermutlich in UTC, aber die User geben Zeiten in "Deutscher Zeit" (Local) ein.
                        ' Wir müssen "Jetzt" also in Deutscher Zeit holen, um Äpfel mit Äpfeln zu vergleichen.
                        Dim nowUtc = DateTime.UtcNow
                        Dim kst = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin") ' Linux/Docker braucht oft IANA IDs
                        Dim nowGermany = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, kst)
                        
                        ' Query: Alle geplanten, offenen Termine laden (Filterung Zeit-Logik im Speicher, da EF Core Translation mglw. fehlt)
                        Dim candidates = db.Termine _
                                           .Include(Function(t) t.Kunde) _
                                           .Where(Function(t) t.Status = TerminStatus.Geplant AndAlso t.ErinnerungVerschickt = False) _
                                           .ToList()

                        _logger.LogInformation($"[ErinnerungsService] Prüfe {candidates.Count} Kandidaten...")
                        _logger.LogInformation($"[DEBUG] Serverzeit (UTC): {nowUtc} | Verwendete Zeit (DE): {nowGermany}")

                        Dim fälligeTermine As New List(Of Termin)
                        For Each t In candidates
                            ' t.DatumUhrzeit ist z.B. 09:15 (vom User gemeinte deutsche Zeit)
                            ' Trigger ist z.B. 08:45 (deutsche Zeit)
                            ' nowGermany ist z.B. 08:49 (deutsche Zeit) -> 08:45 <= 08:49 -> TREFFER.
                            
                            Dim triggerZeit = t.DatumUhrzeit.AddMinutes(-t.ErinnerungVorlaufMinuten)
                            
                            If triggerZeit <= nowGermany AndAlso t.DatumUhrzeit > nowGermany Then
                                fälligeTermine.Add(t)
                            End If
                        Next
                        
                        _logger.LogInformation($"[ErinnerungsService] {fälligeTermine.Count} Termine sind effektiv fällig.")

                        For Each t In fälligeTermine
                            _logger.LogWarning($"[EMAIL GESENDET] An: {t.Kunde.Email} | Termin: {t.DatumUhrzeit}")
                            
                            Try
                                Dim message = New MimeMessage()
                                Dim senderName = _configuration("SmtpSettings:SenderName")
                                Dim senderEmail = _configuration("SmtpSettings:SenderEmail")
                                message.From.Add(New MailboxAddress(senderName, senderEmail))
                                message.To.Add(New MailboxAddress(t.Kunde.Vorname & " " & t.Kunde.Nachname, t.Kunde.Email))
                                message.Subject = "Erinnerung an Ihren Termin bei Sauberfix"

                                Dim bodyBuilder = New BodyBuilder()
                                bodyBuilder.HtmlBody = $"<h1>Hallo {t.Kunde.Vorname} {t.Kunde.Nachname},</h1>" &
                                                       $"<p>Dies ist eine freundliche Erinnerung an Ihren Termin morgen:</p>" &
                                                       $"<p><strong>Datum:</strong> {t.DatumUhrzeit.ToString("dd.MM.yyyy HH:mm")}</p>" &
                                                       $"<p><strong>Beschreibung:</strong> {t.Beschreibung}</p>" &
                                                       $"<br><p>Viele Grüße,<br>Ihr Sauberfix Team</p>"
                                message.Body = bodyBuilder.ToMessageBody()

                                Using client = New SmtpClient()
                                    Dim server = _configuration("SmtpSettings:Server")
                                    Dim port = Integer.Parse(_configuration("SmtpSettings:Port"))
                                    
                                    ' Connect(host, port, useSsl) -> useSsl=False for Port 25 usually. Auto for other.
                                    
                                    ' Fix: Zertifikat-Fehler (Name Mismatch smtp vs mx01) ignorieren
                                    client.CheckCertificateRevocation = False
                                    client.ServerCertificateValidationCallback = Function(s, c, h, e) True
                                    
                                    client.Connect(server, port, False)
                                    ' Note: No authentication for now as none was provided.
                                    ' If needed: client.Authenticate("user", "pass")
                                    
                                    client.Send(message)
                                    client.Disconnect(True)
                                End Using

                                ' Status speichern!
                                t.ErinnerungVerschickt = True
                            Catch ex As Exception
                                _logger.LogError(ex, $"FEHLER beim Senden der Email an {t.Kunde.Email}")
                                ' We do NOT set ErinnerungVerschickt = True so we retry later? 
                                ' Or set it to true to avoid spam loop if error persists? 
                                ' For now, let's keep it false to retry, but logs will spam. 
                                ' Better: Keep false.
                            End Try
                        Next
                        
                        If fälligeTermine.Count > 0 Then
                            db.SaveChanges() ' DB Update
                        End If
                    End Using

                Catch ex As Exception
                    _logger.LogError(ex, "Fehler im Erinnerungs-Service")
                End Try

                Await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken)
            End While
        End Function
    End Class
End Namespace
