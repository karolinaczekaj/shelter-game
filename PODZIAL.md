# Podział pracy i ustalenia

Każdy ma swój obszar i swój folder w `Assets/Schron/Systems`. Wspólne rzeczy (enumy, kontrakty, eventy) są w `Core` i zmieniamy je tylko po ustaleniu z resztą.

## Kto co robi

### Karolina: ekonomia i budowanie (DEV_1)
Folder: `Systems/Buildings&Economy`, scena testowa: `TEST/DEV_1_TEST`

Siatka schronu, budowanie i ulepszanie pomieszczeń, klasa `Room`, produkcja i zużycie zasobów, `ResourceManager` i `DayManager`, skutki katastrofy dla bazy, balans liczb.

### Bartek: mieszkańcy (DEV_2)
Folder: `Systems/ResidentsAI`, scena testowa: `TEST/DEV_2_TEST`

Dane i statystyki mieszkańców, ich stany, przydzielanie do pomieszczeń, poruszanie się i animacje, ekspedycje.

### Norbert: UI i fabuła (DEV_3)
Folder: `Systems/UI&Events`, scena testowa: `TEST/DEV_3_TEST`

Interfejs gry, wydarzenia z wyborami, inne schrony i relacje z nimi, przebieg katastrofy, zakończenia.

## Przebieg gry

- **Faza przygotowań:** dni 1–60. Budujemy, ulepszamy, zbieramy zapasy, dogadujemy się (albo nie) z innymi schronami.
- **Faza katastrofy:** dni 61–65. Katastrofa uderza w schron, budowanie jest ograniczone, liczą się zapasy i decyzje.
- **Wygrana:** schron przetrwał 5 dni katastrofy i żyje w nim co najmniej jeden mieszkaniec. Im więcej ludzi i lepszy stan schronu, tym lepsze zakończenie.
- **Przegrana:** wszyscy mieszkańcy zginęli albo morale schronu spadło do zera (bunt). Może się to stać w każdej fazie.

Liczby (60 dni, 5 dni) ustawia się w `DayManager`, więc można je łatwo zmienić przy testach.

## Przebieg jednego dnia

Gracz robi w danym dniu, co chce, i klika „Następny dzień”. Wtedy po kolei:

1. `ShelterActions.DayEnded(dzień)`, czyli rozliczenie mijającego dnia:
   - pokoje z prądem produkują zasoby (warsztat najpierw zużywa złom i rudę),
   - pokoje zużywają energię,
   - mieszkańcy jedzą i piją, a przy braku jedzenia lub wody tracą zdrowie,
   - przeliczane jest morale schronu.
2. Licznik dni rośnie o jeden.
3. Jeśli zaczyna się dzień 61: `GameEvents.DisasterStarted()`.
4. `ShelterActions.DayStarted(dzień)`, czyli nowy dzień: może pojawić się wydarzenie, a UI się odświeża.
5. Sprawdzenie wygranej i przegranej, ewentualnie `GameEvents.GameOver(true/false)`.

## Zasoby

Jedzenie, woda, energia, złom, ruda, komponenty i morale. Pieniędzy w grze nie ma.

- Kopalnia daje rudę, a czasem trochę złomu.
- Złom w większych ilościach przynoszą wyprawy (jak już będą).
- Warsztat przerabia złom i rudę na komponenty.
- Budowa i ulepszenia kosztują komponenty, tańsze rzeczy sam złom.
- Z innymi schronami handlujemy wymianą: złom, komponenty, jedzenie, woda.

**Morale schronu** to średnia morale wszystkich mieszkańców. Liczy się samo i pokazuje na pasku zasobów, nikt go ręcznie nie dodaje ani nie odejmuje. Zmieniamy morale konkretnych mieszkańców.

**Brak energii:** pokoje bez prądu przestają produkować.

## Start gry

Gracz zaczyna z:
- postawionym generatorem i kwaterami,
- kilkoma mieszkańcami,
- zapasem jedzenia i wody na kilka dni,
- 30 złomu i 20 komponentów, żeby było z czego zacząć budować.

Liczby są do przetestowania i zmiany przy balansowaniu.

## Pomieszczenia

| Pokój | Co robi | Statystyka |
|---|---|---|
| Kwatery | mieszkańcy śpią, zwiększa limit ludzi, odpoczynek poprawia morale | – |
| Farma | jedzenie | zręczność |
| Uzdatnianie wody | woda | inteligencja |
| Generator | energia | siła |
| Kopalnia | ruda, czasem złom | siła |
| Warsztat | złom + ruda → komponenty | inteligencja |
| Lecznica | leczy rannych i chorych | inteligencja |

