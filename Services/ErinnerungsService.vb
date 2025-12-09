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

                        ' Logic: Termine morgen, die noch KEINE Mail bekommen haben
                        Dim morgen = DateTime.UtcNow.AddDays(1)
                        Dim startZeit = morgen.AddMinutes(-2) 
                        Dim endZeit = morgen.AddMinutes(2)

                        Dim fälligeTermine = db.Termine _
                                               .Include(Function(t) t.Kunde) _
                                               .Where(Function(t) t.DatumUhrzeit >= startZeit AndAlso t.DatumUhrzeit <= endZeit AndAlso t.Status = TerminStatus.Geplant AndAlso t.ErinnerungVerschickt = False) _
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
