# Projektverlauf Sauberfix

## 1. Initiale Entwicklungsphase (Dezember 2025)

Der Startschuss für das Projekt fiel Anfang Dezember. Der Fokus lag auf der Schaffung eines funktionsfähigen MVPs (Minimum Viable Product).

*   **09.12.2025**:
    *   Initialer Commit des Backends (ASP.NET Core) und Frontends.
    *   Implementierung der Kernlogik für Terminkollisionen.
    *   Strukturierung des Repositories (Trennung von Data/Services).
*   **11.12.2025**:
    *   Integration des "FullCalendar Scheduler" für die visuelle Dispo-Tafel.
    *   Erweiterung des Datenmodells um "Endzeit" für Termine.
    *   Implementierung von Datenbank-Migrationen und Seeding.
    *   Erweiterung der Kundenverwaltung um Firmennamen.

## 2. Sicherheits-Audit & Härtung (Mitte Dezember 2025)

Nach der Fertigstellung der Basisfunktionen wurde ein umfassendes Sicherheitsaudit durchgeführt.

*   **12.12.2025**:
    *   **Security Overhaul**: Implementierung von XSS-Schutz, Umstellung auf BCrypt für Passwörter, Erzwingung von HTTPS.
    *   Absicherung aller API-Endpunkte mittels JWT-Auth.
    *   Korrektur von Informationslecks in Fehlermeldungen.

## 3. DevOps & Containerisierung (Januar 2026)

Zu Beginn des neuen Jahres wurde die Anwendung für den Produktionsbetrieb in Container-Umgebungen vorbereitet.

*   **07.01.2026**:
    *   Erstellung von `Dockerfile` und Kubernetes Manifesten (`k8s/`).
    *   Einrichtung der CI/CD Pipeline via GitHub Actions.
    *   Umfassende Dokumentation (`README.md`, `SETUP.md`).
*   **08.01.2026**:
    *   Optimierung der CI/CD Pipeline (Short SHAs, Trigger-Korrekturen).
    *   Update der Landing Page.
    *   **Feature**: Dark Mode für Frontend implementiert.

## 4. Produktionsreife & E-Mail-Integration (Ende Januar 2026)

Die finale Phase konzentrierte sich auf betriebsrelevante Features und Stabilität.

*   **28.01.2026**:
    *   **Feature**: E-Mail-Erinnerungssystem (24h vorher).
    *   **Fix**: Zeitzonen-Korrektur (Server UTC vs. Lokale Zeit).
    *   **Infra**: SMTP-Konfiguration über Kubernetes Secrets.
    *   **Fix**: Anpassung auf SMTP Port 587 (STARTTLS) und Zertifikats-Handling.
    *   **Docs**: Generierung einer professionellen PDF-Gesamtdokumentation.

## Zusammenfassung

Das Projekt hat sich von einem einfachen MVP zu einer produktionsreifen, containerisierten Lösung entwickelt. Kritische Meilensteine waren die Implementierung der visuellen Plantafel, das Sicherheits-Audit und die erfolgreiche Integration in eine Kubernetes-Umgebung mit automatisierter CI/CD-Pipeline.