Najpierw robimy pierwsze sześć, bo bez nich gra się nie kręci. Lecznica jest następna. Jeśli zostanie czas: magazyn, ochrona i radiostacja (kontakt i handel z innymi schronami).

## Mieszkańcy

Statystyki: siła, inteligencja, zręczność (1–10). Każdy pokój ma swoją statystykę, która zwiększa jego produkcję.

**Przydział do pokoju zmieniamy tylko przez `room.TryAssign()` i `room.Remove()`.** Te metody aktualizują jednocześnie listę pracowników w pokoju i pozycję pokoju zapisaną w mieszkańcu, żeby nic się nie rozjechało. Mieszkaniec może być naraz tylko w jednym pokoju.

`Stan ustawia sam pokój:` TryAssign w kwaterach daje Rest, w lecznicy Recovery, a w pozostałych pokojach Working. Remove zawsze ustawia Idle. Do kwater i lecznicy przypisujemy tak samo przez TryAssign.

Lista room.Workers w kwaterach i lecznicy oznacza osoby w środku, a nie pracowników. Nazwa została, żeby nie psuć kodu.

## Siatka i grafika

- Pixels Per Unit: 32
- 1 kratka = 3 × 2 jednostki (96 × 64 px)
- postać mniej więcej 1 × 1.5 jednostki (32 × 48 px)
- pokoje mają 1–3 kratki szerokości i zawsze 1 piętro wysokości
- siatka na start: 8 kratek szerokości, 6 pięter w dół
- winda to stała kolumna łącząca piętra; w jej kolumnie nie da się budować, a pokój nie może jej przecinać

## Wspólny kod w `Core/Contracts/Scripts`

Wszystko jest w namespace `Schron.Core`, więc w swoich skryptach dopisujecie na górze `using Schron.Core;`.

| Plik | Co to jest |
|---|---|
| `Enum.cs` | listy wartości: zasoby (`ResourceType`), typy pokoi (`RoomType`), statystyki (`StatType`), stany mieszkańca (`ResidentState`) |
| `RoomDataSO.cs` | dane typu pokoju (koszt, produkcja, liczba miejsc, wymagana statystyka). Pliki tworzymy w Unity przez Create → Schron → Room Data i trzymamy w `Data/Rooms` |
| `Room.cs` | konkretny pokój postawiony w schronie. Metody: `TryAssign(resident)`, `Remove(resident)`, `TryUpgrade()` |
| `ResidentData.cs` | dane mieszkańca: imię, zdrowie, morale, statystyki, stan, pozycja przydzielonego pokoju. Metoda `GetStat(StatType)` |
| `ShelterActions.cs` | eventy symulacji: dni, zasoby, budowa i ulepszanie pokoi, przydział i stan mieszkańców |
| `GameEvents.cs` | eventy fabularne: wyprawy, katastrofa, koniec gry, komunikaty dla gracza (`Notify`) |

**Jak używać eventów:** kto coś zmienia, ten ogłasza, np. po postawieniu pokoju `ShelterActions.RoomBuilt(data, pozycja)`. Kto chce zareagować, podpina się w `OnEnable` i odpina w `OnDisable`:

```csharp
void OnEnable()  { ShelterActions.OnRoomBuilt += HandleRoomBuilt; }
void OnDisable() { ShelterActions.OnRoomBuilt -= HandleRoomBuilt; }
```

Dzięki temu nasze części nie muszą o sobie wiedzieć, np. UI słucha zmian zasobów i samo odświeża pasek.

**Komunikaty dla gracza** (brak zasobów, pełny pokój itp.) wysyłamy przez `GameEvents.Notify("tekst")`, a UI pokazuje je w jednym miejscu.

## Menedżery w `Core/Managers`

Oba siedzą na obiekcie `GameManager` w scenie i są dostępne z każdego skryptu przez `.Instance`.

**ResourceManager**: stan zasobów schronu. Zasoby startowe ustawia się w Inspectorze.

- `Get(typ)`: ile mamy danego zasobu
- `Add(typ, ilość)`: dodaje, albo odejmuje przy ujemnej wartości; nie schodzi poniżej 0
- `TrySpend(typ, koszt)`: płaci, jeśli stać, i zwraca true albo false
- `CanAfford(listaKosztów)`: czy stać nas na cały koszt, np. przy budowie
- `TrySpend(listaKosztów)`: płaci cały koszt albo nic
- `UpdateShelterMorale(listaMieszkańców)`: liczy morale schronu jako średnią, raz dziennie

