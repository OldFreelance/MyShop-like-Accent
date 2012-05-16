using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyShop
{
    public partial class DocumentForm : Form
    {
        MyShopEntities db = new MyShopEntities();

        public Document value;

        public DocumentForm()
        {
            InitializeComponent();
        }

        private void DocumentForm_Load(object sender, EventArgs e)
        {
            BindingSource bsFrom=  new BindingSource();
            bsFrom.DataSource = db.Agents;
            cmbAgentFrom.DataSource = bsFrom;
            cmbAgentFrom.DisplayMember = "Name";
            cmbAgentFrom.ValueMember = "Id";
            cmbAgentFrom.SelectedValue = value.AgentFromId;

            BindingSource bsTo = new BindingSource();
            bsTo.DataSource = db.Agents;
            cmbAgentTo.DataSource = bsTo;
            cmbAgentTo.DisplayMember = "Name";
            cmbAgentTo.ValueMember = "Id";
            cmbAgentTo.SelectedValue = value.AgentToId;

            txtName.Text = value.Name;
            dateTimePicker1.Value = value.Date;
            txtNumber.Text = value.Number;
            txtSumma.Text = value.Summa.ToString();
            dataGridViewDetails.DataSource = value.DocumentDetails;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            value.Name = txtName.Text;
            value.Date = dateTimePicker1.Value;
            value.Number = txtNumber.Text;
            value.AgentFromId = value.AgentFromId;
            value.AgentToId = value.AgentToId;
            value.Summa = decimal.Parse(txtSumma.Text);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
