# Podział pracy

Każdy ma swój obszar i swój folder w Assets/Schron/Systems. Wspólne rzeczy (enumy, kontrakty, eventy) są w Core i zmieniamy je tylko po ustaleniu z resztą.

## Ekonomia i budowanie: Karolina

Folder: Systems/Buildings&Economy

Siatka schronu, budowanie i ulepszanie pomieszczeń, produkcja i zużycie zasobów, pieniądze, skutki katastrofy dla bazy, balans liczb.

## Mieszkańcy: Bartek

Folder: Systems/ResidentsAI

Dane i statystyki mieszkańców, ich stany, przydzielanie do pomieszczeń, poruszanie się i animacje, ekspedycje.

## UI i fabuła: Norbert

Folder: Systems/UI&Events

Interfejs gry, wydarzenia z wyborami, inne schrony i relacje z nimi, przebieg katastrofy, zakończenia.

## Jak pracujemy
- Każdy na swoim branchu, zmiany łączymy przez pull requesty, a PR zatwierdza ktoś inny.
- Testujemy na swoich scenach w TEST/DEV_X_TEST. Główną scenę edytuje jedna osoba naraz, po wcześniejszym uprzedzeniu na czacie.
- Nowe wartości w enumach dopisujemy tylko na końcu listy.
- Zadania i ich status trzymamy na boardzie.
