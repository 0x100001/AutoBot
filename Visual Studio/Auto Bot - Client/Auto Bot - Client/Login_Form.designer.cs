namespace Auto_Bot___Client
{
    partial class Login_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login_Form));
            this.form_header = new System.Windows.Forms.Panel();
            this.header_title_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.username_textbox = new System.Windows.Forms.TextBox();
            this.password_textbox = new System.Windows.Forms.TextBox();
            this.login_button = new System.Windows.Forms.Button();
            this.form_header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // form_header
            // 
            this.form_header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.form_header.Controls.Add(this.header_title_label);
            this.form_header.Dock = System.Windows.Forms.DockStyle.Top;
            this.form_header.Location = new System.Drawing.Point(0, 0);
            this.form_header.Name = "form_header";
            this.form_header.Size = new System.Drawing.Size(405, 33);
            this.form_header.TabIndex = 4;
            this.form_header.MouseDown += new System.Windows.Forms.MouseEventHandler(this.form_header_MouseDown);
            this.form_header.MouseMove += new System.Windows.Forms.MouseEventHandler(this.form_header_MouseMove);
            // 
            // header_title_label
            // 
            this.header_title_label.AutoSize = true;
            this.header_title_label.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.header_title_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.header_title_label.Location = new System.Drawing.Point(13, 9);
            this.header_title_label.Name = "header_title_label";
            this.header_title_label.Size = new System.Drawing.Size(120, 18);
            this.header_title_label.TabIndex = 1;
            this.header_title_label.Text = "Auto Bot - Login";
            this.header_title_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseDown);
            this.header_title_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseMove);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "Password:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Username:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Auto_Bot___Client.Properties.Resources.logo;
            this.pictureBox1.Location = new System.Drawing.Point(295, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 64;
            this.pictureBox1.TabStop = false;
            // 
            // username_textbox
            // 
            this.username_textbox.Location = new System.Drawing.Point(89, 57);
            this.username_textbox.Name = "username_textbox";
            this.username_textbox.Size = new System.Drawing.Size(200, 20);
            this.username_textbox.TabIndex = 65;
            // 
            // password_textbox
            // 
            this.password_textbox.Location = new System.Drawing.Point(89, 85);
            this.password_textbox.Name = "password_textbox";
            this.password_textbox.Size = new System.Drawing.Size(200, 20);
            this.password_textbox.TabIndex = 66;
            this.password_textbox.UseSystemPasswordChar = true;
            // 
            // login_button
            // 
            this.login_button.Location = new System.Drawing.Point(89, 111);
            this.login_button.Name = "login_button";
            this.login_button.Size = new System.Drawing.Size(200, 23);
            this.login_button.TabIndex = 67;
            this.login_button.Text = "Login";
            this.login_button.UseVisualStyleBackColor = true;
            this.login_button.Click += new System.EventHandler(this.login_button_Click);
            // 
            // Login_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(405, 149);
            this.Controls.Add(this.login_button);
            this.Controls.Add(this.password_textbox);
            this.Controls.Add(this.username_textbox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.form_header);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Bot - Client";
            this.Load += new System.EventHandler(this.Login_Form_Load);
            this.form_header.ResumeLayout(false);
            this.form_header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel form_header;
        private System.Windows.Forms.Label header_title_label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox username_textbox;
        private System.Windows.Forms.TextBox password_textbox;
        private System.Windows.Forms.Button login_button;
    }
}