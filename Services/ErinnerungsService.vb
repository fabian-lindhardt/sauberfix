Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Logging
Imports Microsoft.Extensions.DependencyInjection
Imports Sauberfix.Data
Imports Microsoft.EntityFrameworkCore
Imports System
Imports System.Threading
Imports System.Threading.Tasks

Namespace Services
    Public Class ErinnerungsService
        Inherits BackgroundService

        Private ReadOnly _serviceScopeFactory As IServiceScopeFactory
        Private ReadOnly _logger As ILogger(Of ErinnerungsService)

        Public Sub New(serviceScopeFactory As IServiceScopeFactory, logger As ILogger(Of ErinnerungsService))
            _serviceScopeFactory = serviceScopeFactory
            _logger = logger
        End Sub

        Protected Overrides Async Function ExecuteAsync(stoppingToken As CancellationToken) As Task
            _logger.LogInformation(">>> Erinnerungs-Service gestartet.")

            While Not stoppingToken.IsCancellationRequested
                Try
                    Using scope = _serviceScopeFactory.CreateScope()
                        Dim db = scope.ServiceProvider.GetRequiredService(Of AppDbContext)()

                        ' Logic: Termin.DatumUhrzeit - Termin.Vorlauf <= Now (e.g. 10:00 - 24h = Yesterday 10:00. If Now >= Yesterday 10:00, Send).
                        ' But we only want to send if we haven't sent yet, and if the appointment is in the future (optional, but good practice).
                        ' Implementation: t.DatumUhrzeit.AddMinutes(-t.ErinnerungVorlaufMinuten) <= DateTime.UtcNow
                        
                        Dim now = DateTime.UtcNow
                        
                        ' Wir nutzen EF.Functions oder client-side evaluation, falls Provider AddMinutes mit Spalte nicht unterstützt.
                        ' Postgres Npgsql unterstützt Arithmetik meist gut.
                        
                        Dim fälligeTermine = db.Termine _
                                               .Include(Function(t) t.Kunde) _
                                               .Where(Function(t) t.Status = TerminStatus.Geplant _
                                                              AndAlso t.ErinnerungVerschickt = False _
                                                              AndAlso t.DatumUhrzeit.AddMinutes(-t.ErinnerungVorlaufMinuten) <= now _
                                                              AndAlso t.DatumUhrzeit > now) _
                                               .ToList()

                        For Each t In fälligeTermine
                            _logger.LogWarning($"[EMAIL GESENDET] An: {t.Kunde.Email} | Termin: {t.DatumUhrzeit}")
                            
                            ' Status speichern!
                            t.ErinnerungVerschickt = True
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
