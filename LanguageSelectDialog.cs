using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SteamWorkshopUploader
{

    internal sealed class LanguageSelectDialog : Form
    {
        private readonly ListBox _liste = new ListBox();
        private readonly List<string> _kodlar = new List<string>();

        public string Kod
        {
            get
            {
                int i = _liste.SelectedIndex;
                return i < 0 || i >= _kodlar.Count ? null : _kodlar[i];
            }
        }

        public LanguageSelectDialog(IEnumerable<string> existing)
        {
            List<string> alreadyAdded = new List<string>(existing ?? new string[0]);

            Text = "Add Language";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(320, 360);

            Label info = new Label
            {
                Text = "Select a language to add:",
                Location = new Point(12, 12),
                Size = new Size(296, 20)
            };

            _liste.Location = new Point(12, 36);
            _liste.Size = new Size(296, 268);
            _liste.IntegralHeight = false;

            foreach (string[] d in WorkshopItem.AllLanguages)
            {
                if (alreadyAdded.Exists(v => string.Equals(v, d[0], StringComparison.OrdinalIgnoreCase))) continue;
                _kodlar.Add(d[0]);
                _liste.Items.Add(d[1] + "   (" + d[0] + ")");
            }

            if (_liste.Items.Count > 0) _liste.SelectedIndex = 0;

            Button ok = new Button
            {
                Text = "Add",
                DialogResult = DialogResult.OK,
                Location = new Point(140, 316),
                Size = new Size(80, 28)
            };

            Button cancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(228, 316),
                Size = new Size(80, 28)
            };

            _liste.DoubleClick += delegate
            {
                if (_liste.SelectedIndex >= 0) { DialogResult = DialogResult.OK; Close(); }
            };

            Controls.Add(info);
            Controls.Add(_liste);
            Controls.Add(ok);
            Controls.Add(cancel);

            AcceptButton = ok;
            CancelButton = cancel;

            ok.Enabled = _liste.Items.Count > 0;
        }
    }
}
