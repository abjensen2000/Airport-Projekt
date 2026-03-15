# Feedback til abjensen - Airport Information System

Virkelig godt gået med dit projekt! Du har fået styr på de grundlæggende principper, og det er især fedt at se, at du har kastet dig over bonusopgaven med **Topics**.

## ASP.NET Core Web API
- **Models:** Din `Flight` model bruger private fields og manuelle getters/setters. I moderne C# bruger vi ofte **auto-properties** (`public string FlightNumber { get; set; }`), da det gør koden kortere og mere overskuelig. Jeg har opdateret den til dig som et eksempel.
- **Data Typer:** Du bruger `string` til `DepartureTime`. For at kunne sortere og arbejde med tid, er det en god idé at bruge `DateTime`.

## RabbitMQ Integration
- **Topics & Routing:** Det er en super smart detalje, at du grupperer fly efter destination og bruger destinationen som routing-key! Det viser en god forståelse for, hvordan man kan filtrere beskeder i et distribueret system.
- **Async Mønstre:** Jeg lagde mærke til `async void OpdaterSubscribers`. I C# bør vi altid returnere en `Task` ved asynkrone metoder (undtagen i event-handlers). `async void` gør det svært at fange fejl og kan crashe hele applikationen. Jeg har rettet det til `async Task OpdaterSubscribersAsync`.
- **Resource Management:** Husk altid at rydde op efter dig selv med `using` eller `await using`, når du bruger RabbitMQ-kanaler. Ellers kan du risikere at løbe tør for ressourcer (memory-leak).

## Konsol Applikation (Flight Info Screen)
- **Interaktivitet:** Det er rigtig godt, at man selv kan vælge lufthavn ved opstart!
- **Navngivning:** I din `PrintOrders` metode bruger du ordet "Orders". I dette domæne er det nok mere naturligt at kalde det "Flights".

Super flot arbejde! Din løsning med destination-topics er meget inspirerende.
