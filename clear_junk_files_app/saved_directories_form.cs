using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace clear_junk_files_app
{

    public class saved_directories_form : System.Windows.Forms.Form
    {

        public string TAG;

        /*Default values */
        const string APPLICATION = "SBPayroll";
        const string VERSION = "1.0.0.0";
        const string METADATA = "SBPayrollDBEntities";
        DataSet ds;
        BindingSource bs;
        private Button btndelete;
        string TableName;
        private DataGridView dataGridView_saved_directories;
        private DataGridViewTextBoxColumn Column1name;
        private BindingSource bindingSource_saved_directories;
        private TextBox txtdirectory_to_scan;
        private Button btnadd_directory_to_scan;
        private Button btnrefresh;
        private ErrorProvider errorProvider;
        List<saved_directories> lst_saved_directories_from_xml = new List<saved_directories>();

        public saved_directories_form()
        {
            InitializeComponent();

            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadException);

        }

        private void SystemsForm_Load(object sender, System.EventArgs e)
        {
            try
            {
                bs = new BindingSource(); //Private Variable class level 
                ds = new DataSet();

                lst_saved_directories_from_xml = get_saved_directories();
                bindingSource_saved_directories.DataSource = lst_saved_directories_from_xml;

                this.dataGridView_saved_directories.AutoGenerateColumns = false;
                this.dataGridView_saved_directories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                this.dataGridView_saved_directories.DataSource = bindingSource_saved_directories;
                groupBox2.Text = lst_saved_directories_from_xml.Count.ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);

                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "\n" + ex.InnerException.Message;
                MessageBox.Show(msg, Utils.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine(ex.ToString());
        }

        private void ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Console.WriteLine(ex.ToString());
        }

        private List<saved_directories> get_saved_directories()
        {
            try
            {
                string _filename = Utils.get_directories_to_scan_file();

                if (File.Exists(_filename))
                {
                    DataSet ds = new DataSet();

                    ds.ReadXml(_filename);

                    if (ds.Tables.Count > 0)
                    {
                        int count = ds.Tables[0].Rows.Count;
                        Console.WriteLine(count);

                        DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                        for (int i = 0; i < count; i++)
                        {
                            string saved_directory = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                            lst_saved_directories_from_xml.Add(new saved_directories(saved_directory));
                            Console.WriteLine(saved_directory);
                        }
                    }
                    Console.WriteLine("file for saved directories exists.");
                }
                else
                {
                    Console.WriteLine("file for saved directories does not exist.");
                }
                return lst_saved_directories_from_xml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private void refreshgrid()
        {
            try
            {
                bindingSource_saved_directories.DataSource = null;
                this.dataGridView_saved_directories.DataSource = null;
                lst_saved_directories_from_xml.Clear();

                lst_saved_directories_from_xml = get_saved_directories();
                bindingSource_saved_directories.DataSource = lst_saved_directories_from_xml;

                this.dataGridView_saved_directories.DataSource = bindingSource_saved_directories;
                groupBox2.Text = lst_saved_directories_from_xml.Count.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void btndelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (dataGridView_saved_directories.SelectedRows.Count != 0)
                {
                    if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete the selected record?", "Confirm Delete", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                    {
                        saved_directories dir = (saved_directories)bindingSource_saved_directories.Current;

                        string _filename = Utils.get_directories_to_scan_file();

                        if (File.Exists(_filename))
                        {
                            DataSet ds = new DataSet();

                            ds.ReadXml(_filename);

                            if (ds.Tables.Count > 0)
                            {
                                //if tables count is greater than zero we have records in xml

                                int count = ds.Tables[0].Rows.Count;

                                for (int i = 0; i < count; i++)
                                {
                                    ds.Tables[0].DefaultView.RowFilter = "directory_to_scan = '" + dir.name + "'";

                                    DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                                    int counta = dt.Rows.Count;

                                    if (counta > 0)
                                    {
                                        //we have a matching recording between what has been selected and what is in the xml
                                        try
                                        {
                                            ds.Tables[0].Rows[i].Delete();
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.WriteToErrorLogFile_and_EventViewer(ex);
                                        }
                                    }
                                }

                                //get data
                                string xmlData = ds.GetXml();

                                //save to file
                                ds.WriteXml(_filename);
                            }
                        }
                        else
                        {
                            Console.WriteLine("file for saved directories does not exist.");
                        }

                    }
                }

                refreshgrid();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);

                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "\n" + ex.InnerException.Message;
                MessageBox.Show(msg, Utils.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void dataGridView_saved_directories_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                e.ThrowException = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnadd_directory_to_scan_Click(object sender, System.EventArgs e)
        {
            try
            {
                errorProvider.Clear();
                if (string.IsNullOrEmpty(txtdirectory_to_scan.Text))
                {
                    errorProvider.SetError(txtdirectory_to_scan, "directory to scan cannot be null.");
                    Console.WriteLine("directory to scan cannot be null.");
                }
                else
                {
                    save_directory_to_scan(txtdirectory_to_scan.Text);
                    refreshgrid();
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void save_directory_to_scan(string directory_to_scan)
        {
            try
            {
                if (!Directory.Exists(directory_to_scan))
                {
                    errorProvider.SetError(txtdirectory_to_scan, "directory [ " + directory_to_scan + " ] does not exist.");
                    Console.WriteLine("directory [ " + directory_to_scan + " ] does not exist.");
                }
                else
                {
                    save_in_xml(directory_to_scan);
                    //save_in_sqlite(directory_to_scan);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void save_in_xml(string directory_to_scan)
        {
            try
            {
                string _filename = Utils.get_directories_to_scan_file();
                bool exists = false;

                if (File.Exists(_filename))
                {
                    lst_saved_directories_from_xml = new List<saved_directories>();
                    lst_saved_directories_from_xml = get_saved_directories();

                    foreach (saved_directories dir in lst_saved_directories_from_xml)
                    {
                        if (dir.name.Equals(directory_to_scan))
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        XDocument doc = XDocument.Load(_filename);

                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("directory_to_scan", directory_to_scan)
                            ));

                        doc.Save(_filename);

                        Console.WriteLine("directory saved in xml successfully.");
                        refreshgrid();
                    }
                    else if (exists)
                    {
                        Console.WriteLine("directory [ " + directory_to_scan + " ] exists.");
                        errorProvider.SetError(txtdirectory_to_scan, "directory [ " + directory_to_scan + " ] exists.");
                    }
                }
                else if (!File.Exists(_filename))
                {
                    List<String> systems = new List<String>() { 
                        directory_to_scan
                     };

                    var xml = new XElement("Systems", systems.Select(x =>
                            new XElement("System",
                            new XAttribute("directory_to_scan", directory_to_scan)
                            )));

                    xml.Save(_filename);

                    Console.WriteLine("directory saved in xml successfully.");
                    refreshgrid();
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void btnrefresh_Click(object sender, System.EventArgs e)
        {
            try
            {
                refreshgrid();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }


        #region "Windows Form Designer generated code"
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(saved_directories_form));
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnadd_directory_to_scan = new System.Windows.Forms.Button();
            this.txtdirectory_to_scan = new System.Windows.Forms.TextBox();
            this.btndelete = new System.Windows.Forms.Button();
            this.btnrefresh = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView_saved_directories = new System.Windows.Forms.DataGridView();
            this.Column1name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingSource_saved_directories = new System.Windows.Forms.BindingSource(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_saved_directories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource_saved_directories)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(770, 33);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 24);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnadd_directory_to_scan);
            this.groupBox1.Controls.Add(this.txtdirectory_to_scan);
            this.groupBox1.Controls.Add(this.btndelete);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnrefresh);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 352);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(853, 83);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // btnadd_directory_to_scan
            // 
            this.btnadd_directory_to_scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnadd_directory_to_scan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnadd_directory_to_scan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnadd_directory_to_scan.Location = new System.Drawing.Point(428, 34);
            this.btnadd_directory_to_scan.Name = "btnadd_directory_to_scan";
            this.btnadd_directory_to_scan.Size = new System.Drawing.Size(99, 23);
            this.btnadd_directory_to_scan.TabIndex = 6;
            this.btnadd_directory_to_scan.Text = "save directory";
            this.btnadd_directory_to_scan.UseVisualStyleBackColor = false;
            this.btnadd_directory_to_scan.Click += new System.EventHandler(this.btnadd_directory_to_scan_Click);
            // 
            // txtdirectory_to_scan
            // 
            this.txtdirectory_to_scan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtdirectory_to_scan.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtdirectory_to_scan.Location = new System.Drawing.Point(3, 16);
            this.txtdirectory_to_scan.Multiline = true;
            this.txtdirectory_to_scan.Name = "txtdirectory_to_scan";
            this.txtdirectory_to_scan.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtdirectory_to_scan.Size = new System.Drawing.Size(389, 64);
            this.txtdirectory_to_scan.TabIndex = 5;
            // 
            // btndelete
            // 
            this.btndelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btndelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btndelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btndelete.Location = new System.Drawing.Point(647, 33);
            this.btndelete.Name = "btndelete";
            this.btndelete.Size = new System.Drawing.Size(105, 24);
            this.btndelete.TabIndex = 4;
            this.btndelete.Text = "delete selected";
            this.btndelete.UseVisualStyleBackColor = false;
            this.btndelete.Click += new System.EventHandler(this.btndelete_Click);
            // 
            // btnrefresh
            // 
            this.btnrefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnrefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnrefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnrefresh.Location = new System.Drawing.Point(545, 33);
            this.btnrefresh.Name = "btnrefresh";
            this.btnrefresh.Size = new System.Drawing.Size(84, 24);
            this.btnrefresh.TabIndex = 2;
            this.btnrefresh.Text = "reload";
            this.btnrefresh.UseVisualStyleBackColor = false;
            this.btnrefresh.Click += new System.EventHandler(this.btnrefresh_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView_saved_directories);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(853, 352);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "saved directories";
            // 
            // dataGridView_saved_directories
            // 
            this.dataGridView_saved_directories.AllowUserToAddRows = false;
            this.dataGridView_saved_directories.AllowUserToDeleteRows = false;
            this.dataGridView_saved_directories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_saved_directories.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1name});
            this.dataGridView_saved_directories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_saved_directories.Location = new System.Drawing.Point(3, 16);
            this.dataGridView_saved_directories.MultiSelect = false;
            this.dataGridView_saved_directories.Name = "dataGridView_saved_directories";
            this.dataGridView_saved_directories.ReadOnly = true;
            this.dataGridView_saved_directories.Size = new System.Drawing.Size(847, 333);
            this.dataGridView_saved_directories.TabIndex = 0;
            this.dataGridView_saved_directories.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_saved_directories_DataError);
            // 
            // Column1name
            // 
            this.Column1name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1name.DataPropertyName = "name";
            this.Column1name.HeaderText = "Directories";
            this.Column1name.Name = "Column1name";
            this.Column1name.ReadOnly = true;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // SystemsForm
            // 
            this.AcceptButton = this.btnrefresh;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(853, 435);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SystemsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "saved directories";
            this.Load += new System.EventHandler(this.SystemsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_saved_directories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource_saved_directories)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.ComponentModel.IContainer components;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion "Windows Form Designer generated code"









    }
}
