Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting
Imports Microsoft.Extensions.Configuration
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Authentication.JwtBearer
Imports Microsoft.IdentityModel.Tokens
Imports Sauberfix.Data
Imports Sauberfix.Services
Imports System
Imports System.Text
Imports System.Security.Claims

Module Program
    Sub Main(args As String())
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", True)
        Dim builder = WebApplication.CreateBuilder(args)
        
        Dim connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        builder.Services.AddDbContext(Of AppDbContext)(Sub(options)
            options.UseNpgsql(connectionString, Sub(b) b.MigrationsAssembly("Sauberfix.Data"))
        End Sub)

        Dim jwtKey = builder.Configuration("JwtSettings:Key")
        Dim keyBytes = Encoding.UTF8.GetBytes(jwtKey)

        builder.Services.AddAuthentication(Function(x)
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme
        End Function).AddJwtBearer(Function(x)
            x.RequireHttpsMetadata = True
            x.SaveToken = True
            x.TokenValidationParameters = New TokenValidationParameters With {
                .ValidateIssuerSigningKey = True,
                .IssuerSigningKey = New SymmetricSecurityKey(keyBytes),
                .ValidateIssuer = True,
                .ValidIssuer = builder.Configuration("JwtSettings:Issuer"),
                .ValidateAudience = True,
                .ValidAudience = builder.Configuration("JwtSettings:Audience")
            }
        End Function)
        
        builder.Services.AddAuthorization()

        ' CORS-Konfiguration für Sicherheit
        builder.Services.AddCors(Sub(options)
            options.AddPolicy("AllowFrontend", Function(policy)
                Return policy.WithOrigins("http://localhost:5000", "https://localhost:5001") _
                      .AllowAnyMethod() _
                      .AllowAnyHeader() _
                      .AllowCredentials()
            End Function)
        End Sub)

        builder.Services.AddScoped(Of KundenService)()
        builder.Services.AddScoped(Of MitarbeiterService)()
        builder.Services.AddScoped(Of TerminService)()
        builder.Services.AddScoped(Of AuthService)()
        builder.Services.AddHostedService(Of ErinnerungsService)()

        Dim app = builder.Build()
        app.UseDefaultFiles()
        app.UseStaticFiles()
        app.UseCors("AllowFrontend")
        app.UseAuthentication()
        app.UseAuthorization()

        app.MapPost("/login", Function(auth As AuthService, input As LoginDto)
            Dim result = auth.Login(input)
            If result Is Nothing Then Return Results.Unauthorized()
            Return Results.Ok(result)
        End Function)

        ' MITARBEITER
        app.MapPost("/mitarbeiter", Function(s As MitarbeiterService, i As CreateMitarbeiterDto)
             Try
                 Return Results.Created("/m", s.CreateMitarbeiter(i))
             Catch ex As ArgumentException
                 Return Results.Problem(ex.Message)
             Catch ex As Exception
                 Return Results.Problem("Ein Fehler ist beim Erstellen des Mitarbeiters aufgetreten.")
             End Try
        End Function).RequireAuthorization()
        
        app.MapPut("/mitarbeiter/{id}", Function(s As MitarbeiterService, id As Integer, i As CreateMitarbeiterDto)
             Try
                 Return Results.Ok(s.UpdateMitarbeiter(id, i))
             Catch ex As ArgumentException
                 Return Results.Problem(ex.Message)
             Catch ex As Exception
                 Return Results.Problem("Ein Fehler ist beim Aktualisieren des Mitarbeiters aufgetreten.")
             End Try
        End Function).RequireAuthorization()

        app.MapGet("/mitarbeiter", Function(s As MitarbeiterService) s.GetAllMitarbeiter()).RequireAuthorization()
        
        app.MapDelete("/mitarbeiter/{id}", Function(s As MitarbeiterService, id As Integer)
            s.DeleteMitarbeiter(id)
            Return Results.Ok("Gelöscht")
        End Function).RequireAuthorization()

        ' TERMINE
        app.MapGet("/termine", Function(s As TerminService, user As ClaimsPrincipal)
            Dim id = Integer.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value)
            Dim role = user.FindFirst(ClaimTypes.Role).Value
            Return s.GetAllTermine(id, role)
        End Function).RequireAuthorization()
        
        app.MapPost("/termine", Function(s As TerminService, i As CreateTerminDto)
            Try
                Return Results.Created("/t", s.CreateTermin(i))
            Catch ex As ArgumentException
                Return Results.Problem(ex.Message)
            Catch ex As Exception
                Return Results.Problem("Ein Fehler ist beim Erstellen des Termins aufgetreten.")
            End Try
        End Function).RequireAuthorization()

        app.MapPut("/termine/{id}", Function(s As TerminService, id As Integer, i As CreateTerminDto)
            Try
                Return Results.Ok(s.UpdateTermin(id, i))
            Catch ex As ArgumentException
                Return Results.Problem(ex.Message)
            Catch ex As Exception
                Return Results.Problem("Ein Fehler ist beim Aktualisieren des Termins aufgetreten.")
            End Try
        End Function).RequireAuthorization()

        app.MapDelete("/termine/{id}", Function(s As TerminService, id As Integer)
            s.DeleteTermin(id)
            Return Results.Ok("Gelöscht")
        End Function).RequireAuthorization()

        ' KUNDEN
        app.MapPost("/kunden", Function(s As KundenService, i As CreateKundeDto) Results.Created("/k", s.CreateKunde(i))).RequireAuthorization()
        app.MapGet("/kunden", Function(s As KundenService) s.GetAllKunden()).RequireAuthorization()
        
        ' NEU: Update Kunde
        app.MapPut("/kunden/{id}", Function(s As KundenService, id As Integer, i As CreateKundeDto)
             Try
                 Return Results.Ok(s.UpdateKunde(id, i))
             Catch ex As ArgumentException
                 Return Results.Problem(ex.Message)
             Catch ex As Exception
                 Return Results.Problem("Ein Fehler ist beim Aktualisieren des Kunden aufgetreten.")
             End Try
        End Function).RequireAuthorization()

        ' NEU: Delete Kunde
        app.MapDelete("/kunden/{id}", Function(s As KundenService, id As Integer)
            s.DeleteKunde(id)
            Return Results.Ok("Kunde gelöscht")
        End Function).RequireAuthorization()


        Using scope = app.Services.CreateScope()
            Dim db = scope.ServiceProvider.GetRequiredService(Of AppDbContext)()
            db.Database.Migrate()
            DatabaseSeeder.SeedDatabase(db)
        End Using

        app.Run()
    End Sub
End Module
