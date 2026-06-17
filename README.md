# System obsługi wniosków kredytowych

Aplikacja webowa wspierająca obsługę wniosków kredytowych w banku. System umożliwia tworzenie wniosków, automatyczne obliczenie podstawowej zdolności kredytowej, przekazanie wniosku do analizy przełożonego oraz zapis historii decyzji.

## Technologie

Projekt został wykonany w technologii ASP.NET Core MVC.

Wykorzystane technologie:

* C#
* ASP.NET Core MVC
* ASP.NET Core Identity
* Entity Framework Core
* SQL Server LocalDB
* Razor Views
* Bootstrap
* .NET 8

## Wymagania

Do uruchomienia projektu lokalnie wymagane są:

* Windows 10 lub Windows 11
* Visual Studio 2022
* .NET 8 SDK
* SQL Server Express LocalDB

Projekt został przygotowany dla .NET 8. Jeżeli na komputerze jest zainstalowany tylko .NET 9, należy dodatkowo zainstalować .NET 8 SDK. Nie trzeba usuwać .NET 9 — obie wersje mogą być zainstalowane jednocześnie.

## Uruchomienie projektu

1. Pobierz projekt z repozytorium GitHub.

2. Rozpakuj projekt, jeżeli został pobrany jako plik ZIP.

3. Otwórz plik:

   `LoanApplicationSystem/LoanApplicationSystem.sln`

   w Visual Studio 2022.

4. Ustaw projekt `LoanApplicationSystem` jako projekt startowy.

5. Uruchom aplikację przyciskiem Start w Visual Studio.

Przy pierwszym uruchomieniu aplikacja automatycznie utworzy bazę danych na podstawie migracji Entity Framework Core.

## Baza danych

Aplikacja korzysta z SQL Server LocalDB.

Connection string znajduje się w pliku:

`LoanApplicationSystem/LoanApplicationSystem/appsettings.json`

Domyślna baza danych:

`LoanApplicationSystemDb`

Baza danych jest tworzona automatycznie przy starcie aplikacji dzięki migracjom Entity Framework Core.

## Konta testowe

Rejestracja użytkowników została wyłączona, ponieważ system jest przeznaczony do użytku wewnętrznego. Konta użytkowników są przygotowywane podczas startu aplikacji.

### Administrator / przełożony

Email:

`admin@app.pl`

Hasło:

`Admin123!`

Administrator ma dostęp do panelu przełożonego, listy wszystkich wniosków, zmiany statusów oraz edycji ogłoszenia systemowego.

### Pracownik

Email:

`pracownik@app.pl`

Hasło:

`Pracownik123!`

Pracownik może tworzyć nowe wnioski kredytowe oraz przeglądać swoje wnioski.

## Główne funkcje systemu

System umożliwia:

* logowanie użytkowników,
* tworzenie nowych wniosków kredytowych,
* wybór kraju i automatyczne formatowanie numeru telefonu,
* przeglądanie własnych wniosków przez pracownika,
* przeglądanie wszystkich wniosków przez administratora,
* filtrowanie i wyszukiwanie wniosków,
* automatyczne wyliczenie podstawowych wskaźników kredytowych,
* automatyczne odrzucenie wniosków niespełniających minimalnych kryteriów,
* przekazanie wniosków pozytywnych i warunkowych do analizy przełożonego,
* zmianę statusu wniosku przez administratora,
* zapisywanie historii decyzji,
* oznaczanie wniosków jako zawierające błędne dane,
* wyświetlanie ogłoszenia systemowego,
* edycję treści ogłoszenia przez administratora.

## Statusy wniosków

Wniosek może posiadać jeden z następujących statusów:

* Złożony
* W trakcie analizy
* Zaakceptowany
* Odrzucony
* Błędne dane

Każdy nowy wniosek otrzymuje początkowo status `Złożony`, chyba że system automatycznie odrzuci go z powodu niespełnienia minimalnych kryteriów zdolności kredytowej.

## Logika obsługi wniosków

Pracownik tworzy wniosek na podstawie danych klienta. Przed zapisaniem formularza system wyświetla komunikat potwierdzający poprawność wprowadzonych danych.