Przykład: `ResourceManager.Instance.Add(ResourceType.Food, -2);`

**DayManager**: system tur. Liczbę dni przygotowań i katastrofy ustawia się w Inspectorze.

- `NextDay()`: przechodzi do następnego dnia, pod przycisk „Następny dzień”
- `EndGame(czyWygrana)`: kończy grę, np. `EndGame(false)`, gdy zginą wszyscy mieszkańcy
- `CurrentDay`: numer dnia
- `IsDisasterPhase`: czy trwa już katastrofa

Do testów bez UI: w Play kliknij prawym na komponencie `DayManager` w Inspectorze -> **Next Day**.

## Siatka i budowanie (Systems/Buildings&Economy)

Obiekt `Shelter` w scenie ma `ShelterGrid` i `BuildManager`.

**ShelterGrid**: siatka schronu, pole (0,0) to lewy górny róg, y rośnie w dół.
- `CanPlace(pole, rozmiar)`: czy pokój się zmieści
- `GetRoom(pole)`: jaki pokój stoi na danym polu
- `WorldToCell(pozycja)`: zamienia pozycję w świecie (np. kliknięcie) na pole siatki
- `AreaCenter(pole, rozmiar)`: środek obszaru w świecie, np. żeby postać wiedziała, gdzie iść

**BuildManager**:
- `SelectRoom(danePokoju)`: wybiera pokój do budowy, pod przyciski w menu budowania; `SelectRoom(null)` anuluje
- `TryBuild(danePokoju, pole)`: buduje, jeśli jest miejsce i zasoby
- pokoje startowe ustawia się w Inspectorze w `Starting Rooms`
- po wybraniu pokoju pod kursorem widać jego zarys: zielony, gdy można budować, czerwony, gdy nie ma miejsca albo zasobów; kliknięcia nad UI nie budują

Na razie do testów pokój wybiera się klawiszami 1–6, a Esc albo prawy przycisk myszy anuluje.


**ProductionManager** (na obiekcie `Shelter`): rozlicza produkcję na koniec każdego dnia.
- najpierw produkują generatory, potem każdy pokój płaci za prąd; bez prądu nie produkuje
- warsztat zużywa złom i rudę (`inputPerDay`), bez materiałów nie robi komponentów
- produkcja pracownika = `baseProductionPerWorker × (0.5 + statystyka / 10)`, czyli stat 1 daje ×0.6, stat 5 ×1.0, stat 10 ×1.5
- każdy poziom pokoju powyżej pierwszego daje +50% produkcji
- kopalnia ma 30% szans na 1 złom od każdego pracownika dziennie
- po każdym dniu w Console jest podsumowanie zasobów


**ShelterCapacity** (`Schron.Building`, bez obiektu w scenie): ile osób może mieszkać w schronie.
- `ShelterCapacity.GetTotalBeds()`: łączna liczba miejsc we wszystkich kwaterach
- `ShelterCapacity.HasFreeBed(liczbaMieszkańców)`: czy zmieści się jeszcze ktoś, do sprawdzenia przed przyjęciem nowej osoby
- kwatery mają 4 miejsca, każdy kolejny poziom dodaje 2 (poziom 2 to 6, poziom 3 to 8)

W swoim skrypcie trzeba dopisać `using Schron.Building;`.

## Jak pracujemy

- Kod i nazwy w kodzie po angielsku, commity mogą być po polsku.
- Każdy na swoim branchu, zmiany łączymy przez pull requesty, a PR zatwierdza ktoś inny.
- Branche nazywamy od zadania, np. `siatka-schronu`, `pasek-zasobow`.
- Przed pracą zawsze `git checkout main` i `git pull`.
- W opisie pull requesta piszemy `Closes #numer` zadania, wtedy karta na boardzie sama przechodzi do Done.
- Testujemy na swoich scenach. Główną scenę `Main` edytuje jedna osoba naraz, po wcześniejszym uprzedzeniu na czacie.
- Pliki w `Core` zmieniamy tylko po ustaleniu we trójkę. Nowe wartości w enumach dopisujemy tylko na końcu listy.
- Zadania i ich status trzymamy na boardzie. Zaczynając zadanie, przesuwamy kartę do In Progress.
- Grafiki i dźwięki z darmowych paczek zapisujemy w osobnym pliku `CREDITS.md`: skąd są i na jakiej licencji.

## Wersja Unity

Wszyscy używamy **Unity 6.3 LTS (6000.3.28f1)**. Bez zmiany wersji bez ustalenia z resztą.
