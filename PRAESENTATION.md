% Sauberfix
% Fabian Lindhardt
% 28. Januar 2026

# Projektübersicht

## Einführung

**Sauberfix** ist eine moderne, webbasierte Lösung zur Terminverwaltung für Reinigungsunternehmen.

*   **Ziel**: Digitalisierung der Einsatzplanung
*   **Kernfunktion**: Grafische Plantafel mit Kollisionsprüfung
*   **Technologie**: .NET 9.0, Docker, Kubernetes

# Ausgangslage & Motivation

## Das Problem (Ist-Zustand)

Reinigungskräfte und Termine wurden manuell koordiniert.

*   ❌ Terminkollisionen (Doppelbuchungen)
*   ❌ Keine zentrale Übersicht
*   ❌ Kunden wurden vergessen (fehlende Erinnerungen)
*   ❌ Hoher manueller Aufwand

## Die Lösung (Soll-Zustand)

Einführung einer zentralen Plattform.

*   ✅ **Automatische Kollisionsprüfung**
*   ✅ **Visuelle Plantafel** (Drag & Drop)
*   ✅ **E-Mail-Erinnerungen** (automatisiert 24h vorher)
*   ✅ **Ortsunabhängiger Zugriff**

# Projektverlauf

## Phasen

1.  **Initiale Entwicklung (Dez 2025)**
    *   Basis-Backend & Frontend
    *   Implementierung der Dispo-Tafel (FullCalendar)

2.  **Sicherheits-Audit (Mitte Dez 2025)**
    *   Härtung (HTTPS, BCrypt, XSS-Schutz)
    *   Rollenkonzept (Admin/User)

## Phasen (Fortsetzung)

3.  **DevOps & Containerisierung (Jan 2026)**
    *   Docker & Kubernetes Support
    *   CI/CD Pipeline (GitHub Actions)

4.  **Produktionsreife (Ende Jan 2026)**
    *   E-Mail-Erinnerungssystem (SMTP)
    *   Zeitzonen-Korrektur
    *   Dokumentation

# Technische Details

## Architektur

*   **Backend**: ASP.NET Core 9.0 (Minimal APIs)
*   **Datenbank**: PostgreSQL (via Entity Framework Core)
*   **Frontend**: Native Web-Tech (HTML5, JS, CSS), FullCalendar
*   **Auth**: JWT Bearer Tokens

## Sicherheit

*   **Passwörter**: BCrypt (WorkFactor 12)
*   **Transport**: HTTPS Erzwungen
*   **API**: Authentifiziert & Autorisiert
*   **Deployment**: Secrets Management in Kubernetes

# Zusammenfassung

## Fazit

**Sauberfix** transformiert die Planung von manuell & fehleranfällig zu digital & effizient.

*   Stabile, skalierbare Architektur
*   Hoher Sicherheitsstandard
*   Automatisierte Prozesse (CI/CD, E-Mails)
*   Bereit für den Produktionseinsatz 🚀

## Vielen Dank

**Fragen?**