Po zapisaniu wniosku jego dane nie są edytowane przez pracownika. Jeżeli dane zostały wpisane błędnie, administrator może oznaczyć wniosek statusem `Błędne dane`, a pracownik tworzy nowy wniosek z poprawnymi informacjami.

Administrator może awaryjnie dodać wniosek, jednak standardowo wnioski tworzy pracownik.

## Ocena zdolności kredytowej

System oblicza podstawowe dane pomocnicze dla decyzji kredytowej, między innymi:

* dochód rozporządzalny,
* szacowaną miesięczną ratę,
* wskaźnik DTI,
* wynik scoringowy,
* rekomendację systemową.

System może nadać jedną z rekomendacji:

* Zdolność kredytowa pozytywna
* Zdolność kredytowa warunkowa
* Brak zdolności kredytowej

Jeżeli wniosek nie spełnia minimalnych kryteriów, system automatycznie nadaje mu status `Odrzucony`.

Wnioski pozytywne i warunkowe trafiają do panelu przełożonego, gdzie administrator podejmuje decyzję końcową.

## Zasady decyzji administratora

Administrator może zmieniać status wniosku i dodać komentarz do decyzji. Każda zmiana statusu jest zapisywana w historii decyzji.

Dla wniosków automatycznie odrzuconych przez system administrator nie może nadać statusu `Zaakceptowany`. Może pozostawić status `Odrzucony` albo oznaczyć wniosek jako `Błędne dane`.

Status `Błędne dane` oznacza, że wniosek zawiera niepoprawnie wpisane informacje i nie powinien być dalej rozpatrywany. W takim przypadku pracownik tworzy nowy wniosek z poprawnymi danymi.

## Role użytkowników

W systemie istnieją dwie role:

### Pracownik

Pracownik może:

* utworzyć nowy wniosek kredytowy,
* sprawdzić listę swoich wniosków,
* zobaczyć szczegóły swoich wniosków.

Pracownik nie może zmieniać statusu wniosku ani edytować decyzji przełożonego.

### Administrator

Administrator może:

* zobaczyć wszystkie wnioski,
* filtrować i wyszukiwać wnioski,
* sprawdzać szczegóły wniosków,
* zmieniać statusy wniosków,
* dodawać komentarze do decyzji,
* oznaczać wnioski jako błędne,
* edytować treść ogłoszenia systemowego,
* awaryjnie utworzyć wniosek.

## Ogłoszenie systemowe

Administrator może edytować treść ogłoszenia widocznego dla użytkowników systemu. Tytuł ogłoszenia nie jest edytowany z poziomu panelu, aby zachować stałą strukturę komunikatu.

Ogłoszenie służy do przekazywania pracownikom krótkich informacji organizacyjnych, np. przypomnienia o sprawdzeniu poprawności danych klienta przed złożeniem wniosku.

## Struktura projektu

Najważniejsze foldery projektu:

* `Controllers` — kontrolery MVC,
* `Models` — modele danych,
* `Views` — widoki Razor,
* `Data` — kontekst bazy danych i dane startowe,
* `Services` — logika scoringu kredytowego,
* `Migrations` — migracje Entity Framework Core,
* `Areas/Identity` — logowanie i obsługa kont użytkowników,
* `wwwroot` — pliki statyczne CSS, JS i zasoby.

## Czyszczenie bazy danych

Jeżeli chcesz usunąć aktualne dane testowe i utworzyć bazę od nowa, usuń bazę:

`LoanApplicationSystemDb`

w Visual Studio:

`View → SQL Server Object Explorer → (localdb)\MSSQLLocalDB → Databases → LoanApplicationSystemDb → Delete`

Po ponownym uruchomieniu projektu baza zostanie utworzona automatycznie.

## Uwagi techniczne

Projekt jest aplikacją demonstracyjną/edukacyjną. Nie jest przeznaczony do użycia produkcyjnego bez dodatkowych zabezpieczeń, audytu bezpieczeństwa i pełnej konfiguracji środowiska produkcyjnego.

Rejestracja użytkowników została wyłączona. Dostęp odbywa się przez konta utworzone podczas startu aplikacji.
