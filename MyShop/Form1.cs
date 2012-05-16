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
    public partial class Form1 : Form
    {
        MyShopEntities db = new MyShopEntities();
        BindingSource bsDocumnts = new BindingSource();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridViewDocuments.ColumnCount = 4;
            dataGridViewDocuments.AutoGenerateColumns = false;
            dataGridViewDocuments.Columns[0].Name = "Имя";
            dataGridViewDocuments.Columns[0].DataPropertyName = "Name";
            dataGridViewDocuments.Columns[0].Width = 200;
            dataGridViewDocuments.Columns[1].Name = "Дата";
            dataGridViewDocuments.Columns[1].DataPropertyName = "Date";
            dataGridViewDocuments.Columns[2].Name = "Номер";
            dataGridViewDocuments.Columns[2].DataPropertyName = "Number";
            dataGridViewDocuments.Columns[3].Name = "Id";
            dataGridViewDocuments.Columns[3].DataPropertyName = "Id";
            dataGridViewDocuments.Columns[3].Visible = false;
            bsDocumnts.DataSource = db.Documents;
            dataGridViewDocuments.DataSource = bsDocumnts;

            dataGridViewUsers.ColumnCount = 2;
            dataGridViewUsers.AutoGenerateColumns = false;
            dataGridViewUsers.Columns[0].Name = "Логин";
            dataGridViewUsers.Columns[0].DataPropertyName = "Login";
            dataGridViewUsers.Columns[0].Width = 200;
            dataGridViewUsers.Columns[1].Name = "Пароль";
            dataGridViewUsers.Columns[1].DataPropertyName = "Password";
            dataGridViewUsers.DataSource = db.Users;

            dataGridViewAgents.ColumnCount = 3;
            dataGridViewAgents.AutoGenerateColumns = false;
            dataGridViewAgents.Columns[0].Name = "Имя";
            dataGridViewAgents.Columns[0].DataPropertyName = "Name";
            dataGridViewAgents.Columns[0].Width = 200;
            dataGridViewAgents.Columns[1].Name = "Адрес";
            dataGridViewAgents.Columns[1].DataPropertyName = "Address";
            dataGridViewAgents.Columns[2].Name = "Телефон";
            dataGridViewAgents.Columns[2].DataPropertyName = "Tel";
            dataGridViewAgents.DataSource = db.Agents;

            dataGridViewProducts.ColumnCount = 2;
            dataGridViewProducts.AutoGenerateColumns = false;
            dataGridViewProducts.Columns[0].Name = "Имя";
            dataGridViewProducts.Columns[0].DataPropertyName = "Name";
            dataGridViewProducts.Columns[0].Width = 200;
            dataGridViewProducts.Columns[1].Name = "Цена";
            dataGridViewProducts.Columns[1].DataPropertyName = "Price";
            dataGridViewProducts.DataSource = db.Products;
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            db.SaveChanges();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(tabControl.SelectedTab == tabPageDocuments)
            {
                Document doc = new Document {Id=-1, Name = "Новый документ", Date = DateTime.Now, AgentFromId = 1, AgentToId = 2};
                DocumentForm documentForm = new DocumentForm {value = doc};

                documentForm.Closed += delegate 
                { 
                    if (documentForm.DialogResult == DialogResult.OK)
                    {
                        //db.Documents.AddObject(documentForm.value);
                        //db.SaveChanges();
                        //dataGridViewDocuments.EndEdit();
                        //dataGridViewDocuments.DataSource = null;
                        //dataGridViewDocuments.DataSource = db.Documents;
                        dataGridViewDocuments.Rows.Insert(0, new DataGridViewRow());
                    }
                };

                documentForm.Show();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выделенные строки", "Удаление", MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes) 
                return;

            if (tabControl.SelectedTab == tabPageDocuments)
            {
                foreach (DataGridViewRow row in dataGridViewDocuments.SelectedRows)
                {
                    dataGridViewDocuments.Rows.Remove(row);
                }
            }else
                if (tabControl.SelectedTab == tabPageAgents)
                {
                    foreach (DataGridViewRow row in dataGridViewAgents.SelectedRows)
                    {
                        dataGridViewAgents.Rows.Remove(row);
                    }
                }
                else
                    if (tabControl.SelectedTab == tabPageProducts)
                    {
                        foreach (DataGridViewRow row in dataGridViewProducts.SelectedRows)
                        {
                            dataGridViewProducts.Rows.Remove(row);
                        }
                    }
                    else
                        if (tabControl.SelectedTab == tabPageUsers)
                        {
                            foreach (DataGridViewRow row in dataGridViewUsers.SelectedRows)
                            {
                                dataGridViewUsers.Rows.Remove(row);
                            }
                        }

            db.SaveChanges();
        }

        private void dataGridViewDocuments_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.RowIndex >= 0 && e.ColumnIndex>=0)
            {
                DocumentForm documentForm = new DocumentForm();
                int id = (int) (dataGridViewDocuments.Rows[e.RowIndex].Cells["Id"].Value);
                documentForm.value = db.Documents.FirstOrDefault(s => s.Id == id);
                documentForm.Show();

                documentForm.Closed += delegate
                {
                    if (documentForm.DialogResult == DialogResult.OK)
                    {
                        db.SaveChanges();
                    }
                };
            }
        }
    }
}
