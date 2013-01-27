namespace PushNotification {
	partial class MainFrm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.txtMessage = new System.Windows.Forms.RichTextBox();
			this.btnSend = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lblCharLength = new System.Windows.Forms.Label();
			this.cmbRecipients = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.btnRefresh = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtMessage
			// 
			this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtMessage.Location = new System.Drawing.Point(12, 71);
			this.txtMessage.MaxLength = 165;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(402, 79);
			this.txtMessage.TabIndex = 0;
			this.txtMessage.Text = "";
			// 
			// btnSend
			// 
			this.btnSend.Enabled = false;
			this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSend.Location = new System.Drawing.Point(340, 156);
			this.btnSend.Name = "btnSend";
			this.btnSend.Size = new System.Drawing.Size(75, 30);
			this.btnSend.TabIndex = 1;
			this.btnSend.Text = "Send";
			this.btnSend.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(10, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(290, 18);
			this.label1.TabIndex = 2;
			this.label1.Text = "Enter the notification that you want to send:";
			// 
			// lblCharLength
			// 
			this.lblCharLength.AutoSize = true;
			this.lblCharLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblCharLength.ForeColor = System.Drawing.Color.Green;
			this.lblCharLength.Location = new System.Drawing.Point(13, 156);
			this.lblCharLength.Name = "lblCharLength";
			this.lblCharLength.Size = new System.Drawing.Size(40, 24);
			this.lblCharLength.TabIndex = 3;
			this.lblCharLength.Text = "165";
			// 
			// cmbRecipients
			// 
			this.cmbRecipients.DisplayMember = "Key";
			this.cmbRecipients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbRecipients.FormattingEnabled = true;
			this.cmbRecipients.Location = new System.Drawing.Point(74, 13);
			this.cmbRecipients.Name = "cmbRecipients";
			this.cmbRecipients.Size = new System.Drawing.Size(130, 21);
			this.cmbRecipients.TabIndex = 1;
			this.cmbRecipients.ValueMember = "Value";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(11, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Recipients:";
			// 
			// btnRefresh
			// 
			this.btnRefresh.Location = new System.Drawing.Point(210, 11);
			this.btnRefresh.Name = "btnRefresh";
			this.btnRefresh.Size = new System.Drawing.Size(54, 23);
			this.btnRefresh.TabIndex = 5;
			this.btnRefresh.Text = "Refresh";
			this.btnRefresh.UseVisualStyleBackColor = true;
			// 
			// MainFrm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(427, 193);
			this.Controls.Add(this.lblCharLength);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnSend);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.cmbRecipients);
			this.Controls.Add(this.btnRefresh);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainFrm";
			this.Text = "Notification Sender";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox txtMessage;
		private System.Windows.Forms.Button btnSend;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblCharLength;
		private System.Windows.Forms.Button btnRefresh;
		private System.Windows.Forms.ComboBox cmbRecipients;
		private System.Windows.Forms.Label label2;
	}
}

