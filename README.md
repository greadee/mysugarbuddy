# My Sugar Buddy

My Sugar Buddy is a small C#/.NET desktop app project for tracking blood sugar data.

The long term idea is to connect to Dexcom G6 data, save readings locally, look for patterns in past glucose readings, and eventually show desktop notifications for high, low, rising fast, and dropping fast events.

For now this is still early setup work. I am adding the project structure first and will fill in the actual features in later phases.

## Current Status

- .NET 8 solution
- Empty project shells for Domain, Application, Infrastructure, and Desktop
- Desktop is still just a console app for now
- Basic project references are wired up
- No Dexcom connection yet
- No database yet
- No desktop UI yet

## Project Structure

- `MySugarBuddy.Domain` will hold the core models and blood sugar related logic.
- `MySugarBuddy.Application` will hold services and interfaces that coordinate app behavior.
- `MySugarBuddy.Infrastructure` will hold outside integrations like Dexcom, local storage, and notifications.
- `MySugarBuddy.Desktop` is the entry point for now. It may become an Avalonia desktop app later.

The main rule I want to follow is that the UI should not call Dexcom or database code directly. Also, glucose analysis should use normal app/domain models instead of depending on Dexcom-specific response objects.

## Safety Note

This project is experimental and is not a medical device.

It is not a replacement for Dexcom official alerts.

It is not a replacement for clinical care, emergency care, or finger-stick confirmation when required.

If the app ever estimates longer term glucose control, it should be labelled as GMI or an estimated A1C-like metric, not lab A1C.
