using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace SteamWorkshopUploader
{

    internal sealed class SteamUploader
    {
        public const uint AppId = 261550;

        public event Action<string> Logged;

        public event Action<bool, string> Finished;

        private CallResult<SubmitItemUpdateResult_t> _submission;
        private CallResult<CreateItemResult_t> _creation;
        private CallResult<SteamUGCQueryCompleted_t> _query;
        private UGCUpdateHandle_t _updateHandle = UGCUpdateHandle_t.Invalid;
        private UGCQueryHandle_t _queryHandle = UGCQueryHandle_t.Invalid;
        private WorkshopItem _item;
        private ulong _itemId;
        private List<WorkshopItem.Localization> _pendingLanguages = new List<WorkshopItem.Localization>();
        private int _writtenLanguageCount;

        public bool Connected { get; private set; }
        public bool IsRunning { get; private set; }
        public UGCUpdateHandle_t UpdateHandle { get { return _updateHandle; } }

        public bool Start()
        {
            string appIdFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steam_appid.txt");
            if (!File.Exists(appIdFile)) File.WriteAllText(appIdFile, AppId.ToString());

            try
            {
                Connected = SteamAPI.Init();
            }
            catch (Exception e)
            {
                Log("Steamworks could not be loaded: " + e.Message);
                Log("  -> steam_api64.dll must be next to the executable, and the tool must be built as x64.");
                Connected = false;
                return false;
            }

            if (!Connected)
            {
                Log("Could not connect to Steam.");
                Log("  - Steam must be running and logged in");
                Log("  - Bannerlord must be visible in your Steam library");
                return false;
            }

            Log("Connected to Steam. User: " + SteamFriends.GetPersonaName());
            return true;
        }

        public void Shutdown()
        {
            try { if (Connected) SteamAPI.Shutdown(); } catch { }
            Connected = false;
        }

        public void RunCallbacks()
        {
            try { if (Connected) SteamAPI.RunCallbacks(); } catch { }
        }

        public void Upload(WorkshopItem item, bool createNewItem)
        {
            if (IsRunning) return;
            if (!Connected) { Log("Steam is not connected."); return; }

            _item = item;
            IsRunning = true;

            List<string> blockers = item.Blockers();
            if (blockers.Count > 0 && !createNewItem)
            {
                foreach (string e in blockers) Log("BLOCKED: " + e);
                Finish(false, "Not uploaded");
                return;
            }

            if (createNewItem)
            {
                Log("Creating a new Steam Workshop item...");
                _creation = CallResult<CreateItemResult_t>.Create(CreationFinished);
                _creation.Set(SteamUGC.CreateItem(new AppId_t(AppId), EWorkshopFileType.k_EWorkshopFileTypeCommunity));
                return;
            }

            if (item.ItemId == 0)
            {
                Log("The item has no Workshop ID. Use 'Create Item' first.");
                Finish(false, "Missing item ID");
                return;
            }

            UploadItem(item.ItemId);
        }

        private void CreationFinished(CreateItemResult_t result, bool ioError)
        {
            if (ioError || result.m_eResult != EResult.k_EResultOK)
            {
                Fail(result.m_eResult, ioError);
                return;
            }

            ulong newValue = result.m_nPublishedFileId.m_PublishedFileId;
            _item.ItemId = newValue;
            try { _item.Save(); Log("New item ID saved to file: " + newValue); }
            catch (Exception e) { Log("The item ID could not be saved (" + e.Message + "). Write it down manually: " + newValue); }

            if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
                Log("The Workshop legal agreement has not been accepted; the item will stay hidden until it is accepted.");

            List<string> blockers = _item.Blockers();
            if (blockers.Count > 0)
            {

                foreach (string e in blockers) Log("BLOCKED: " + e);
                Finish(false, "Item created, content not uploaded");
                return;
            }

            UploadItem(newValue);
        }

        private void UploadItem(ulong itemId)
        {
            _itemId = itemId;
            _writtenLanguageCount = 0;
            _pendingLanguages = _item.Languages.FindAll(d => !d.IsEmpty && !string.IsNullOrWhiteSpace(d.Language));

            _updateHandle = SteamUGC.StartItemUpdate(new AppId_t(AppId), new PublishedFileId_t(itemId));
            if (_updateHandle == UGCUpdateHandle_t.Invalid)
            {
                Log("StartItemUpdate returned an invalid handle.");
                Log("  -> This item does not belong to this account or this game. Try 'Create Item'.");
                Finish(false, "Invalid handle");
                return;
            }

            SteamUGC.SetItemUpdateLanguage(_updateHandle, WorkshopItem.SteamDefaultLanguage);

            SteamUGC.SetItemContent(_updateHandle, _item.FullContentPath);
            SteamUGC.SetItemPreview(_updateHandle, _item.FullPreviewPath);
            SteamUGC.SetItemTitle(_updateHandle, _item.Title);
            SteamUGC.SetItemDescription(_updateHandle, _item.Description ?? "");
            SteamUGC.SetItemVisibility(_updateHandle, Visibility(_item.Visibility));
            SteamUGC.SetItemMetadata(_updateHandle, _item.Metadata ?? "");

            List<string> tags = _item.Tags.FindAll(e => !string.IsNullOrEmpty(e.Trim()));
            if (tags.Count > 0) SteamUGC.SetItemTags(_updateHandle, tags);

            Log("Uploading: " + itemId + "  <- " + _item.FullContentPath);

            string not = string.IsNullOrWhiteSpace(_item.ChangeNote) ? "Update" : _item.ChangeNote;
            _submission = CallResult<SubmitItemUpdateResult_t>.Create(SubmissionFinished);
            _submission.Set(SteamUGC.SubmitItemUpdate(_updateHandle, not));
        }

        private void SubmissionFinished(SubmitItemUpdateResult_t result, bool ioError)
        {
            if (!ioError && result.m_eResult == EResult.k_EResultOK)
            {
                if (_writtenLanguageCount == 0)
                {
                    Log("UPLOADED. (content + default text)");
                    if (result.m_bUserNeedsToAcceptWorkshopLegalAgreement)
                    {
                        Log("You must accept the Workshop legal agreement; the item will stay hidden until then:");
                        Log("  https://steamcommunity.com/sharedfiles/workshoplegalagreement");
                    }
                }
                else
                {
                    Log("  written: " + _lastLanguage);
                }

                UploadNextLanguage();
                return;
            }

            if (_writtenLanguageCount > 0)
            {
                Log("'" + _lastLanguage + "' translation could not be written. Steam result: " + result.m_eResult);
                Log("  -> " + Explain(result.m_eResult));
                Log("  Content was uploaded; only this language's title/description is missing.");
                Finish(true, "Uploaded, " + _lastLanguage + " translation missing");
                return;
            }

            Fail(result.m_eResult, ioError);
        }

        private void UploadNextLanguage()
        {
            if (_pendingLanguages.Count == 0)
            {
                Finish(true, _writtenLanguageCount > 0 ? "Completed (" + (_writtenLanguageCount + 1) + " languages)" : "Completed");
                return;
            }

            WorkshopItem.Localization language = _pendingLanguages[0];
            _pendingLanguages.RemoveAt(0);
            _lastLanguage = language.Language;
            _writtenLanguageCount++;

            _updateHandle = SteamUGC.StartItemUpdate(new AppId_t(AppId), new PublishedFileId_t(_itemId));
            if (_updateHandle == UGCUpdateHandle_t.Invalid)
            {
                Log("Invalid handle for '" + language.Language + "'; translation could not be written.");
                Finish(true, "Uploaded, " + language.Language + " translation missing");
                return;
            }

            SteamUGC.SetItemUpdateLanguage(_updateHandle, language.Language);
            SteamUGC.SetItemTitle(_updateHandle, language.Title ?? "");
            SteamUGC.SetItemDescription(_updateHandle, language.Description ?? "");

            Log("Uploading translation: " + language.Language);

            string not = string.IsNullOrWhiteSpace(_item.ChangeNote) ? "Update" : _item.ChangeNote;
            _submission = CallResult<SubmitItemUpdateResult_t>.Create(SubmissionFinished);
            _submission.Set(SteamUGC.SubmitItemUpdate(_updateHandle, not));
        }

        private string _lastLanguage = "";

        private void Fail(EResult result, bool ioError)
        {
            if (ioError)
            {
                Log("I/O error while communicating with Steam.");
            }
            else
            {

                Log("FAILED. Steam result: " + result + " (" + (int)result + ")");
                Log("  -> " + Explain(result));
            }

            Finish(false, "Failed");
        }

        private void Finish(bool successful, string message)
        {
            IsRunning = false;
            _updateHandle = UGCUpdateHandle_t.Invalid;
            Action<bool, string> h = Finished;
            if (h != null) h(successful, message);
        }

        public void ListItems()
        {
            if (!Connected) { Log("Steam is not connected."); return; }
            if (_queryHandle != UGCQueryHandle_t.Invalid) { Log("The previous query is still running."); return; }

            AccountID_t account = SteamUser.GetSteamID().GetAccountID();

            UGCQueryHandle_t query = SteamUGC.CreateQueryUserUGCRequest(
                account,
                EUserUGCList.k_EUserUGCList_Published,
                EUGCMatchingUGCType.k_EUGCMatchingUGCType_Items,
                EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc,
                new AppId_t(AppId),
                new AppId_t(AppId),
                1);

            if (query == UGCQueryHandle_t.Invalid)
            {
                Log("Could not create the query.");
                return;
            }

            _queryHandle = query;
            Log("Querying Steam for this account's Bannerlord items...");

            _query = CallResult<SteamUGCQueryCompleted_t>.Create(QueryFinished);
            _query.Set(SteamUGC.SendQueryUGCRequest(query));
        }

        private void QueryFinished(SteamUGCQueryCompleted_t result, bool ioError)
        {
            try
            {
                if (ioError || result.m_eResult != EResult.k_EResultOK)
                {
                    Log("Could not retrieve the item list. Steam result: " + result.m_eResult);
                    return;
                }

                if (result.m_unNumResultsReturned == 0)
                {
                    Log("This account has no published items for Bannerlord (261550).");
                    Log("  -> The ID in the file belongs to another account or another game.");
                    Log("  -> Use 'Create Item' to create a clean item.");
                    return;
                }

                Log("Found " + result.m_unNumResultsReturned + " item(s) on this account:");
                for (uint i = 0; i < result.m_unNumResultsReturned; i++)
                {
                    SteamUGCDetails_t details;
                    if (!SteamUGC.GetQueryUGCResult(_queryHandle, i, out details)) continue;

                    Log("  " + details.m_nPublishedFileId.m_PublishedFileId
                        + "   \"" + details.m_rgchTitle + "\""
                        + "   [" + VisibilityName(details.m_eVisibility) + "]"
                        + (details.m_eResult == EResult.k_EResultOK ? "" : "   (status: " + details.m_eResult + ")"));

                    if (!string.IsNullOrEmpty(details.m_rgchTags))
                        Log("      tags: " + details.m_rgchTags);
                }
                Log("Enter the correct ID in the 'Workshop ID' field and save.");
            }
            finally
            {

                if (_queryHandle != UGCQueryHandle_t.Invalid)
                {
                    SteamUGC.ReleaseQueryUGCRequest(_queryHandle);
                    _queryHandle = UGCQueryHandle_t.Invalid;
                }

                Action<bool, string> h = Queried;
                if (h != null) h(true, "");
            }
        }

        public event Action<bool, string> Queried;

        private static string VisibilityName(ERemoteStoragePublishedFileVisibility g)
        {
            switch (g)
            {
                case ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic: return "public";
                case ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly: return "friends only";
                case ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate: return "private";
                default: return "unlisted";
            }
        }

        public static ERemoteStoragePublishedFileVisibility Visibility(int value)
        {
            switch (value)
            {
                case 1: return ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly;
                case 2: return ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
                case 3: return ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted;
                default: return ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic;
            }
        }

        public static string Explain(EResult result)
        {
            switch (result)
            {
                case EResult.k_EResultFail:

                    return "Steam did not provide a specific reason. Check these in order: "
                         + "(1) is a file in the content folder locked by another program "
                         + "- the mod's own .log file can stay open while the game or server is running; "
                         + "(2) is the preview image readable and under 1 MB; "
                         + "(3) does the item ID belong to this account ('List Workshop IDs'); "
                         + "(4) is the Steam client running and logged in.";

                case EResult.k_EResultAccessDenied:
                    return "The item does not belong to this account. Create a new one with 'Create Item'.";
                case EResult.k_EResultFileNotFound:

                    return "Steam could not find this ITEM. Your local files are present; the issue is the ID. "
                         + "The ID does not belong to this account, belongs to another game, or was deleted. "
                         + "Use 'List Workshop IDs' to see the real IDs on this account.";
                case EResult.k_EResultInvalidParam:
                    return "Invalid parameter - usually the item ID is wrong, an empty tag was sent, or the item belongs to another game.";
                case EResult.k_EResultLimitExceeded:

                    return "A size limit was exceeded. Check the preview image first (hard 1 MB limit); "
                         + "if the image is small, the exceeded limit is the game's total Workshop item limit.";
                case EResult.k_EResultTimeout:
                    return "Steam timed out. Try again.";
                case EResult.k_EResultNotLoggedOn:
                    return "Steam is not logged in.";
                case EResult.k_EResultBanned:
                    return "The account or item is banned from Workshop.";
                case EResult.k_EResultInsufficientPrivilege:
                    return "This account is not allowed to upload to Workshop (Steam Guard / community restriction).";
                case EResult.k_EResultServiceUnavailable:
                    return "Steam is currently unavailable. Try again in a few minutes.";
                default:
                    return "Steam returned this result code; look it up in the Steamworks EResult list.";
            }
        }

        public static string StatusName(EItemUpdateStatus status)
        {
            switch (status)
            {
                case EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig: return "preparing configuration";
                case EItemUpdateStatus.k_EItemUpdateStatusPreparingContent: return "preparing content";
                case EItemUpdateStatus.k_EItemUpdateStatusUploadingContent: return "uploading content";
                case EItemUpdateStatus.k_EItemUpdateStatusUploadingPreviewFile: return "uploading preview";
                case EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges: return "committing changes";
                case EItemUpdateStatus.k_EItemUpdateStatusInvalid: return "waiting";
                default: return status.ToString();
            }
        }

        private void Log(string line)
        {
            Action<string> h = Logged;
            if (h != null) h(line);
        }
    }
}
