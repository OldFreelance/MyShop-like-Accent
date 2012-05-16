using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
            //Создание столбцов и привязка данных для таблицы документов
            dataGridViewDocuments.ColumnCount = 5;
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
            dataGridViewDocuments.Columns[4].Name = "Сумма";
            dataGridViewDocuments.Columns[4].DataPropertyName = "Summa";
            bsDocumnts.DataSource = db.Documents;
            dataGridViewDocuments.DataSource = bsDocumnts;

            //Создание столбцов и привязка данных для таблицы пользователей
            dataGridViewUsers.ColumnCount = 2;
            dataGridViewUsers.AutoGenerateColumns = false;
            dataGridViewUsers.Columns[0].Name = "Логин";
            dataGridViewUsers.Columns[0].DataPropertyName = "Login";
            dataGridViewUsers.Columns[0].Width = 200;
            dataGridViewUsers.Columns[1].Name = "Пароль";
            dataGridViewUsers.Columns[1].DataPropertyName = "Password";
            dataGridViewUsers.DataSource = db.Users;

            //Создание столбцов и привязка данных для таблицы корреспондентов
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

            //Создание столбцов и привязка данных для таблицы товаров
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
            //Завершение редактирования во всех таблицах
            dataGridViewDocuments.EndEdit();
            dataGridViewUsers.EndEdit();
            dataGridViewAgents.EndEdit();
            dataGridViewProducts.EndEdit();

            db.SaveChanges();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if(tabControl.SelectedTab == tabPageDocuments)
            {
                //Создание нового документа
                Document doc = new Document {Id=-1, Name = "Новый документ", Date = DateTime.Now, AgentFromId = 1, AgentToId = 2};
                DocumentForm documentForm = new DocumentForm {value = doc};

                documentForm.Closed += delegate 
                { 
                    if (documentForm.DialogResult == DialogResult.OK)
                    {
                        //Пересчет суммы документа
                        //foreach (var detail in documentForm.value.DocumentDetails)
                        //{
                        //    detail
                        //}
                        
                        documentForm.value.Summa = documentForm.value.DocumentDetails.Sum(s => db.Products.FirstOrDefault(p=>p.Id==s.ProductId).Price * s.Qty);

                        db.Documents.AddObject(documentForm.value);
                        db.SaveChanges();
                        //TODO: Обновить грид
                    }
                };

                documentForm.Show();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить выделенные строки", "Удаление", MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes) 
                return;

            //Определяем какая вкладка сейчас активна и удаляем соответствующие объекты
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
                //Редактирование документа
                DocumentForm documentForm = new DocumentForm();
                int id = (int) (dataGridViewDocuments.Rows[e.RowIndex].Cells["Id"].Value);
                documentForm.value = db.Documents.FirstOrDefault(s => s.Id == id);
                documentForm.Show();

                documentForm.Closed += delegate
                {
                    if (documentForm.DialogResult == DialogResult.OK)
                    {
                        //Удалены лишь связи. Удаление самих объектов:
                        foreach (var detail in db.DocumentDetails.Where(s=>s.DocumentId==documentForm.value.Id))
                        {
                            if(!documentForm.value.DocumentDetails.Contains(detail))
                                db.DocumentDetails.DeleteObject(detail);
                        }

                        //Пересчет суммы документа
                        documentForm.value.Summa = documentForm.value.DocumentDetails.Sum(s => s.Product.Price*s.Qty);

                        db.SaveChanges();
                    }
                };
            }
        }

        private void tabPageStats_Enter(object sender, EventArgs e)
        {
            chart1.Series[0].Name = "Прибыль по месяцам";
            chart1.Series[0].Points.Clear();

            //Строим диаграмму от документа с самой ранней датой до самой поздней
            DateTime startDate = db.Documents.Min(s => s.Date).Date;
            DateTime endDate = db.Documents.Max(s => s.Date).Date;

            DateTime index = startDate;

            while(index<=endDate)
            {
                //Нахождение суммы всех приходных накладных за этот месяц
                List<Document> docsIn=db.Documents.Where(s => s.Date.Month == index.Month && s.Date.Year == index.Year && s.AgentToId == 1).ToList();
                decimal sumIn = docsIn.Sum(s => s.Summa);

                //Нахождение суммы всех расходных накладных за этот месяц
                List<Document> docsOut=db.Documents.Where(s => s.Date.Month == index.Month && s.Date.Year == index.Year && s.AgentFromId == 1).ToList();
                decimal sumOut = docsOut.Sum(s => s.Summa);

                //Создание точки на диаграмме с подписью текущего месяца
                DataPoint point=new DataPoint();
                point.Label = index.ToString("MMMM yyyy");
                point.YValues = new [] { (double)(sumOut - sumIn) };
                chart1.Series[0].Points.Add(point);

                //Переход к следующему месяцу
                index=index.AddMonths(1);
            }
        }
    }
}
