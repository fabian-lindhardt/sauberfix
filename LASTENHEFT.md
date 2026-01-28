# Lastenheft: Einführung einer digitalen Terminverwaltung "Sauberfix"

## 1. Ausgangssituation (Ist-Zustand)
Das Reinigungsunternehmen "Sauberfix" verwaltet seine Kunden, Mitarbeiter und Reinigungstermine aktuell über Excel-Tabellen und handschriftliche Kalender. 
Dies führt zu folgenden Problemen:
*   Häufige Terminkollisionen (Doppelbuchungen von Mitarbeitern).
*   Fehlende Übersicht über die Verfügbarkeit von Mitarbeitern.
*   Keine automatische Erinnerung an Kunden, was zu vergessenen Terminen führt.
*   Hoher manueller Aufwand bei Terminverschiebungen.
*   Zugriff auf aktuelle Pläne ist nur im Büro möglich (kein Fernzugriff).

## 2. Zielsetzung (Soll-Zustand)
Es soll eine webbasierte Anwendung ("Sauberfix") eingeführt werden, die den gesamten Planungsprozess digitalisiert und zentralisiert.
Die Hauptziele sind:
*   **Vermeidung von Fehlern**: Das System soll Doppelbuchungen technisch unmöglich machen.
*   **Effizienzsteigerung**: Schnelle Übersicht über freie Kapazitäten durch eine grafische Plantafel.
*   **Kundenbindung**: Reduktion von Terminausfällen durch automatische E-Mail-Erinnerungen.
*   **Flexibilität**: Ortsunabhängiger Zugriff für Disponenten und Mitarbeiter.

## 3. Zielgruppen
*   **Disponenten (Admins)**: Planen Termine, verwalten Stammdaten (Kunden/Mitarbeiter).
*   **Reinigungskräfte (User)**: Sehen ihre eigenen Einsätze ein.

## 4. Funktionale Anforderungen

### 4.1 Terminplanung
*   Erfassung von Terminen mit Datum, Uhrzeit (Start/Ende), Kunde und Mitarbeiter.
*   Visuelle Darstellung der Termine in einem Kalender (Wochenansicht).
*   Einfaches Verschieben von Terminen (Drag & Drop).
*   **Muss**: Automatische Warnung/Blockierung bei Überschneidungen von Terminen eines Mitarbeiters.

### 4.2 Stammdatenverwaltung
*   Zentrale Datenbank für Kunden (Anschrift, Kontaktdaten).
*   Verwaltung von Mitarbeiterprofilen inkl. Zugangsdaten.

### 4.3 Benachrichtigungen
*   Das System soll automatisch Erinnerungs-E-Mails an Kunden versenden (z.B. 24h vor Termin).

### 4.4 Zugriffsschutz
*   Unterschiedliche Berechtigungen für Admins (Vollzugriff) und Mitarbeiter (Lesezugriff auf eigene Termine).
*   Sichere Anmeldung mittels Passwort.

## 5. Nicht-funktionale Anforderungen

### 5.1 Systemumgebung
*   Webbasierte Lösung, lauffähig in modernen Browsern (Chrome, Firefox, Edge).
*   Hosting auf firmeneigenen Servern oder Cloud-Container-Umgebung (Docker).

### 5.2 Datensicherheit
*   Verschlüsselter Zugriff (HTTPS).
*   Sichere Speicherung von Passwörtern (kein Klartext).
*   Regelmäßige Datensicherung (Backup).

### 5.3 Benutzerfreundlichkeit
*   Intuitive Bedienung, insbesondere der Plantafel.
*   Modernes Design ("Look & Feel"), ggf. mit Dark Mode für ergonomisches Arbeiten.

## 6. Lieferumfang
*   Lauffähige Software-Anwendung (Container-Images).
*   Installations- und Betriebsanleitung.
*   Benutzerhandbuch (oder integrierte Hilfe).
