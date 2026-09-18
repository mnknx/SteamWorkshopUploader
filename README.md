# Steam Workshop Uploader

A small Windows desktop tool for uploading Mount & Blade II: Bannerlord mods to Steam Workshop.

The app is built around Steamworks.NET and uses the Steam API files from the local Bannerlord installation. It can create new Workshop items, update existing items, validate common upload problems, and manage localized title/description fields before upload.

## Features

- Windows Forms interface for managing `*.workshop.json` item files
- Steam Workshop upload support for Bannerlord app ID `261550`
- Create a new Workshop item and save the returned published file ID
- Upload content folders and preview images
- Validate missing content, missing preview files, oversized preview images, tags, and language data
- List Workshop items published by the current Steam account
- Optional console mode for scripted uploads

## Requirements

- Windows
- Steam client installed, running, and logged in
- Mount & Blade II: Bannerlord installed through Steam
- .NET SDK with .NET Framework 4.7.2 targeting support
- Bannerlord game files available at the path configured in `Uploader.csproj`

By default, the project expects Bannerlord files here:

```text
C:\Program Files (x86)\Steam\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client
```

If your game is installed somewhere else, update the `GameBin` property in `Uploader.csproj`.

## Build

```powershell
dotnet build Uploader.csproj -c Release
```

The build copies `steam_api64.dll` and writes `steam_appid.txt` to the output folder.

## Usage

Run the application without arguments to open the desktop UI:

```powershell
SteamWorkshopUploader.exe
```

Console commands:

```powershell
SteamWorkshopUploader.exe console [mod-name]
SteamWorkshopUploader.exe create [mod-name]
SteamWorkshopUploader.exe items
```

Command meanings:

- `console`: upload an existing Workshop item
- `create`: create a new Steam Workshop item, save its ID, then upload it
- `items`: list Workshop items owned by the current Steam account for Bannerlord

`[mod-name]` is the name of a `*.workshop.json` file without the `.workshop.json` extension. If there is only one item file, it can be omitted.

## Workshop Item Files

Each mod is configured with a `*.workshop.json` file. The main fields are:

- `publishedfileid`: Steam Workshop item ID
- `contentfolder`: folder to upload
- `previewfile`: preview image path
- `visibility`: Steam visibility value
- `title`: fallback title
- `description`: fallback description
- `metadata`: optional Steam metadata
- `tags`: Workshop tags
- `changenote`: update note
- `languages`: localized title and description values

Relative paths are resolved from the selected mod pack folder.

## Notes

Preview images must be under Steam's 1 MB limit. The app performs local checks before upload, but Steam may still reject an update if the item ID belongs to another account, another app, or a deleted Workshop item.
