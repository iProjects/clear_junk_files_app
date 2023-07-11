namespace clear_junk_files_app
{
    partial class main_form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main_form));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblrunningtime = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbltimelapsed = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblprocessed = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnstopscan = new System.Windows.Forms.Button();
            this.btn_modify_saved_directories = new System.Windows.Forms.Button();
            this.btn_scan_entire_computer = new System.Windows.Forms.Button();
            this.txtdirectory_to_scan = new System.Windows.Forms.TextBox();
            this.btnadd_directory_to_scan = new System.Windows.Forms.Button();
            this.btnclose = new System.Windows.Forms.Button();
            this.btn_scan_saved_directories = new System.Windows.Forms.Button();
            this.btndelete = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listView_files_list = new System.Windows.Forms.ListView();
            this.txtlog = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkselectall = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.appNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.loggedInTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("statusStrip1.BackgroundImage")));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel1,
            this.lblrunningtime,
            this.toolStripStatusLabel3,
            this.lbltimelapsed,
            this.toolStripStatusLabel2,
            this.lblprocessed});
            this.statusStrip1.Location = new System.Drawing.Point(0, 686);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1144, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(150, 16);
            this.toolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel1.Text = "          ";
            // 
            // lblrunningtime
            // 
            this.lblrunningtime.Name = "lblrunningtime";
            this.lblrunningtime.Size = new System.Drawing.Size(73, 17);
            this.lblrunningtime.Text = "runningtime";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel3.Text = "          ";
            // 
            // lbltimelapsed
            // 
            this.lbltimelapsed.Name = "lbltimelapsed";
            this.lbltimelapsed.Size = new System.Drawing.Size(65, 17);
            this.lbltimelapsed.Text = "timelapsed";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(37, 17);
            this.toolStripStatusLabel2.Text = "          ";
            // 
            // lblprocessed
            // 
            this.lblprocessed.Name = "lblprocessed";
            this.lblprocessed.Size = new System.Drawing.Size(60, 17);
            this.lblprocessed.Text = "processed";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnstopscan);
            this.groupBox1.Controls.Add(this.btn_modify_saved_directories);
            this.groupBox1.Controls.Add(this.btn_scan_entire_computer);
            this.groupBox1.Controls.Add(this.txtdirectory_to_scan);
            this.groupBox1.Controls.Add(this.btnadd_directory_to_scan);
            this.groupBox1.Controls.Add(this.btnclose);
            this.groupBox1.Controls.Add(this.btn_scan_saved_directories);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 598);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1144, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // btnstopscan
            // 
            this.btnstopscan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnstopscan.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnstopscan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnstopscan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnstopscan.Location = new System.Drawing.Point(959, 31);
            this.btnstopscan.Name = "btnstopscan";
            this.btnstopscan.Size = new System.Drawing.Size(78, 23);
            this.btnstopscan.TabIndex = 5;
            this.btnstopscan.Text = "Stop Scan";
            this.btnstopscan.UseVisualStyleBackColor = false;
            this.btnstopscan.Click += new System.EventHandler(this.btnstopscan_Click);
            // 
            // btn_modify_saved_directories
            // 
            this.btn_modify_saved_directories.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btn_modify_saved_directories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_modify_saved_directories.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_modify_saved_directories.Location = new System.Drawing.Point(422, 34);
            this.btn_modify_saved_directories.Name = "btn_modify_saved_directories";
            this.btn_modify_saved_directories.Size = new System.Drawing.Size(187, 23);
            this.btn_modify_saved_directories.TabIndex = 2;
            this.btn_modify_saved_directories.Text = "Modify Saved Directories";
            this.btn_modify_saved_directories.UseVisualStyleBackColor = false;
            this.btn_modify_saved_directories.Click += new System.EventHandler(this.btn_modify_saved_directories_Click);
            // 
            // btn_scan_entire_computer
            // 
            this.btn_scan_entire_computer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btn_scan_entire_computer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_scan_entire_computer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_scan_entire_computer.Location = new System.Drawing.Point(805, 31);
            this.btn_scan_entire_computer.Name = "btn_scan_entire_computer";
            this.btn_scan_entire_computer.Size = new System.Drawing.Size(140, 23);
            this.btn_scan_entire_computer.TabIndex = 4;
            this.btn_scan_entire_computer.Text = "Scan Entire Computer";
            this.btn_scan_entire_computer.UseVisualStyleBackColor = false;
            this.btn_scan_entire_computer.Click += new System.EventHandler(this.btn_scan_entire_computer_Click);
            // 
            // txtdirectory_to_scan
            // 
            this.txtdirectory_to_scan.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtdirectory_to_scan.Dock = System.Windows.Forms.DockStyle.Left;
            this.txtdirectory_to_scan.Location = new System.Drawing.Point(3, 16);
            this.txtdirectory_to_scan.Multiline = true;
            this.txtdirectory_to_scan.Name = "txtdirectory_to_scan";
            this.txtdirectory_to_scan.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtdirectory_to_scan.Size = new System.Drawing.Size(221, 69);
            this.txtdirectory_to_scan.TabIndex = 0;
            // 
            // btnadd_directory_to_scan
            // 
            this.btnadd_directory_to_scan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnadd_directory_to_scan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnadd_directory_to_scan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnadd_directory_to_scan.Location = new System.Drawing.Point(268, 34);
            this.btnadd_directory_to_scan.Name = "btnadd_directory_to_scan";
            this.btnadd_directory_to_scan.Size = new System.Drawing.Size(140, 23);
            this.btnadd_directory_to_scan.TabIndex = 1;
            this.btnadd_directory_to_scan.Text = "Save Directory";
            this.btnadd_directory_to_scan.UseVisualStyleBackColor = false;
            this.btnadd_directory_to_scan.Click += new System.EventHandler(this.btnaddfoldertoscan_Click);
            // 
            // btnclose
            // 
            this.btnclose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnclose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnclose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnclose.Location = new System.Drawing.Point(1051, 31);
            this.btnclose.Name = "btnclose";
            this.btnclose.Size = new System.Drawing.Size(70, 23);
            this.btnclose.TabIndex = 6;
            this.btnclose.Text = "Exit";
            this.btnclose.UseVisualStyleBackColor = false;
            this.btnclose.Click += new System.EventHandler(this.btnclose_Click);
            // 
            // btn_scan_saved_directories
            // 
            this.btn_scan_saved_directories.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btn_scan_saved_directories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_scan_saved_directories.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_scan_saved_directories.Location = new System.Drawing.Point(623, 31);
            this.btn_scan_saved_directories.Name = "btn_scan_saved_directories";
            this.btn_scan_saved_directories.Size = new System.Drawing.Size(168, 23);
            this.btn_scan_saved_directories.TabIndex = 3;
            this.btn_scan_saved_directories.Text = "Scan Saved Directories";
            this.btn_scan_saved_directories.UseVisualStyleBackColor = false;
            this.btn_scan_saved_directories.Click += new System.EventHandler(this.btn_scan_saved_directories_Click);
            // 
            // btndelete
            // 
            this.btndelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btndelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btndelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btndelete.Location = new System.Drawing.Point(148, 15);
            this.btndelete.Name = "btndelete";
            this.btndelete.Size = new System.Drawing.Size(160, 23);
            this.btndelete.TabIndex = 1;
            this.btndelete.Text = "Delete Selected Files";
            this.btndelete.UseVisualStyleBackColor = false;
            this.btndelete.Click += new System.EventHandler(this.btndelete_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView_files_list);
            this.groupBox2.Controls.Add(this.txtlog);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 52);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1144, 546);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            // 
            // listView_files_list
            // 
            this.listView_files_list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView_files_list.CheckBoxes = true;
            this.listView_files_list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView_files_list.FullRowSelect = true;
            this.listView_files_list.GridLines = true;
            this.listView_files_list.HideSelection = false;
            this.listView_files_list.LabelEdit = true;
            this.listView_files_list.Location = new System.Drawing.Point(3, 16);
            this.listView_files_list.Name = "listView_files_list";
            this.listView_files_list.ShowItemToolTips = true;
            this.listView_files_list.Size = new System.Drawing.Size(419, 527);
            this.listView_files_list.TabIndex = 1;
            this.listView_files_list.UseCompatibleStateImageBehavior = false;
            this.listView_files_list.View = System.Windows.Forms.View.Details;
            this.listView_files_list.SizeChanged += new System.EventHandler(this.listView_files_list_SizeChanged);
            // 
            // txtlog
            // 
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtlog.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtlog.ForeColor = System.Drawing.Color.Lime;
            this.txtlog.Location = new System.Drawing.Point(422, 16);
            this.txtlog.Name = "txtlog";
            this.txtlog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(719, 527);
            this.txtlog.TabIndex = 0;
            this.txtlog.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkselectall);
            this.groupBox3.Controls.Add(this.btndelete);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1144, 52);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            // 
            // chkselectall
            // 
            this.chkselectall.AutoSize = true;
            this.chkselectall.Location = new System.Drawing.Point(12, 19);
            this.chkselectall.Name = "chkselectall";
            this.chkselectall.Size = new System.Drawing.Size(94, 17);
            this.chkselectall.TabIndex = 0;
            this.chkselectall.Text = "Select All Files";
            this.chkselectall.UseVisualStyleBackColor = true;
            this.chkselectall.CheckedChanged += new System.EventHandler(this.chkselectall_CheckedChanged);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // appNotifyIcon
            // 
            this.appNotifyIcon.Visible = true;
            // 
            // main_form
            // 
            this.AcceptButton = this.btnadd_directory_to_scan;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.CancelButton = this.btnstopscan;
            this.ClientSize = new System.Drawing.Size(1144, 708);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "main_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Clear Junk Files App";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.main_form_FormClosing);
            this.Load += new System.EventHandler(this.main_form_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel lblrunningtime;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel lbltimelapsed;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnclose;
        private System.Windows.Forms.Button btndelete;
        private System.Windows.Forms.Button btn_scan_saved_directories;
        private System.Windows.Forms.RichTextBox txtlog;
        private System.Windows.Forms.ListView listView_files_list;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel lblprocessed;
        private System.Windows.Forms.Button btnadd_directory_to_scan;
        private System.Windows.Forms.TextBox txtdirectory_to_scan;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Button btn_scan_entire_computer;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkselectall;
        private System.Windows.Forms.Button btn_modify_saved_directories;
        private System.Windows.Forms.Button btnstopscan;
        private System.Windows.Forms.NotifyIcon appNotifyIcon;
        private System.Windows.Forms.Timer loggedInTimer;

    }
}

