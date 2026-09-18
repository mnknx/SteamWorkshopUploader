using System;
using System.IO;

namespace SteamWorkshopUploader
{

    internal static class Workspace
    {
        private const string SettingsFile = "uploader.json";
        private const string FolderName = "ModPack";
        private const int ParentLimit = 8;

        public static string ExecutableFolder
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public static string Find()
        {
            string selected = ReadSaved();
            if (!string.IsNullOrEmpty(selected) && Directory.Exists(selected)) return selected;

            DirectoryInfo d = new DirectoryInfo(ExecutableFolder);
            for (int i = 0; i < ParentLimit && d != null; i++, d = d.Parent)
            {

                if (string.Equals(d.Name, FolderName, StringComparison.OrdinalIgnoreCase)) return d.FullName;

                string candidate = Path.Combine(d.FullName, FolderName);
                if (Directory.Exists(candidate)) return candidate;
            }

            return ExecutableFolder;
        }

        public static void Log(string rootFolder)
        {
            try
            {
                JsonObject j = new JsonObject();
                j.Set("modpack", rootFolder ?? "");
                File.WriteAllText(Path.Combine(ExecutableFolder, SettingsFile), Json.Write(j));
            }
            catch
            {

            }
        }

        private static string ReadSaved()
        {
            try
            {
                string path = Path.Combine(ExecutableFolder, SettingsFile);
                if (!File.Exists(path)) return null;

                JsonObject j = Json.Parse(File.ReadAllText(path)) as JsonObject;
                return j == null ? null : j.Text("modpack");
            }
            catch
            {
                return null;
            }
        }
    }
}
