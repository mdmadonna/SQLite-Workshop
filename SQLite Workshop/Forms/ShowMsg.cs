using System;
using System.Drawing;
using System.Windows.Forms;

using static SQLiteWorkshop.Common;
using static SQLiteWorkshop.GUIManager;

namespace SQLiteWorkshop
{
    public partial class ShowMsg : Form
    {
        ToolTip toolTip;

        public enum ButtonStyle
        {
            OKCancel,
            OK
        }

        public ButtonStyle ButtonStyleValue { get; set; }
        public string Caption { get; set; }

        public string Message { get; set; }

        public DialogResult Result { get; set; }

        public bool DoNotShow { get; set; }

        public ShowMsg(ButtonStyle buttonStyle = ButtonStyle.OKCancel)
        {
            ButtonStyleValue = buttonStyle;
            InitializeComponent();
        }

        private void ShowMsg_Load(object sender, EventArgs e)
        {
            textBoxMessage.Text = Message;
            Result = DialogResult.Cancel;       //Set default even if Cancel Button is not displayed
            toolTip = new ToolTip();
            HouseKeeping(this, string.IsNullOrEmpty(Caption) ? "Warning" : Caption);

            switch (ButtonStyleValue)
            {
                case ButtonStyle.OKCancel:          //Default - do nothing
                    break;
                case ButtonStyle.OK:                //Hide Cancel button and move OK button
                    btnCancel.Visible = false;
                    btnOK.Left = btnCancel.Left;
                    break;
                default:
                    break;
            }
        }

        private void ShowMsg_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClose(this);
        }

        private void ShowMsg_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoNotShow = checkBoxDoNot.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            this.Close();
        }

    }
}
