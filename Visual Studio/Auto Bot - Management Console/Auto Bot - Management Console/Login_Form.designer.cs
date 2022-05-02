namespace Auto_Bot_Management_Console
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login_Form));
            this.form_header = new System.Windows.Forms.Panel();
            this.guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.header_title_label = new System.Windows.Forms.Label();
            this.username_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.password_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.borderless_control = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.background_picturebox = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.login_button = new Guna.UI2.WinForms.Guna2GradientButton();
            this.logo_picturebox = new Guna.UI2.WinForms.Guna2PictureBox();
            this.refresh_logo_timer = new System.Windows.Forms.Timer(this.components);
            this.form_header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.background_picturebox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logo_picturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // form_header
            // 
            this.form_header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.form_header.Controls.Add(this.guna2ControlBox2);
            this.form_header.Controls.Add(this.guna2ControlBox1);
            this.form_header.Controls.Add(this.header_title_label);
            this.form_header.Dock = System.Windows.Forms.DockStyle.Top;
            this.form_header.Location = new System.Drawing.Point(0, 0);
            this.form_header.Name = "form_header";
            this.form_header.Size = new System.Drawing.Size(1600, 33);
            this.form_header.TabIndex = 4;
            this.form_header.MouseDown += new System.Windows.Forms.MouseEventHandler(this.form_header_MouseDown);
            this.form_header.MouseMove += new System.Windows.Forms.MouseEventHandler(this.form_header_MouseMove);
            // 
            // guna2ControlBox2
            // 
            this.guna2ControlBox2.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.guna2ControlBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.guna2ControlBox2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.guna2ControlBox2.HoverState.Parent = this.guna2ControlBox2;
            this.guna2ControlBox2.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox2.Location = new System.Drawing.Point(1510, 0);
            this.guna2ControlBox2.Name = "guna2ControlBox2";
            this.guna2ControlBox2.ShadowDecoration.Parent = this.guna2ControlBox2;
            this.guna2ControlBox2.Size = new System.Drawing.Size(45, 33);
            this.guna2ControlBox2.TabIndex = 30;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.guna2ControlBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.guna2ControlBox1.HoverState.Parent = this.guna2ControlBox1;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(1555, 0);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.ShadowDecoration.Parent = this.guna2ControlBox1;
            this.guna2ControlBox1.Size = new System.Drawing.Size(45, 33);
            this.guna2ControlBox1.TabIndex = 29;
            // 
            // header_title_label
            // 
            this.header_title_label.AutoSize = true;
            this.header_title_label.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.header_title_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.header_title_label.Location = new System.Drawing.Point(39, 8);
            this.header_title_label.Name = "header_title_label";
            this.header_title_label.Size = new System.Drawing.Size(120, 18);
            this.header_title_label.TabIndex = 1;
            this.header_title_label.Text = "Auto Bot - Login";
            this.header_title_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseDown);
            this.header_title_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseMove);
            // 
            // username_textbox
            // 
            this.username_textbox.Animated = true;
            this.username_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.username_textbox.DefaultText = "";
            this.username_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.username_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.username_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.username_textbox.DisabledState.Parent = this.username_textbox;
            this.username_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.username_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.username_textbox.FocusedState.Parent = this.username_textbox;
            this.username_textbox.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.username_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.username_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.username_textbox.HoverState.Parent = this.username_textbox;
            this.username_textbox.Location = new System.Drawing.Point(617, 509);
            this.username_textbox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.username_textbox.Name = "username_textbox";
            this.username_textbox.PasswordChar = '\0';
            this.username_textbox.PlaceholderText = "";
            this.username_textbox.SelectedText = "";
            this.username_textbox.ShadowDecoration.Parent = this.username_textbox;
            this.username_textbox.Size = new System.Drawing.Size(363, 41);
            this.username_textbox.TabIndex = 23;
            this.username_textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.username_textbox_KeyDown);
            // 
            // password_textbox
            // 
            this.password_textbox.Animated = true;
            this.password_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.password_textbox.DefaultText = "";
            this.password_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.password_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.password_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.password_textbox.DisabledState.Parent = this.password_textbox;
            this.password_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.password_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.password_textbox.FocusedState.Parent = this.password_textbox;
            this.password_textbox.Font = new System.Drawing.Font("Segoe UI", 20.25F);
            this.password_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.password_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.password_textbox.HoverState.Parent = this.password_textbox;
            this.password_textbox.Location = new System.Drawing.Point(617, 556);
            this.password_textbox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.password_textbox.Name = "password_textbox";
            this.password_textbox.PasswordChar = '\0';
            this.password_textbox.PlaceholderText = "";
            this.password_textbox.SelectedText = "";
            this.password_textbox.ShadowDecoration.Parent = this.password_textbox;
            this.password_textbox.Size = new System.Drawing.Size(363, 41);
            this.password_textbox.TabIndex = 24;
            this.password_textbox.UseSystemPasswordChar = true;
            this.password_textbox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.password_textbox_KeyDown);
            // 
            // borderless_control
            // 
            this.borderless_control.AnimateWindow = true;
            this.borderless_control.AnimationType = Guna.UI2.WinForms.Guna2BorderlessForm.AnimateWindowType.AW_CENTER;
            this.borderless_control.ContainerControl = this;
            this.borderless_control.DragStartTransparencyValue = 0.7D;
            this.borderless_control.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.borderless_control.TransparentWhileDrag = true;
            // 
            // background_picturebox
            // 
            this.background_picturebox.Image = global::Auto_Bot___Management_Console.Properties.Resources.login;
            this.background_picturebox.Location = new System.Drawing.Point(0, 30);
            this.background_picturebox.Name = "background_picturebox";
            this.background_picturebox.Size = new System.Drawing.Size(1600, 920);
            this.background_picturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.background_picturebox.TabIndex = 68;
            this.background_picturebox.TabStop = false;
            this.background_picturebox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.background_picturebox_MouseDown);
            this.background_picturebox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.background_picturebox_MouseMove);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.pictureBox2.Image = global::Auto_Bot___Management_Console.Properties.Resources.logo;
            this.pictureBox2.Location = new System.Drawing.Point(0, 0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(33, 33);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 69;
            this.pictureBox2.TabStop = false;
            // 
            // login_button
            // 
            this.login_button.Animated = true;
            this.login_button.BackColor = System.Drawing.Color.Transparent;
            this.login_button.CheckedState.Parent = this.login_button;
            this.login_button.CustomImages.Parent = this.login_button;
            this.login_button.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.login_button.FillColor2 = System.Drawing.Color.Navy;
            this.login_button.Font = new System.Drawing.Font("Segoe UI", 20.25F);
            this.login_button.ForeColor = System.Drawing.Color.White;
            this.login_button.HoverState.FillColor = System.Drawing.Color.Navy;
            this.login_button.HoverState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.login_button.HoverState.Parent = this.login_button;
            this.login_button.Location = new System.Drawing.Point(617, 607);
            this.login_button.Name = "login_button";
            this.login_button.ShadowDecoration.Parent = this.login_button;
            this.login_button.Size = new System.Drawing.Size(363, 45);
            this.login_button.TabIndex = 70;
            this.login_button.Text = "Login";
            this.login_button.Click += new System.EventHandler(this.login_button_Click_1);
            // 
            // logo_picturebox
            // 
            this.logo_picturebox.BackColor = System.Drawing.Color.Transparent;
            this.logo_picturebox.Image = global::Auto_Bot___Management_Console.Properties.Resources.logo;
            this.logo_picturebox.Location = new System.Drawing.Point(683, 274);
            this.logo_picturebox.Name = "logo_picturebox";
            this.logo_picturebox.ShadowDecoration.BorderRadius = 0;
            this.logo_picturebox.ShadowDecoration.Parent = this.logo_picturebox;
            this.logo_picturebox.Size = new System.Drawing.Size(224, 225);
            this.logo_picturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.logo_picturebox.TabIndex = 72;
            this.logo_picturebox.TabStop = false;
            this.logo_picturebox.UseTransparentBackground = true;
            // 
            // refresh_logo_timer
            // 
            this.refresh_logo_timer.Enabled = true;
            this.refresh_logo_timer.Interval = 150;
            this.refresh_logo_timer.Tick += new System.EventHandler(this.refresh_logo_timer_Tick);
            // 
            // Login_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1600, 950);
            this.Controls.Add(this.logo_picturebox);
            this.Controls.Add(this.login_button);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.password_textbox);
            this.Controls.Add(this.username_textbox);
            this.Controls.Add(this.form_header);
            this.Controls.Add(this.background_picturebox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Login_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Bot - Login";
            this.Load += new System.EventHandler(this.Login_Form_Load);
            this.form_header.ResumeLayout(false);
            this.form_header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.background_picturebox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logo_picturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel form_header;
        private System.Windows.Forms.Label header_title_label;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2TextBox password_textbox;
        private Guna.UI2.WinForms.Guna2TextBox username_textbox;
        private System.Windows.Forms.PictureBox background_picturebox;
        private Guna.UI2.WinForms.Guna2BorderlessForm borderless_control;
        private System.Windows.Forms.PictureBox pictureBox2;
        private Guna.UI2.WinForms.Guna2GradientButton login_button;
        private Guna.UI2.WinForms.Guna2PictureBox logo_picturebox;
        private System.Windows.Forms.Timer refresh_logo_timer;
    }
}