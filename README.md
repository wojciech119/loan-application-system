# System oceny zdolności kredytowej

Projekt przedstawia aplikację webową wspierającą pracę banku przy obsłudze wniosków kredytowych klientów. System umożliwia tworzenie wniosków, automatyczne obliczanie wybranych parametrów finansowych oraz zarządzanie statusem sprawy przez przełożonego.

## Opis projektu

Aplikacja została przygotowana jako system wspomagający analizę wniosków kredytowych. Pracownik banku wprowadza dane klienta, takie jak dochód, zobowiązania, kwota kredytu, okres spłaty, forma zatrudnienia oraz historia kredytowa. Na podstawie tych danych system oblicza między innymi dochód rozporządzalny, szacowaną ratę, wskaźnik DTI oraz wynik scoringowy.

Wynik systemu ma charakter pomocniczy i nie stanowi ostatecznej decyzji banku.

## Główne funkcje

* rejestracja i logowanie użytkowników,
* obsługa ról użytkowników,
* tworzenie wniosków kredytowych,
* automatyczna ocena zdolności kredytowej,
* obliczanie scoringu, DTI oraz szacowanej raty,
* panel przełożonego z dostępem do wszystkich wniosków,
* zmiana statusu wniosku,
* historia decyzji przełożonego,
* zarządzanie podstawowymi danymi konta użytkownika.

## Role w systemie

### Pracownik banku

Pracownik może tworzyć nowe wnioski kredytowe oraz przeglądać sprawy utworzone przez siebie.

### Przełożony

Przełożony ma dostęp do panelu administracyjnego, w którym może przeglądać wszystkie wnioski, analizować szczegóły sprawy oraz zmieniać status wniosku.

### Klient

Klient nie loguje się do systemu. Jego dane są wprowadzane przez pracownika banku podczas obsługi wniosku.

## Technologie

* ASP.NET Core MVC
* ASP.NET Core Identity
* Entity Framework Core
* SQL Server LocalDB
* Razor Views
* Bootstrap
* C#

## Przykładowe statusy wniosku

* Złożony
* W trakcie analizy
* Wymaga uzupełnienia
* Zaakceptowany
* Odrzucony

## Uruchomienie projektu

1. Otwórz plik `LoanApplicationSystem.sln` w Visual Studio.
2. Sprawdź połączenie z bazą danych w pliku `appsettings.json`.
3. Wykonaj migracje bazy danych, jeśli są wymagane.
4. Uruchom projekt w Visual Studio.
5. Zarejestruj konto użytkownika i zaloguj się do aplikacji.

## Dane testowe

Do testów można dodać przykładowe wnioski kredytowe z różnymi wynikami scoringu: pozytywnym, warunkowym oraz negatywnym.

## Charakter projektu

Projekt ma charakter edukacyjny i demonstracyjny. Do aplikacji należy wprowadzać wyłącznie dane testowe.
