using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Steamworks;

namespace SteamWorkshopUploader
{

    internal static class Program
    {
        private static bool _finished;
        private static bool _successful;

        [STAThread]
        private static int Main(string[] args)
        {
            string command = args.Length > 0 ? args[0].ToLowerInvariant() : "";
            bool consoleMode = command == "console" || command == "create" || command == "items";

            if (!consoleMode)
            {
                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
                System.Windows.Forms.Application.Run(new MainWindow());
                return 0;
            }

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Log("=== Workshop Uploader ===", ConsoleColor.Cyan);

            SteamUploader steam = new SteamUploader();
            steam.Logged += s => Console.WriteLine(s);
            steam.Finished += (successful, message) => { _successful = successful; _finished = true; };
            steam.Queried += (successful, message) => { _successful = successful; _finished = true; };

            try
            {

                if (command == "items")
                {
                    if (!steam.Start()) return 1;
                    steam.ListItems();
                    WaitForSteam(steam);
                    return _successful ? 0 : 1;
                }

                string rootFolder = Workspace.Find();
                Log("ModPack: " + rootFolder, ConsoleColor.DarkGray);

                WorkshopItem item = SelectItem(rootFolder, args.Length > 1 ? args[1] : null);
                if (item == null) return 1;

                Log("Item   : " + item.Name + "  (" + (item.ItemId == 0 ? "no ID" : item.ItemId.ToString()) + ")");
                Log("Content: " + item.FullContentPath);
                Log("Preview: " + item.FullPreviewPath);

                List<WorkshopItem.Localization> translations = item.Languages.FindAll(d => !d.IsEmpty);
                if (translations.Count > 0)
                    Log("Languages: " + string.Join(", ", translations.ConvertAll(d => d.Language).ToArray())
                        + "   (primary: " + WorkshopItem.PrimaryLanguage + ")");

                Console.WriteLine();

                foreach (string u in item.Warnings()) Log("  " + u, ConsoleColor.DarkGray);

                List<string> blockers = item.Blockers();
                if (blockers.Count > 0 && command != "create")
                {
                    foreach (string e in blockers) Log("BLOCKED: " + e, ConsoleColor.Red);
                    return 1;
                }

                if (!steam.Start()) return 1;

                steam.Upload(item, command == "create");
                WaitForSteam(steam);
                return _successful ? 0 : 1;
            }
            catch (Exception e)
            {
                Log("UNEXPECTED ERROR: " + e.Message, ConsoleColor.Red);
                Console.WriteLine(e.StackTrace);
                return 1;
            }
            finally
            {
                steam.Shutdown();
                Console.WriteLine();
                Console.WriteLine("Press any key to close...");
                try { Console.ReadKey(true); } catch { }
            }
        }

        private static WorkshopItem SelectItem(string rootFolder, string name)
        {
            List<string> errors = new List<string>();
            List<WorkshopItem> allItems = WorkshopItem.Scan(rootFolder, errors);
            foreach (string h in errors) Log("ERROR: " + h, ConsoleColor.Red);

            if (allItems.Count == 0)
            {
                Log("No *.workshop.json files found in this folder: " + rootFolder, ConsoleColor.Red);
                return null;
            }

            if (string.IsNullOrEmpty(name))
            {
                if (allItems.Count == 1) return allItems[0];

                Log("Multiple items found. Specify which one to use:", ConsoleColor.Yellow);
                foreach (WorkshopItem i in allItems) Log("  " + i.Name, ConsoleColor.Yellow);
                return null;
            }

            WorkshopItem found = allItems.Find(i => string.Equals(i.Name, name, StringComparison.OrdinalIgnoreCase));
            if (found == null)
            {
                Log("No item found with this name: " + name, ConsoleColor.Red);
                foreach (WorkshopItem i in allItems) Log("  " + i.Name, ConsoleColor.Yellow);
            }
            return found;
        }

        private static void WaitForSteam(SteamUploader steam)
        {
            EItemUpdateStatus previous = EItemUpdateStatus.k_EItemUpdateStatusInvalid;
            int guard = 0;

            while (!_finished && guard++ < 18000)
            {
                steam.RunCallbacks();

                if (steam.IsRunning)
                {
                    ulong done, total;
                    EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(steam.UpdateHandle, out done, out total);
                    if (status != previous)
                    {
                        previous = status;
                        Log("  status: " + SteamUploader.StatusName(status)
                            + (total > 0 ? "  " + (done * 100 / total) + "%" : ""));
                    }
                }

                Thread.Sleep(100);
            }

            if (!_finished) Log("Steam did not respond within 30 minutes; aborting.", ConsoleColor.Red);
        }

        private static void Log(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = old;
        }
    }
}
