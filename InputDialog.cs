using System.Windows.Forms;

namespace SteamWorkshopUploader
{

    public partial class InputDialog : Form
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        public InputDialog(string title, string question, string defaultValue)
            : this()
        {
            Text = title;
            lblQuestion.Text = question;
            txtValue.Text = defaultValue ?? "";
            txtValue.SelectAll();
        }

        public string Value
        {
            get { return txtValue.Text; }
        }
    }
}
