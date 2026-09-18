namespace SteamWorkshopUploader
{

    partial class MainWindow
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblRootFolder;
        private System.Windows.Forms.TextBox txtRootFolder;
        private System.Windows.Forms.Button btnChangeFolder;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Label lblMods;
        private System.Windows.Forms.ListBox lstMods;
        private System.Windows.Forms.Panel panelLeftBottom;
        private System.Windows.Forms.Button btnNewMod;

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.TextBox txtItemId;
        private System.Windows.Forms.Button btnOpenInSteam;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label lblTags;
        private System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.Label lblVisibility;
        private System.Windows.Forms.ComboBox cmbVisibility;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Button btnBrowseContent;
        private System.Windows.Forms.Label lblPreview;
        private System.Windows.Forms.TextBox txtPreview;
        private System.Windows.Forms.Button btnBrowsePreview;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Label lblPreviewInfo;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblMetadata;
        private System.Windows.Forms.TextBox txtMetadata;
        private System.Windows.Forms.Label lblChangeNote;
        private System.Windows.Forms.TextBox txtChangeNote;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnCreateItem;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Button btnListItems;

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.ProgressBar prgProgress;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtLog;

        private System.Windows.Forms.Timer timerHeartbeat;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblRootFolder = new System.Windows.Forms.Label();
            this.txtRootFolder = new System.Windows.Forms.TextBox();
            this.btnChangeFolder = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.lstMods = new System.Windows.Forms.ListBox();
            this.panelLeftBottom = new System.Windows.Forms.Panel();
            this.btnNewMod = new System.Windows.Forms.Button();
            this.lblMods = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelFields = new System.Windows.Forms.Panel();
            this.lblItemId = new System.Windows.Forms.Label();
            this.txtItemId = new System.Windows.Forms.TextBox();
            this.btnOpenInSteam = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.lblTags = new System.Windows.Forms.Label();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.lblVisibility = new System.Windows.Forms.Label();
            this.cmbVisibility = new System.Windows.Forms.ComboBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.btnBrowseContent = new System.Windows.Forms.Button();
            this.lblPreview = new System.Windows.Forms.Label();
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.btnBrowsePreview = new System.Windows.Forms.Button();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.lblPreviewInfo = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblMetadata = new System.Windows.Forms.Label();
            this.txtMetadata = new System.Windows.Forms.TextBox();
            this.lblChangeNote = new System.Windows.Forms.Label();
            this.txtChangeNote = new System.Windows.Forms.TextBox();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnCreateItem = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.btnListItems = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.prgProgress = new System.Windows.Forms.ProgressBar();
            this.timerHeartbeat = new System.Windows.Forms.Timer(this.components);
            this.panelTop.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelLeftBottom.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();

            this.panelTop.Controls.Add(this.lblRootFolder);
            this.panelTop.Controls.Add(this.txtRootFolder);
            this.panelTop.Controls.Add(this.btnChangeFolder);
            this.panelTop.Controls.Add(this.btnRefresh);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1004, 44);
            this.panelTop.TabIndex = 0;

            this.lblRootFolder.Location = new System.Drawing.Point(12, 14);
            this.lblRootFolder.Name = "lblRootFolder";
            this.lblRootFolder.Size = new System.Drawing.Size(128, 20);
            this.lblRootFolder.TabIndex = 0;
            this.lblRootFolder.Text = "ModPack folder";

            this.txtRootFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRootFolder.BackColor = System.Drawing.SystemColors.Window;
            this.txtRootFolder.Location = new System.Drawing.Point(140, 11);
            this.txtRootFolder.Name = "txtRootFolder";
            this.txtRootFolder.ReadOnly = true;
            this.txtRootFolder.Size = new System.Drawing.Size(670, 23);
            this.txtRootFolder.TabIndex = 1;

            this.btnChangeFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeFolder.Location = new System.Drawing.Point(816, 9);
            this.btnChangeFolder.Name = "btnChangeFolder";
            this.btnChangeFolder.Size = new System.Drawing.Size(90, 26);
            this.btnChangeFolder.TabIndex = 2;
            this.btnChangeFolder.Text = "Change...";
            this.btnChangeFolder.UseVisualStyleBackColor = true;
            this.btnChangeFolder.Click += new System.EventHandler(this.btnChangeFolder_Click);

            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(912, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 26);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.panelLeft.Controls.Add(this.lstMods);
            this.panelLeft.Controls.Add(this.panelLeftBottom);
            this.panelLeft.Controls.Add(this.lblMods);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 44);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(12, 8, 6, 8);
            this.panelLeft.Size = new System.Drawing.Size(236, 505);
            this.panelLeft.TabIndex = 1;

            this.lstMods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMods.FormattingEnabled = true;
            this.lstMods.IntegralHeight = false;
            this.lstMods.ItemHeight = 15;
            this.lstMods.Location = new System.Drawing.Point(12, 28);
            this.lstMods.Name = "lstMods";
            this.lstMods.Size = new System.Drawing.Size(218, 425);
            this.lstMods.TabIndex = 1;
            this.lstMods.SelectedIndexChanged += new System.EventHandler(this.lstMods_SelectedIndexChanged);

            this.panelLeftBottom.Controls.Add(this.btnNewMod);
            this.panelLeftBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelLeftBottom.Location = new System.Drawing.Point(12, 453);
            this.panelLeftBottom.Name = "panelLeftBottom";
            this.panelLeftBottom.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.panelLeftBottom.Size = new System.Drawing.Size(218, 44);
            this.panelLeftBottom.TabIndex = 2;

            this.btnNewMod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNewMod.Location = new System.Drawing.Point(0, 8);
            this.btnNewMod.Name = "btnNewMod";
            this.btnNewMod.Size = new System.Drawing.Size(218, 36);
            this.btnNewMod.TabIndex = 0;
            this.btnNewMod.Text = "New mod...";
            this.btnNewMod.UseVisualStyleBackColor = true;
            this.btnNewMod.Click += new System.EventHandler(this.btnNewMod_Click);

            this.lblMods.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMods.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMods.Location = new System.Drawing.Point(12, 8);
            this.lblMods.Name = "lblMods";
            this.lblMods.Size = new System.Drawing.Size(218, 20);
            this.lblMods.TabIndex = 0;
            this.lblMods.Text = "Mods";

            this.panelMain.Controls.Add(this.panelFields);
            this.panelMain.Controls.Add(this.panelButtons);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(236, 44);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(768, 505);
            this.panelMain.TabIndex = 2;

            this.panelFields.AutoScroll = true;
            this.panelFields.Controls.Add(this.lblItemId);
            this.panelFields.Controls.Add(this.txtItemId);
            this.panelFields.Controls.Add(this.btnOpenInSteam);
            this.panelFields.Controls.Add(this.lblTitle);
            this.panelFields.Controls.Add(this.txtTitle);
            this.panelFields.Controls.Add(this.cmbLanguage);
            this.panelFields.Controls.Add(this.lblTags);
            this.panelFields.Controls.Add(this.txtTags);
            this.panelFields.Controls.Add(this.lblVisibility);
            this.panelFields.Controls.Add(this.cmbVisibility);
            this.panelFields.Controls.Add(this.lblContent);
            this.panelFields.Controls.Add(this.txtContent);
            this.panelFields.Controls.Add(this.btnBrowseContent);
            this.panelFields.Controls.Add(this.lblPreview);
            this.panelFields.Controls.Add(this.txtPreview);
            this.panelFields.Controls.Add(this.btnBrowsePreview);
            this.panelFields.Controls.Add(this.picPreview);
            this.panelFields.Controls.Add(this.lblPreviewInfo);
            this.panelFields.Controls.Add(this.lblDescription);
            this.panelFields.Controls.Add(this.txtDescription);
            this.panelFields.Controls.Add(this.lblMetadata);
            this.panelFields.Controls.Add(this.txtMetadata);
            this.panelFields.Controls.Add(this.lblChangeNote);
            this.panelFields.Controls.Add(this.txtChangeNote);
            this.panelFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFields.Location = new System.Drawing.Point(0, 0);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(768, 453);
            this.panelFields.TabIndex = 0;

            this.lblItemId.Location = new System.Drawing.Point(12, 12);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(128, 20);
            this.lblItemId.TabIndex = 0;
            this.lblItemId.Text = "WorkshopID";

            this.txtItemId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemId.Location = new System.Drawing.Point(140, 9);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Size = new System.Drawing.Size(494, 23);
            this.txtItemId.TabIndex = 1;
            this.txtItemId.TextChanged += new System.EventHandler(this.Field_Changed);
            this.txtItemId.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtItemId_KeyPress);

            this.btnOpenInSteam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenInSteam.Location = new System.Drawing.Point(640, 8);
            this.btnOpenInSteam.Name = "btnOpenInSteam";
            this.btnOpenInSteam.Size = new System.Drawing.Size(116, 24);
            this.btnOpenInSteam.TabIndex = 2;
            this.btnOpenInSteam.Text = "Open in Steam";
            this.btnOpenInSteam.UseVisualStyleBackColor = true;
            this.btnOpenInSteam.Click += new System.EventHandler(this.btnOpenInSteam_Click);

            this.lblTitle.Location = new System.Drawing.Point(12, 43);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(128, 20);
            this.lblTitle.TabIndex = 3;
            this.lblTitle.Text = "Title";

            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTitle.Location = new System.Drawing.Point(140, 40);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(476, 23);
            this.txtTitle.TabIndex = 4;
            this.txtTitle.TextChanged += new System.EventHandler(this.Field_Changed);

            this.cmbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLanguage.FormattingEnabled = true;

            this.cmbLanguage.Location = new System.Drawing.Point(624, 40);
            this.cmbLanguage.Name = "cmbLanguage";
            this.cmbLanguage.Size = new System.Drawing.Size(132, 23);
            this.cmbLanguage.TabIndex = 5;
            this.cmbLanguage.SelectedIndexChanged += new System.EventHandler(this.cmbLanguage_SelectedIndexChanged);

            this.lblTags.Location = new System.Drawing.Point(12, 74);
            this.lblTags.Name = "lblTags";
            this.lblTags.Size = new System.Drawing.Size(128, 20);
            this.lblTags.TabIndex = 5;
            this.lblTags.Text = "Tags (comma separated)";

            this.txtTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTags.Location = new System.Drawing.Point(140, 71);
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(616, 23);
            this.txtTags.TabIndex = 6;
            this.txtTags.TextChanged += new System.EventHandler(this.Field_Changed);

            this.lblVisibility.Location = new System.Drawing.Point(12, 105);
            this.lblVisibility.Name = "lblVisibility";
            this.lblVisibility.Size = new System.Drawing.Size(128, 20);
            this.lblVisibility.TabIndex = 7;
            this.lblVisibility.Text = "Visibility";

            this.cmbVisibility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVisibility.FormattingEnabled = true;
            this.cmbVisibility.Items.AddRange(new object[] {
            "0 - Public",
            "1 - Friends only",
            "2 - Private",
            "3 - Unlisted (open by link)"});
            this.cmbVisibility.Location = new System.Drawing.Point(140, 102);
            this.cmbVisibility.Name = "cmbVisibility";
            this.cmbVisibility.Size = new System.Drawing.Size(300, 23);
            this.cmbVisibility.TabIndex = 8;
            this.cmbVisibility.SelectedIndexChanged += new System.EventHandler(this.Field_Changed);

            this.lblContent.Location = new System.Drawing.Point(12, 136);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(128, 20);
            this.lblContent.TabIndex = 9;
            this.lblContent.Text = "Content folder";

            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.Location = new System.Drawing.Point(140, 133);
            this.txtContent.Name = "txtContent";
            this.txtContent.Size = new System.Drawing.Size(526, 23);
            this.txtContent.TabIndex = 10;
            this.txtContent.TextChanged += new System.EventHandler(this.Field_Changed);

            this.btnBrowseContent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseContent.Location = new System.Drawing.Point(672, 132);
            this.btnBrowseContent.Name = "btnBrowseContent";
            this.btnBrowseContent.Size = new System.Drawing.Size(84, 24);
            this.btnBrowseContent.TabIndex = 11;
            this.btnBrowseContent.Text = "Browse";
            this.btnBrowseContent.UseVisualStyleBackColor = true;
            this.btnBrowseContent.Click += new System.EventHandler(this.btnBrowseContent_Click);

            this.lblPreview.Location = new System.Drawing.Point(12, 167);
            this.lblPreview.Name = "lblPreview";
            this.lblPreview.Size = new System.Drawing.Size(128, 20);
            this.lblPreview.TabIndex = 12;
            this.lblPreview.Text = "Preview image";

            this.txtPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPreview.Location = new System.Drawing.Point(140, 164);
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.Size = new System.Drawing.Size(526, 23);
            this.txtPreview.TabIndex = 13;
            this.txtPreview.TextChanged += new System.EventHandler(this.txtPreview_TextChanged);

            this.btnBrowsePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowsePreview.Location = new System.Drawing.Point(672, 163);
            this.btnBrowsePreview.Name = "btnBrowsePreview";
            this.btnBrowsePreview.Size = new System.Drawing.Size(84, 24);
            this.btnBrowsePreview.TabIndex = 14;
            this.btnBrowsePreview.Text = "Browse";
            this.btnBrowsePreview.UseVisualStyleBackColor = true;
            this.btnBrowsePreview.Click += new System.EventHandler(this.btnBrowsePreview_Click);

            this.picPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Location = new System.Drawing.Point(140, 195);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(176, 99);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picPreview.TabIndex = 15;
            this.picPreview.TabStop = false;

            this.lblPreviewInfo.Location = new System.Drawing.Point(326, 199);
            this.lblPreviewInfo.Name = "lblPreviewInfo";
            this.lblPreviewInfo.Size = new System.Drawing.Size(430, 60);
            this.lblPreviewInfo.TabIndex = 16;

            this.lblDescription.Location = new System.Drawing.Point(12, 305);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(128, 20);
            this.lblDescription.TabIndex = 17;
            this.lblDescription.Text = "Description";

            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(140, 302);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(616, 80);
            this.txtDescription.TabIndex = 18;
            this.txtDescription.TextChanged += new System.EventHandler(this.Field_Changed);

            this.lblMetadata.Location = new System.Drawing.Point(12, 393);
            this.lblMetadata.Name = "lblMetadata";
            this.lblMetadata.Size = new System.Drawing.Size(128, 20);
            this.lblMetadata.TabIndex = 19;
            this.lblMetadata.Text = "Metadata";

            this.txtMetadata.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMetadata.Location = new System.Drawing.Point(140, 390);
            this.txtMetadata.Name = "txtMetadata";
            this.txtMetadata.Size = new System.Drawing.Size(616, 23);
            this.txtMetadata.TabIndex = 20;
            this.txtMetadata.TextChanged += new System.EventHandler(this.Field_Changed);

            this.lblChangeNote.Location = new System.Drawing.Point(12, 424);
            this.lblChangeNote.Name = "lblChangeNote";
            this.lblChangeNote.Size = new System.Drawing.Size(128, 20);
            this.lblChangeNote.TabIndex = 21;
            this.lblChangeNote.Text = "Change note";

            this.txtChangeNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtChangeNote.Location = new System.Drawing.Point(140, 421);
            this.txtChangeNote.Name = "txtChangeNote";
            this.txtChangeNote.Size = new System.Drawing.Size(616, 23);
            this.txtChangeNote.TabIndex = 22;
            this.txtChangeNote.TextChanged += new System.EventHandler(this.Field_Changed);

            this.panelButtons.Controls.Add(this.btnSave);
            this.panelButtons.Controls.Add(this.btnUpload);
            this.panelButtons.Controls.Add(this.btnCreateItem);
            this.panelButtons.Controls.Add(this.btnCheck);
            this.panelButtons.Controls.Add(this.btnListItems);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelButtons.Location = new System.Drawing.Point(0, 453);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(768, 52);
            this.panelButtons.TabIndex = 1;

            this.btnSave.Location = new System.Drawing.Point(12, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 36);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnUpload.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnUpload.Location = new System.Drawing.Point(150, 8);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(150, 36);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Upload";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);

            this.btnCreateItem.Location = new System.Drawing.Point(308, 8);
            this.btnCreateItem.Name = "btnCreateItem";
            this.btnCreateItem.Size = new System.Drawing.Size(170, 36);
            this.btnCreateItem.TabIndex = 2;
            this.btnCreateItem.Text = "Create Item";
            this.btnCreateItem.UseVisualStyleBackColor = true;
            this.btnCreateItem.Click += new System.EventHandler(this.btnCreateItem_Click);

            this.btnCheck.Location = new System.Drawing.Point(486, 8);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(100, 36);
            this.btnCheck.TabIndex = 3;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);

            this.btnListItems.Location = new System.Drawing.Point(594, 8);
            this.btnListItems.Name = "btnListItems";
            this.btnListItems.Size = new System.Drawing.Size(160, 36);
            this.btnListItems.TabIndex = 4;
            this.btnListItems.Text = "List Workshop IDs";
            this.btnListItems.UseVisualStyleBackColor = true;
            this.btnListItems.Click += new System.EventHandler(this.btnListItems_Click);

            this.panelBottom.Controls.Add(this.txtLog);
            this.panelBottom.Controls.Add(this.lblStatus);
            this.panelBottom.Controls.Add(this.prgProgress);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 549);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Padding = new System.Windows.Forms.Padding(12, 4, 12, 12);
            this.panelBottom.Size = new System.Drawing.Size(1004, 232);
            this.panelBottom.TabIndex = 3;

            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.Gainsboro;
            this.txtLog.Location = new System.Drawing.Point(12, 42);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(980, 178);
            this.txtLog.TabIndex = 2;

            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStatus.Location = new System.Drawing.Point(12, 20);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(980, 22);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Connecting to Steam...";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.prgProgress.Dock = System.Windows.Forms.DockStyle.Top;
            this.prgProgress.Location = new System.Drawing.Point(12, 4);
            this.prgProgress.Maximum = 1000;
            this.prgProgress.Name = "prgProgress";
            this.prgProgress.Size = new System.Drawing.Size(980, 16);
            this.prgProgress.TabIndex = 0;

            this.timerHeartbeat.Tick += new System.EventHandler(this.timerHeartbeat_Tick);

            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1004, 781);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(840, 660);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Workshop Uploader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelLeftBottom.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.panelFields.ResumeLayout(false);
            this.panelFields.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
