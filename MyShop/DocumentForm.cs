using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MyShop
{
    public partial class DocumentForm : Form
    {
        private MyShopEntities db = new MyShopEntities();

        public Document value;

        public DocumentForm()
        {
            InitializeComponent();
        }

        private void DocumentForm_Load(object sender, EventArgs e)
        {
            //Подготовка данных для списка "От кого"
            BindingSource bsFrom = new BindingSource();
            bsFrom.DataSource = db.Agents;
            cmbAgentFrom.DataSource = bsFrom;
            cmbAgentFrom.DisplayMember = "Name";
            cmbAgentFrom.ValueMember = "Id";
            cmbAgentFrom.SelectedValue = value.AgentFromId;

            //Подготовка данных для списка "Кому"
            BindingSource bsTo = new BindingSource();
            bsTo.DataSource = db.Agents;
            cmbAgentTo.DataSource = bsTo;
            cmbAgentTo.DisplayMember = "Name";
            cmbAgentTo.ValueMember = "Id";
            cmbAgentTo.SelectedValue = value.AgentToId;

            //Заполение остальных котролов формы
            txtName.Text = value.Name;
            dateTimePicker1.Value = value.Date;
            txtNumber.Text = value.Number;
            txtSumma.Text = value.Summa.ToString();


            //Создание колонок для грида детализации
            dataGridViewDetails.Columns.Add(new DataGridViewComboBoxColumn
                                                {
                                                    DataPropertyName = "ProductId",
                                                    Name = "Товар",
                                                    ValueMember = "Id",
                                                    DisplayMember = "Name",
                                                    DataSource = db.Products
                                                });
            dataGridViewDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Qty", Name = "Количество" });
            dataGridViewDetails.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Id", Name = "Id", Visible = false});

            dataGridViewDetails.AutoGenerateColumns = false;
            dataGridViewDetails.DataSource = value.DocumentDetails;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            //Заполнение полей объекта из значений контролов
            value.Name = txtName.Text;
            value.Date = dateTimePicker1.Value;
            value.Number = txtNumber.Text;
            value.AgentFromId = (int)cmbAgentFrom.SelectedValue;
            value.AgentToId = (int)cmbAgentTo.SelectedValue;
            value.Summa = decimal.Parse(txtSumma.Text);

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void dataGridViewDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnDeleteDetail_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewDetails.SelectedRows)
            {
                //int id = (int)row.Cells["Id"].Value;
                //var detail = db.DocumentDetails.FirstOrDefault(s => s.Id == id);
                dataGridViewDetails.Rows.Remove(row);
                //db.DocumentDetails.DeleteObject(detail);
            }
        }
    }
}
    