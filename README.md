# My Sugar Buddy

My Sugar Buddy is a small C#/.NET desktop app project for tracking blood sugar data.

The long term idea is to connect to Dexcom G6 data, save readings locally, look for patterns in past glucose readings, and eventually show desktop notifications for high, low, rising fast, and dropping fast events.

For now this is still early setup work. The app loads sample readings, saves them locally, calculates a few basic stats, and prints alert information in the console.

## Current Status

- .NET 8 solution
- Domain, Application, Infrastructure, and Desktop projects are wired together
- Desktop is still just a console app for now
- Sample CSV readings load into the app
- Readings are saved locally with SQLite
- Basic summaries, GMI estimate, recent history, trend, and alerts are printed
- First Dexcom Web API reading source is wired behind the reading-source interface when `DEXCOM_ACCESS_TOKEN` is set
- No desktop UI yet

## Project Structure

- `MySugarBuddy.Domain` will hold the core models and blood sugar related logic.
- `MySugarBuddy.Application` will hold services and interfaces that coordinate app behavior.
- `MySugarBuddy.Infrastructure` will hold outside integrations like Dexcom, local storage, and notifications.
- `MySugarBuddy.Desktop` is the entry point for now. It may become an Avalonia desktop app later.

The main rule I want to follow is that the UI should not call Dexcom or database code directly. Also, glucose analysis should use normal app/domain models instead of depending on Dexcom-specific response objects.

## Dexcom Sync

By default the desktop app still loads `samples/glucose-readings.csv`.

To try the first Dexcom-backed source, set `DEXCOM_ACCESS_TOKEN` before running the app. The adapter calls the Dexcom EGV endpoint for the last three hours and converts the response into normal `GlucoseReading` objects before the application service saves them locally.

`DEXCOM_API_BASE_URL` can point the adapter at another Dexcom-compatible base URL for sandbox testing. If it is not set, the app uses `https://api.dexcom.com`.

## Safety Note

This project is experimental and is not a medical device.

It is not a replacement for Dexcom official alerts.

It is not a replacement for clinical care, emergency care, or finger-stick confirmation when required.

If the app ever estimates longer term glucose control, it should be labelled as GMI or an estimated A1C-like metric, not lab A1C.
