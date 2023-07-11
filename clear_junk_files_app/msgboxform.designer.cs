/*
 * Created by SharpDevelop.
 * User: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 09/23/2018
 * Time: 17:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace clear_junk_files_app
{
	partial class msgboxform
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(msgboxform));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btncancel = new System.Windows.Forms.Button();
            this.imgmessagetype = new System.Windows.Forms.PictureBox();
            this.btnok = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtmsg = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgmessagetype)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btncancel);
            this.groupBox1.Controls.Add(this.imgmessagetype);
            this.groupBox1.Controls.Add(this.btnok);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 142);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(512, 59);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "press enter to exit";
            // 
            // btncancel
            // 
            this.btncancel.Location = new System.Drawing.Point(199, 13);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(183, 40);
            this.btncancel.TabIndex = 2;
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.Btncancel_Click);
            // 
            // imgmessagetype
            // 
            this.imgmessagetype.Image = ((System.Drawing.Image)(resources.GetObject("imgmessagetype.Image")));
            this.imgmessagetype.InitialImage = ((System.Drawing.Image)(resources.GetObject("imgmessagetype.InitialImage")));
            this.imgmessagetype.Location = new System.Drawing.Point(394, 13);
            this.imgmessagetype.Name = "imgmessagetype";
            this.imgmessagetype.Size = new System.Drawing.Size(112, 40);
            this.imgmessagetype.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgmessagetype.TabIndex = 1;
            this.imgmessagetype.TabStop = false;
            // 
            // btnok
            // 
            this.btnok.Location = new System.Drawing.Point(3, 16);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(190, 40);
            this.btnok.TabIndex = 0;
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.Btnok_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtmsg);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(512, 142);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // txtmsg
            // 
            this.txtmsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtmsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtmsg.Location = new System.Drawing.Point(3, 16);
            this.txtmsg.Multiline = true;
            this.txtmsg.Name = "txtmsg";
            this.txtmsg.ReadOnly = true;
            this.txtmsg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtmsg.Size = new System.Drawing.Size(506, 123);
            this.txtmsg.TabIndex = 0;
            // 
            // msgboxform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 201);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "msgboxform";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MsgboxformForm_Closing);
            this.Load += new System.EventHandler(this.msgboxform_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgmessagetype)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		private System.Windows.Forms.Button btncancel;
		private System.Windows.Forms.TextBox txtmsg;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnok;
		private System.Windows.Forms.PictureBox imgmessagetype;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}
