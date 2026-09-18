using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Steamworks;

namespace SteamWorkshopUploader
{

    public partial class MainWindow : Form
    {
        private readonly SteamUploader _steam = new SteamUploader();

        private string _rootFolder;
        private List<WorkshopItem> _items = new List<WorkshopItem>();
        private WorkshopItem _selectedItem;
        private bool _loadingFields;
        private bool _dirty;
        private int _languageIndex;
        private List<string> _languageCodes = new List<string>();
        private EItemUpdateStatus _previousStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;

        public MainWindow()
        {
            InitializeComponent();

            _steam.Logged += Log;
            _steam.Finished += SteamFinished;
            _steam.Queried += (successful, message) => RefreshButtons();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (_steam.Start())
            {
                SetStatus("Connected: " + SteamFriends.GetPersonaName(), Color.SeaGreen);
                timerHeartbeat.Start();
            }
            else
            {
                SetStatus("Could not connect to Steam", Color.Firebrick);
            }

            _rootFolder = Workspace.Find();
            txtRootFolder.Text = _rootFolder;
            Scan(null);
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_steam.IsRunning)
            {
                DialogResult c = MessageBox.Show(this,
                    "An upload is in progress. Closing now may interrupt the Steam update.\nClose anyway?",
                    "Upload in progress", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (c != DialogResult.Yes) { e.Cancel = true; return; }
            }

            if (!ResolveDirty()) { e.Cancel = true; return; }

            timerHeartbeat.Stop();
            _steam.Shutdown();
        }

        private void timerHeartbeat_Tick(object sender, EventArgs e)
        {
            _steam.RunCallbacks();
            if (!_steam.IsRunning) return;

            ulong done, total;
            EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(_steam.UpdateHandle, out done, out total);

            if (status != _previousStatus)
            {
                _previousStatus = status;
                Log("  status: " + SteamUploader.StatusName(status));
            }

            if (total > 0)
            {
                prgProgress.Style = ProgressBarStyle.Blocks;
                prgProgress.Value = (int)Math.Min(1000UL, done * 1000 / total);
                SetStatus(SteamUploader.StatusName(status) + "   "
                      + WorkshopItem.FormatSize((long)done) + " / " + WorkshopItem.FormatSize((long)total),
                      Color.DarkGoldenrod);
            }
        }

        private void Scan(string fileToSelect)
        {
            if (!ResolveDirty()) return;

            List<string> errors = new List<string>();
            _items = WorkshopItem.Scan(_rootFolder, errors);

            _loadingFields = true;
            lstMods.Items.Clear();
            foreach (WorkshopItem i in _items) lstMods.Items.Add(i);
            _loadingFields = false;

            foreach (string h in errors) Log("ERROR: " + h);

            if (_items.Count == 0)
            {
                Log("No *.workshop.json files found in this folder: " + _rootFolder);
                Log("  -> Create one with 'New mod...' or choose the correct folder with 'Change...'.");
                SelectItem(null);
                return;
            }

            int indeks = 0;
            if (!string.IsNullOrEmpty(fileToSelect))
            {
                int b = _items.FindIndex(i => string.Equals(i.FilePath, fileToSelect, StringComparison.OrdinalIgnoreCase));
                if (b >= 0) indeks = b;
            }

            lstMods.SelectedIndex = indeks;
        }

        private void lstMods_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingFields) return;

            if (_dirty && _selectedItem != null && !ResolveDirty())
            {

                _loadingFields = true;
                lstMods.SelectedItem = _selectedItem;
                _loadingFields = false;
                return;
            }

