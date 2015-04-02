using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UPnpSpy
{
    public partial class OptionsDialog : Form
    {
        public string UserAgent
        {
            get
            {
                return comboBoxUserAgent.Text;
            }
            set
            {
                comboBoxUserAgent.Text = value;
            }
        }

        public OptionsDialog()
        {
            InitializeComponent();
        }

        private void OptionsDialog_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.UserAgentHistory != null)
            {
                this.comboBoxUserAgent.Items.Clear();
                foreach (var item in Properties.Settings.Default.UserAgentHistory)
                {
                    this.comboBoxUserAgent.Items.Add(item);
                }
            }
        }

        private void buttonSet_Click(object sender, EventArgs e)
        {
            bool bSettingsChanged = false;

            if (!comboBoxUserAgent.Items.Contains(comboBoxUserAgent.Text))
            {
                comboBoxUserAgent.Items.Insert(0, comboBoxUserAgent.Text);
                StringCollection items = new StringCollection();
                foreach (var item in comboBoxUserAgent.Items)
                {
                    items.Add(item.ToString());
                }
                Properties.Settings.Default.UserAgentHistory = items;
                bSettingsChanged = true;
            }

            if (bSettingsChanged)
            {
                Properties.Settings.Default.Save();
            }
        }
    }
}
