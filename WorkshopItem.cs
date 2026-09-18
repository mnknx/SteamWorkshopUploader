using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SteamWorkshopUploader
{

    internal sealed class WorkshopItem
    {
        public const string Extension = ".workshop.json";

        public const long PreviewLimit = 1024 * 1024;

        public const long LargeFileThreshold = 250L * 1024 * 1024;

        private static readonly string[] BackupPatterns = { ".bak", ".old", ".orig", ".tmp", ".log", ".zip", ".rar", ".7z" };

        private static readonly string[] KnownTags =
        {

            "Graphical Enhancement", "Map Pack", "Partial Conversion", "Sound", "Total Conversion",
            "Troops", "UI", "Utility", "Weapons and Armour",

            "Native", "Antiquity", "Dark Ages", "Medieval", "Musket Era", "Modern", "Sci-Fi",
            "Fantasy", "Oriental", "Apocalypse", "Other",

            "Singleplayer", "Multiplayer",

            "v1.2.12",
            "v1.3.4", "v1.3.5", "v1.3.6", "v1.3.7", "v1.3.8", "v1.3.9", "v1.3.10", "v1.3.11",
            "v1.3.12", "v1.3.13", "v1.3.14", "v1.3.15",
            "v1.4.5", "v1.4.6", "v1.4.7", "v1.4.8",

            "War Sails"
        };

        public string FilePath { get; private set; }
        public string RootFolder { get; private set; }

        public ulong ItemId;
        public string ContentFolder = "";
        public string PreviewFile = "";
        public int Visibility;
        public string Title = "";
        public string Description = "";
        public string Metadata = "";
        public List<string> Tags = new List<string>();
        public string ChangeNote = "";

        public List<Localization> Languages = new List<Localization>();

        public bool LegacyFormat;

        public const string PrimaryLanguage = "turkish";

        public const string SteamDefaultLanguage = "english";

        public static readonly string[] DefaultLanguages = { PrimaryLanguage, SteamDefaultLanguage };

        public static readonly string[][] AllLanguages =
        {
            new[] { "turkish", "Turkish" },
            new[] { "english", "English" },
            new[] { "arabic", "Arabic" },
            new[] { "bulgarian", "Bulgarian" },
            new[] { "schinese", "Chinese (Simplified)" },
            new[] { "tchinese", "Chinese (Traditional)" },
            new[] { "czech", "Czech" },
            new[] { "danish", "Danish" },
            new[] { "dutch", "Dutch" },
            new[] { "finnish", "Finnish" },
            new[] { "french", "French" },
            new[] { "german", "German" },
            new[] { "greek", "Greek" },
            new[] { "hungarian", "Hungarian" },
            new[] { "indonesian", "Indonesian" },
            new[] { "italian", "Italian" },
            new[] { "japanese", "Japanese" },
            new[] { "koreana", "Korean" },
            new[] { "malay", "Malay" },
            new[] { "norwegian", "Norwegian" },
            new[] { "polish", "Polish" },
            new[] { "portuguese", "Portuguese" },
            new[] { "brazilian", "Portuguese-Brazil" },
            new[] { "romanian", "Romanian" },
            new[] { "russian", "Russian" },
            new[] { "spanish", "Spanish-Spain" },
            new[] { "latam", "Spanish-Latin America" },
            new[] { "swedish", "Swedish" },
            new[] { "thai", "Thai" },
            new[] { "ukrainian", "Ukrainian" },
            new[] { "vietnamese", "Vietnamese" }
        };

        public static string LanguageLabel(string code)
        {
            foreach (string[] d in AllLanguages)
                if (string.Equals(d[0], code, StringComparison.OrdinalIgnoreCase))
                    return d[0] + " - " + d[1];
            return code;
        }

        public static bool IsKnownLanguage(string code)
        {
            foreach (string[] d in AllLanguages)
                if (string.Equals(d[0], code, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public List<string> LanguageOrder()
        {
            List<string> index = new List<string>(DefaultLanguages);
            foreach (Localization d in Languages)
                if (!index.Exists(s => string.Equals(s, d.Language, StringComparison.OrdinalIgnoreCase)))
                    index.Add(d.Language);
            return index;
        }

        internal sealed class Localization
        {
            public string Language = "";
            public string Title = "";
            public string Description = "";

            public bool IsEmpty
            {
                get { return string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Description); }
            }
        }

        public Localization FindLanguage(string language, bool create)
        {
            Localization y = Languages.Find(d => string.Equals(d.Language, language, StringComparison.OrdinalIgnoreCase));
            if (y == null && create)
            {
                y = new Localization { Language = language };
                Languages.Add(y);
            }
            return y;
        }

        private JsonObject _ham = new JsonObject();

        public string Name
        {
            get
            {
                string name = Path.GetFileName(FilePath) ?? "";
                return name.EndsWith(Extension, StringComparison.OrdinalIgnoreCase)
                    ? name.Substring(0, name.Length - Extension.Length)
                    : Path.GetFileNameWithoutExtension(name);
            }
        }

        public string FullContentPath { get { return ResolvePath(ContentFolder); } }
        public string FullPreviewPath { get { return ResolvePath(PreviewFile); } }

        private string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            try
            {
                return Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(RootFolder, path));
            }
            catch
            {
                return path;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static List<WorkshopItem> Scan(string rootFolder, List<string> errors)
        {
            List<WorkshopItem> list = new List<WorkshopItem>();
            if (string.IsNullOrEmpty(rootFolder) || !Directory.Exists(rootFolder)) return list;

            foreach (string path in Directory.GetFiles(rootFolder, "*" + Extension, SearchOption.TopDirectoryOnly)
                                            .OrderBy(y => y, StringComparer.OrdinalIgnoreCase))
            {
                try
                {
                    list.Add(Load(path, rootFolder));
                }
                catch (Exception e)
                {
                    if (errors != null) errors.Add(Path.GetFileName(path) + " could not be read: " + e.Message);
                }
            }

            return list;
        }

        public static WorkshopItem Load(string filePath, string rootFolder)
        {
            JsonObject j = Json.Parse(File.ReadAllText(filePath)) as JsonObject;
            if (j == null) throw new FormatException("The file root is not a JSON object.");

            WorkshopItem item = new WorkshopItem
            {
                FilePath = filePath,
                RootFolder = rootFolder,
                _ham = j,
                ContentFolder = j.Text("contentfolder"),
                PreviewFile = j.Text("previewfile"),
                Visibility = j.Number("visibility", 0),
                Title = j.Text("title"),
                Description = j.Text("description"),
                Metadata = j.Text("metadata"),
                Tags = j.Strings("tags"),
                ChangeNote = j.Text("changenote")
            };

            ulong id;
            ulong.TryParse(j.Text("publishedfileid"), NumberStyles.Integer, CultureInfo.InvariantCulture, out id);
            item.ItemId = id;

            JsonObject languages = j.Get("languages") as JsonObject;
            if (languages != null)
            {
                foreach (KeyValuePair<string, object> pair in languages)
                {
                    JsonObject g = pair.Value as JsonObject;
                    if (g == null) continue;

                    item.Languages.Add(new Localization
                    {
                        Language = pair.Key,
                        Title = g.Text("title"),
                        Description = g.Text("description")
                    });
                }
            }

            if (languages == null && !(string.IsNullOrWhiteSpace(item.Title) && string.IsNullOrWhiteSpace(item.Description)))
            {
                item.LegacyFormat = true;
                Localization primary = item.FindLanguage(PrimaryLanguage, true);
                primary.Title = item.Title;
                primary.Description = item.Description;
            }

            return item;
        }

        public static WorkshopItem CreateNew(string rootFolder, string name)
        {
            string file = Path.Combine(rootFolder, name + Extension);
            if (File.Exists(file)) throw new IOException("An item with this name already exists: " + Path.GetFileName(file));

            WorkshopItem item = new WorkshopItem
            {
                FilePath = file,
                RootFolder = rootFolder,
                ItemId = 0,
                ContentFolder = Directory.Exists(Path.Combine(rootFolder, name)) ? name : "",
                PreviewFile = File.Exists(Path.Combine(rootFolder, "preview.png")) ? "preview.png" : "",
                Visibility = 2,
                Title = name,
                ChangeNote = "Initial release"
            };

            item.Save();
            return item;
        }

        private void BuildFallback()
        {
            Localization primary = FindLanguage(PrimaryLanguage, false);
            Localization backups = FindLanguage(SteamDefaultLanguage, false);

            Localization source = (primary != null && !string.IsNullOrWhiteSpace(primary.Title)) ? primary : backups;
            if (source == null) source = primary;
            if (source == null) return;

            Title = source.Title ?? "";
            Description = source.Description ?? "";
        }

        public void Save()
        {
            BuildFallback();

            JsonObject output = new JsonObject();
            output.Set("publishedfileid", ItemId.ToString(CultureInfo.InvariantCulture));
            output.Set("contentfolder", ContentFolder ?? "");
            output.Set("previewfile", PreviewFile ?? "");
            output.Set("visibility", (double)Visibility);
            output.Set("title", Title ?? "");
            output.Set("description", Description ?? "");
            output.Set("metadata", Metadata ?? "");
            output.Set("tags", Tags.Where(e => !string.IsNullOrEmpty(e)).Cast<object>().ToList());
            output.Set("changenote", ChangeNote ?? "");

            JsonObject languages = new JsonObject();
            foreach (string languageName in LanguageOrder())
            {
                Localization d = FindLanguage(languageName, false);
                bool defaultValue = Array.Exists(DefaultLanguages, v => string.Equals(v, languageName, StringComparison.OrdinalIgnoreCase));
                if (!defaultValue && (d == null || d.IsEmpty)) continue;

                JsonObject g = new JsonObject();
                g.Set("title", d == null ? "" : (d.Title ?? ""));
                g.Set("description", d == null ? "" : (d.Description ?? ""));
                languages.Set(languageName, g);
            }

            output.Set("languages", languages);

            foreach (KeyValuePair<string, object> pair in _ham)
                if (!output.Has(pair.Key)) output.Set(pair.Key, pair.Value);

            File.WriteAllText(FilePath, Json.Write(output));
            _ham = output;
        }

        public List<string> Blockers()
        {
            List<string> blockers = new List<string>();

            BuildFallback();

            if (string.IsNullOrWhiteSpace(ContentFolder))
                blockers.Add("Content folder is empty.");
            else if (!Directory.Exists(FullContentPath))
                blockers.Add("Content folder does not exist: " + FullContentPath);
            else if (Directory.GetFileSystemEntries(FullContentPath).Length == 0)
                blockers.Add("Content folder is empty: " + FullContentPath);

            if (string.IsNullOrWhiteSpace(PreviewFile))
                blockers.Add("Preview image is empty.");
            else if (!File.Exists(FullPreviewPath))
                blockers.Add("Preview image does not exist: " + FullPreviewPath);
            else
            {
                long size = new FileInfo(FullPreviewPath).Length;
                if (size > PreviewLimit)
                    blockers.Add("Preview image is " + FormatSize(size) + " - Steam limit is 1 MB. Make it smaller.");
            }

            if (string.IsNullOrWhiteSpace(Title))
                blockers.Add("Title is empty.");

            return blockers;
        }

        public List<string> Warnings()
        {
            List<string> warnings = new List<string>();

            if (Tags.Count == 0)
                warnings.Add("No tags - the item will be harder to find in Workshop searches.");
            warnings.AddRange(TagWarnings());
            warnings.AddRange(LanguageWarnings());

            if (!Directory.Exists(FullContentPath)) return warnings;

            long total = 0;
            int count = 0;
            List<string> largeFiles = new List<string>();
            List<string> backups = new List<string>();

            foreach (string path in Directory.GetFiles(FullContentPath, "*", SearchOption.AllDirectories))
            {
                FileInfo info;
                try { info = new FileInfo(path); } catch { continue; }

                total += info.Length;
                count++;

                if (info.Length > LargeFileThreshold)
                    largeFiles.Add(RelativePath(path) + " (" + FormatSize(info.Length) + ")");

                string name = info.Name;
                if (name.EndsWith("~", StringComparison.Ordinal) ||
                    BackupPatterns.Any(d => name.EndsWith(d, StringComparison.OrdinalIgnoreCase)))
                    backups.Add(RelativePath(path));
            }

            warnings.Add(count + " files, total " + FormatSize(total) + ".");

            foreach (string b in largeFiles)
                warnings.Add("Large file: " + b + " - it can be uploaded, but even a small change inside it "
                          + "may force subscribers to download most of this file again.");

            if (backups.Count > 0)
                warnings.Add("Backup/archive files were found in the upload folder. Remove them: " + string.Join(", ", backups.Take(8).ToArray())
                          + (backups.Count > 8 ? " (+" + (backups.Count - 8) + ")" : ""));

            return warnings;
        }

        private List<string> LanguageWarnings()
        {
            List<string> warnings = new List<string>();

            if (LegacyFormat)
                warnings.Add("Old file format: existing text was imported as " + PrimaryLanguage + ". "
                          + "If the text is actually English, move it to the EN tab and save.");

            foreach (Localization d in Languages)
            {
                if (d.IsEmpty) continue;

                if (!IsKnownLanguage(d.Language))
                    warnings.Add("'" + d.Language + "' is not in Steam's language list - this text may be submitted "
                              + "but will not be written anywhere. Choose the correct code from 'Add Language'.");

                if (string.IsNullOrWhiteSpace(d.Title))
                    warnings.Add(d.Language + " translation has an empty title - players using that language will see an untitled item.");

                if (string.IsNullOrWhiteSpace(d.Description))
                    warnings.Add(d.Language + " translation has an empty description - no description will appear in that language.");
            }

            Localization primary = FindLanguage(PrimaryLanguage, false);
            Localization english = FindLanguage(SteamDefaultLanguage, false);

            if (primary == null || primary.IsEmpty)
                warnings.Add("Primary language (" + PrimaryLanguage + ") is empty.");

            if (english == null || english.IsEmpty)
                warnings.Add("No English translation - English players and all players without a specific translation "
                          + "will see the " + PrimaryLanguage + " text.");

            List<Localization> filled = Languages.FindAll(d => !d.IsEmpty);
            if (filled.Count > 0)
                warnings.Add("Languages to submit: " + string.Join(", ", filled.ConvertAll(d => d.Language).ToArray())
                          + " - content is uploaded once, then each language is written in a separate submission.");

            return warnings;
        }

        private List<string> TagWarnings()
        {
            List<string> warnings = new List<string>();

            foreach (string raw in Tags)
            {
                string tag = (raw ?? "").Trim();
                if (tag.Length == 0) continue;

                if (KnownTags.Any(b => string.Equals(b, tag, StringComparison.OrdinalIgnoreCase)))
                    continue;

                int colon = tag.IndexOf(':');
                if (colon >= 0)
                {
                    string value = tag.Substring(colon + 1).Trim();
                    string correct = KnownTags.FirstOrDefault(b => string.Equals(b, value, StringComparison.OrdinalIgnoreCase));
                    if (correct != null)
                    {
                        warnings.Add("Tag contains a group name: \"" + tag + "\" - Steam expects only the value. Use \"" + correct + "\".");
                        continue;
                    }
                }

                warnings.Add("\"" + tag + "\" is not a predefined Bannerlord tag - it will be submitted as a custom tag, "
                          + "but it will not appear in Workshop filters.");
            }

            return warnings;
        }

        private string RelativePath(string path)
        {
            string rootFolder = FullContentPath;
            return path.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase)
                ? path.Substring(rootFolder.Length).TrimStart('\\', '/')
                : path;
        }

        public static string FormatSize(long bytes)
        {
            if (bytes >= 1024L * 1024 * 1024) return (bytes / 1024.0 / 1024 / 1024).ToString("0.0", CultureInfo.InvariantCulture) + " GB";
            if (bytes >= 1024 * 1024) return (bytes / 1024.0 / 1024).ToString("0.0", CultureInfo.InvariantCulture) + " MB";
            if (bytes >= 1024) return (bytes / 1024) + " KB";
            return bytes + " B";
        }
    }
}