            SelectItem(lstMods.SelectedItem as WorkshopItem);
        }

        private void SelectItem(WorkshopItem item)
        {
            _selectedItem = item;
            _loadingFields = true;

            _languageIndex = 0;
            RefreshLanguages(item);

            txtItemId.Text = item == null ? "" : (item.ItemId == 0 ? "" : item.ItemId.ToString());
            WriteLanguageToScreen(item);
            txtTags.Text = item == null ? "" : string.Join(", ", item.Tags.ToArray());
            cmbVisibility.SelectedIndex = item == null ? 0 : Math.Max(0, Math.Min(3, item.Visibility));
            txtContent.Text = item == null ? "" : item.ContentFolder;
            txtPreview.Text = item == null ? "" : item.PreviewFile;
            txtMetadata.Text = item == null ? "" : item.Metadata;
            txtChangeNote.Text = item == null ? "" : item.ChangeNote;

            _loadingFields = false;
            _dirty = false;

            ShowPreview();
            RefreshButtons();
            RefreshTitle();

            if (item == null) return;

            Log("");
            Log("== " + item.Name + " ==  (" + Path.GetFileName(item.FilePath) + ")");
            Check(false);
        }

        private void RefreshButtons()
        {
            bool item = _selectedItem != null;
            bool bos = !_steam.IsRunning;

            btnSave.Enabled = item && bos;
            btnCheck.Enabled = item;
            btnUpload.Enabled = item && bos && _steam.Connected;
            btnCreateItem.Enabled = item && bos && _steam.Connected;
            btnOpenInSteam.Enabled = item;
            btnListItems.Enabled = bos && _steam.Connected;
            lstMods.Enabled = bos;
            btnNewMod.Enabled = bos;
            btnRefresh.Enabled = bos;
            btnChangeFolder.Enabled = bos;
        }

        private void Field_Changed(object sender, EventArgs e)
        {
            if (_loadingFields) return;
            _dirty = true;
            RefreshTitle();
        }

        private void RefreshTitle()
        {
            Text = "Workshop Uploader"
                 + (_selectedItem == null ? "" : "  -  " + _selectedItem.Name + (_dirty ? " *" : ""))
                 + (_languageIndex == 0 ? "" : "   [" + SelectedLanguageCode + "]");
        }

        private void CollectFields(WorkshopItem item)
        {
            ulong id;
            ulong.TryParse(txtItemId.Text.Trim(), out id);
            item.ItemId = id;

            WriteLanguageToModel(item);

            item.Metadata = txtMetadata.Text.Trim();
            item.ContentFolder = txtContent.Text.Trim();
            item.PreviewFile = txtPreview.Text.Trim();
            item.Visibility = Math.Max(0, cmbVisibility.SelectedIndex);
            item.ChangeNote = txtChangeNote.Text.Trim();

            item.Tags = new List<string>();
            foreach (string part in txtTags.Text.Split(','))
            {
                string tag = part.Trim();
                if (tag.Length > 0) item.Tags.Add(tag);
            }
        }

        private string SelectedLanguageCode
        {
            get
            {
                return _languageIndex >= 0 && _languageIndex < _languageCodes.Count ? _languageCodes[_languageIndex] : WorkshopItem.PrimaryLanguage;
            }
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingFields) return;

            int newValue = Math.Max(0, cmbLanguage.SelectedIndex);
            if (newValue == _languageIndex) return;

            if (newValue >= _languageCodes.Count)
            {
                AddLanguage();
                return;
            }

            if (_selectedItem != null) WriteLanguageToModel(_selectedItem);
            _languageIndex = newValue;

            _loadingFields = true;
            WriteLanguageToScreen(_selectedItem);
            _loadingFields = false;

            RefreshTitle();
        }

        private void RefreshLanguages(WorkshopItem item)
        {
            bool previousLoadingFields = _loadingFields;
            _loadingFields = true;

            _languageCodes = item == null ? new List<string>(WorkshopItem.DefaultLanguages) : item.LanguageOrder();

            cmbLanguage.Items.Clear();
            foreach (string code in _languageCodes) cmbLanguage.Items.Add(WorkshopItem.LanguageLabel(code));
            cmbLanguage.Items.Add("+ Add language...");

            if (_languageIndex >= _languageCodes.Count) _languageIndex = 0;
            cmbLanguage.SelectedIndex = _languageIndex;

            _loadingFields = previousLoadingFields;
        }

        private void AddLanguage()
        {
            if (_selectedItem == null) { RefreshLanguages(null); return; }

            WriteLanguageToModel(_selectedItem);

            string code = null;
            using (LanguageSelectDialog d = new LanguageSelectDialog(_languageCodes))
            {
                if (d.ShowDialog(this) == DialogResult.OK) code = d.Kod;
            }

            if (string.IsNullOrEmpty(code))
            {
                RefreshLanguages(_selectedItem);
                return;
            }

            _selectedItem.FindLanguage(code, true);
            RefreshLanguages(_selectedItem);

            int index = _languageCodes.FindIndex(k => string.Equals(k, code, StringComparison.OrdinalIgnoreCase));
            _languageIndex = index < 0 ? 0 : index;

            _loadingFields = true;
            cmbLanguage.SelectedIndex = _languageIndex;
            WriteLanguageToScreen(_selectedItem);
            _loadingFields = false;

            Log("Language added: " + WorkshopItem.LanguageLabel(code)
                + "  (it will be removed on save if you leave it empty)");
            RefreshTitle();
        }

        private void WriteLanguageToModel(WorkshopItem item)
        {
            if (item == null) return;

            WorkshopItem.Localization y = item.FindLanguage(SelectedLanguageCode, true);
            y.Title = txtTitle.Text.Trim();
            y.Description = txtDescription.Text;
        }

        private void WriteLanguageToScreen(WorkshopItem item)
        {
            if (item == null)
            {
                txtTitle.Text = "";
                txtDescription.Text = "";
                return;
            }

            WorkshopItem.Localization y = item.FindLanguage(SelectedLanguageCode, false);
            txtTitle.Text = y == null ? "" : y.Title;
            txtDescription.Text = y == null ? "" : y.Description;
        }

        private bool Save(bool verbose)
        {
            if (_selectedItem == null) return false;

            CollectFields(_selectedItem);
            try
            {
                _selectedItem.Save();
            }
            catch (Exception e)
            {
                Log("Could not save: " + e.Message);
                if (verbose) MessageBox.Show(this, e.Message, "Could not save", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _dirty = false;
            RefreshTitle();
            if (verbose) Log("Saved: " + Path.GetFileName(_selectedItem.FilePath));
            return true;
        }

        private bool ResolveDirty()
        {
            if (!_dirty || _selectedItem == null) return true;

            DialogResult c = MessageBox.Show(this,
                _selectedItem.Name + " has unsaved changes. Save them?",
                "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (c == DialogResult.Cancel) return false;
            if (c == DialogResult.No) { _dirty = false; return true; }
            return Save(true);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            Check(true);
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Upload(false);
        }

        private void btnCreateItem_Click(object sender, EventArgs e)
        {
            Upload(true);
        }

        private void btnListItems_Click(object sender, EventArgs e)
        {
            Log("");
            _steam.ListItems();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Scan(_selectedItem == null ? null : _selectedItem.FilePath);
        }

        private void txtItemId_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void txtPreview_TextChanged(object sender, EventArgs e)
        {
            Field_Changed(sender, e);
            ShowPreview();
        }

        private void btnBrowseContent_Click(object sender, EventArgs e)
        {
            SelectFolder(txtContent);
        }

        private void btnBrowsePreview_Click(object sender, EventArgs e)
        {
            SelectFile(txtPreview);
        }

        private void btnOpenInSteam_Click(object sender, EventArgs e)
        {
            ulong id;
            if (!ulong.TryParse(txtItemId.Text.Trim(), out id) || id == 0)
            {
                Log("No item ID - upload first or create a new item.");
                return;
            }

            try { Process.Start("https://steamcommunity.com/sharedfiles/filedetails/?id=" + id); }
            catch (Exception error) { Log("Could not open browser: " + error.Message); }
        }

        private void btnNewMod_Click(object sender, EventArgs e)
        {
            if (!ResolveDirty()) return;

            string name;
            using (InputDialog d = new InputDialog("New mod file", "Mod name (using the folder name is easiest):", ""))
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                name = (d.Value ?? "").Trim();
            }

            if (name.Length == 0) return;

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show(this, "The file name contains an invalid character.", "Invalid name",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                WorkshopItem newValue = WorkshopItem.CreateNew(_rootFolder, name);
                Log("Created: " + Path.GetFileName(newValue.FilePath));
                Scan(newValue.FilePath);
            }
            catch (Exception error)
            {
                MessageBox.Show(this, error.Message, "Could not create", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnChangeFolder_Click(object sender, EventArgs e)
        {
            if (!ResolveDirty()) return;

            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                d.Description = "Select the folder that contains *.workshop.json files";
                if (Directory.Exists(_rootFolder)) d.SelectedPath = _rootFolder;
                if (d.ShowDialog(this) != DialogResult.OK) return;

                _rootFolder = d.SelectedPath;
                txtRootFolder.Text = _rootFolder;
                Workspace.Log(_rootFolder);
                Scan(null);
            }
        }

        private void Check(bool verbose)
        {
            if (_selectedItem == null) return;

            CollectFields(_selectedItem);

            foreach (string u in _selectedItem.Warnings()) Log("  " + u);

            List<string> blockers = _selectedItem.Blockers();
            foreach (string en in blockers) Log("  BLOCKED: " + en);

            if (blockers.Count == 0 && verbose) Log("  No blockers - ready to upload.");
        }

        private void Upload(bool createNewItem)
        {
            if (_selectedItem == null || _steam.IsRunning) return;

            if (createNewItem && _selectedItem.ItemId != 0)
            {
                DialogResult c = MessageBox.Show(this,
                    "This item already has an ID (" + _selectedItem.ItemId + ").\n\n" +
                    "Creating a new item will create a SECOND Workshop entry and replace the ID in this file. " +
                    "You will no longer reach the old entry from this tool.\n\nContinue?",
                    "Create new item", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (c != DialogResult.Yes) return;
            }

            if (!Save(false)) return;

            _previousStatus = EItemUpdateStatus.k_EItemUpdateStatusInvalid;
            prgProgress.Style = ProgressBarStyle.Marquee;
            prgProgress.Value = 0;
            SetStatus("Uploading...", Color.DarkGoldenrod);

            Log("");
            Log(createNewItem ? "-- new item --" : "-- upload --");

            _steam.Upload(_selectedItem, createNewItem);
            RefreshButtons();
        }

        private void SteamFinished(bool successful, string message)
        {
            prgProgress.Style = ProgressBarStyle.Blocks;
            prgProgress.Value = successful ? prgProgress.Maximum : 0;
            SetStatus(message, successful ? Color.SeaGreen : Color.Firebrick);

            if (_selectedItem != null)
            {
                _loadingFields = true;
                txtItemId.Text = _selectedItem.ItemId == 0 ? "" : _selectedItem.ItemId.ToString();
                _loadingFields = false;
            }

            RefreshButtons();
        }

        private void ShowPreview()
        {

            Image old = picPreview.Image;
            picPreview.Image = null;
            if (old != null) old.Dispose();

            lblPreviewInfo.Text = "";
            lblPreviewInfo.ForeColor = SystemColors.ControlText;

            if (_selectedItem == null || string.IsNullOrEmpty(txtPreview.Text.Trim())) return;

            string path = Path.IsPathRooted(txtPreview.Text.Trim())
                ? txtPreview.Text.Trim()
                : Path.Combine(_rootFolder, txtPreview.Text.Trim());

            if (!File.Exists(path))
            {
                lblPreviewInfo.Text = "File not found:" + Environment.NewLine + path;
                lblPreviewInfo.ForeColor = Color.Firebrick;
                return;
            }

            long size = new FileInfo(path).Length;
            try
            {

                using (FileStream akis = File.OpenRead(path))
                using (Image raw = Image.FromStream(akis))
                    picPreview.Image = new Bitmap(raw);

                lblPreviewInfo.Text = picPreview.Image.Width + " x " + picPreview.Image.Height
                                      + "   " + WorkshopItem.FormatSize(size);
            }
            catch (Exception e)
            {
                lblPreviewInfo.Text = "Could not read image: " + e.Message;
                lblPreviewInfo.ForeColor = Color.Firebrick;
                return;
            }

            if (size > WorkshopItem.PreviewLimit)
            {
                lblPreviewInfo.Text += Environment.NewLine + "1 MB limit exceeded - Steam will reject it.";
                lblPreviewInfo.ForeColor = Color.Firebrick;
            }
        }

        private void SelectFolder(TextBox box)
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog())
            {
                string simdiki = Path.IsPathRooted(box.Text) ? box.Text : Path.Combine(_rootFolder, box.Text ?? "");
                if (Directory.Exists(simdiki)) d.SelectedPath = simdiki;
                else if (Directory.Exists(_rootFolder)) d.SelectedPath = _rootFolder;

                if (d.ShowDialog(this) == DialogResult.OK) box.Text = MakeRelative(d.SelectedPath);
            }
        }

        private void SelectFile(TextBox box)
        {
            using (OpenFileDialog d = new OpenFileDialog())
            {
                d.Filter = "Images|*.png;*.jpg;*.jpeg;*.gif|All files|*.*";
                string simdiki = Path.IsPathRooted(box.Text) ? box.Text : Path.Combine(_rootFolder, box.Text ?? "");
                if (File.Exists(simdiki)) d.InitialDirectory = Path.GetDirectoryName(simdiki);
                else if (Directory.Exists(_rootFolder)) d.InitialDirectory = _rootFolder;

                if (d.ShowDialog(this) == DialogResult.OK) box.Text = MakeRelative(d.FileName);
            }
        }

        private string MakeRelative(string fullPath)
        {
            string rootFolder = _rootFolder;
            if (string.IsNullOrEmpty(rootFolder)) return fullPath;
            if (!rootFolder.EndsWith("\\", StringComparison.Ordinal)) rootFolder += "\\";

            return fullPath.StartsWith(rootFolder, StringComparison.OrdinalIgnoreCase)
                ? fullPath.Substring(rootFolder.Length)
                : fullPath;
        }

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        private void Log(string line)
        {
            txtLog.AppendText((line.Length == 0 ? "" : DateTime.Now.ToString("HH:mm:ss") + "  ")
                                 + line + Environment.NewLine);
        }
    }
}
