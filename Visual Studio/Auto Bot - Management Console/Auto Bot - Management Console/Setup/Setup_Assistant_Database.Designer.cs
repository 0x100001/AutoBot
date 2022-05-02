namespace Auto_Bot_Management_Console.Setup
{
    partial class Setup_Assistant_Database
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup_Assistant_Database));
            this.form_header = new System.Windows.Forms.Panel();
            this.guna2ControlBox2 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.guna2ControlBox1 = new Guna.UI2.WinForms.Guna2ControlBox();
            this.header_title_label = new System.Windows.Forms.Label();
            this.database_configuration_groupbox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.database_configuration_check_connection_button = new Guna.UI2.WinForms.Guna2Button();
            this.database_configuration_continue_button = new Guna.UI2.WinForms.Guna2Button();
            this.welcome_groupbox = new Guna.UI2.WinForms.Guna2GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.database_configuration_mysql_port_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.database_configuration_mysql_password_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.database_configuration_mysql_username_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.database_configuration_mysql_host_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.database_configuration_mysql_database_textbox = new Guna.UI2.WinForms.Guna2TextBox();
            this.form_header.SuspendLayout();
            this.welcome_groupbox.SuspendLayout();
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
            this.form_header.Size = new System.Drawing.Size(500, 33);
            this.form_header.TabIndex = 5;
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
            this.guna2ControlBox2.Location = new System.Drawing.Point(410, 0);
            this.guna2ControlBox2.Name = "guna2ControlBox2";
            this.guna2ControlBox2.ShadowDecoration.Parent = this.guna2ControlBox2;
            this.guna2ControlBox2.Size = new System.Drawing.Size(45, 33);
            this.guna2ControlBox2.TabIndex = 24;
            // 
            // guna2ControlBox1
            // 
            this.guna2ControlBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.guna2ControlBox1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.guna2ControlBox1.HoverState.Parent = this.guna2ControlBox1;
            this.guna2ControlBox1.IconColor = System.Drawing.Color.White;
            this.guna2ControlBox1.Location = new System.Drawing.Point(455, 0);
            this.guna2ControlBox1.Name = "guna2ControlBox1";
            this.guna2ControlBox1.ShadowDecoration.Parent = this.guna2ControlBox1;
            this.guna2ControlBox1.Size = new System.Drawing.Size(45, 33);
            this.guna2ControlBox1.TabIndex = 23;
            // 
            // header_title_label
            // 
            this.header_title_label.AutoSize = true;
            this.header_title_label.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.header_title_label.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.header_title_label.Location = new System.Drawing.Point(13, 9);
            this.header_title_label.Name = "header_title_label";
            this.header_title_label.Size = new System.Drawing.Size(272, 18);
            this.header_title_label.TabIndex = 1;
            this.header_title_label.Text = "Auto Bot - Setup Assistant (Database)";
            this.header_title_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseDown);
            this.header_title_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseMove);
            // 
            // database_configuration_groupbox
            // 
            this.database_configuration_groupbox.AutoSize = true;
            this.database_configuration_groupbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.database_configuration_groupbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.database_configuration_groupbox.Location = new System.Drawing.Point(339, 100);
            this.database_configuration_groupbox.Name = "database_configuration_groupbox";
            this.database_configuration_groupbox.Size = new System.Drawing.Size(6, 5);
            this.database_configuration_groupbox.TabIndex = 8;
            this.database_configuration_groupbox.TabStop = false;
            this.database_configuration_groupbox.Text = "Database Configuration";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 183);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 16);
            this.label1.TabIndex = 20;
            this.label1.Text = "MySQL Port:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(117, 16);
            this.label5.TabIndex = 18;
            this.label5.Text = "MySQL Password:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(1, 127);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(119, 16);
            this.label6.TabIndex = 17;
            this.label6.Text = "MySQL Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 16);
            this.label3.TabIndex = 14;
            this.label3.Text = "MySQL Database:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1, 71);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(143, 16);
            this.label4.TabIndex = 13;
            this.label4.Text = "MySQL Host(name/IP):";
            // 
            // database_configuration_check_connection_button
            // 
            this.database_configuration_check_connection_button.Animated = true;
            this.database_configuration_check_connection_button.CheckedState.Parent = this.database_configuration_check_connection_button;
            this.database_configuration_check_connection_button.CustomImages.Parent = this.database_configuration_check_connection_button;
            this.database_configuration_check_connection_button.FillColor = System.Drawing.SystemColors.ControlDarkDark;
            this.database_configuration_check_connection_button.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.database_configuration_check_connection_button.ForeColor = System.Drawing.Color.White;
            this.database_configuration_check_connection_button.HoverState.Parent = this.database_configuration_check_connection_button;
            this.database_configuration_check_connection_button.Location = new System.Drawing.Point(150, 205);
            this.database_configuration_check_connection_button.Name = "database_configuration_check_connection_button";
            this.database_configuration_check_connection_button.ShadowDecoration.Parent = this.database_configuration_check_connection_button;
            this.database_configuration_check_connection_button.Size = new System.Drawing.Size(156, 23);
            this.database_configuration_check_connection_button.TabIndex = 20;
            this.database_configuration_check_connection_button.Text = "Test Connection";
            this.database_configuration_check_connection_button.Click += new System.EventHandler(this.database_configuration_check_connection_button_Click);
            // 
            // database_configuration_continue_button
            // 
            this.database_configuration_continue_button.Animated = true;
            this.database_configuration_continue_button.CheckedState.Parent = this.database_configuration_continue_button;
            this.database_configuration_continue_button.CustomImages.Parent = this.database_configuration_continue_button;
            this.database_configuration_continue_button.Enabled = false;
            this.database_configuration_continue_button.FillColor = System.Drawing.SystemColors.ControlDarkDark;
            this.database_configuration_continue_button.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.database_configuration_continue_button.ForeColor = System.Drawing.Color.White;
            this.database_configuration_continue_button.HoverState.Parent = this.database_configuration_continue_button;
            this.database_configuration_continue_button.Location = new System.Drawing.Point(312, 205);
            this.database_configuration_continue_button.Name = "database_configuration_continue_button";
            this.database_configuration_continue_button.ShadowDecoration.Parent = this.database_configuration_continue_button;
            this.database_configuration_continue_button.Size = new System.Drawing.Size(156, 23);
            this.database_configuration_continue_button.TabIndex = 21;
            this.database_configuration_continue_button.Text = "Continue";
            this.database_configuration_continue_button.Click += new System.EventHandler(this.database_configuration_continue_button_Click);
            // 
            // welcome_groupbox
            // 
            this.welcome_groupbox.Controls.Add(this.label2);
            this.welcome_groupbox.Controls.Add(this.database_configuration_continue_button);
            this.welcome_groupbox.Controls.Add(this.database_configuration_mysql_port_textbox);
            this.welcome_groupbox.Controls.Add(this.database_configuration_mysql_password_textbox);
            this.welcome_groupbox.Controls.Add(this.database_configuration_check_connection_button);
            this.welcome_groupbox.Controls.Add(this.label4);
            this.welcome_groupbox.Controls.Add(this.database_configuration_mysql_username_textbox);
            this.welcome_groupbox.Controls.Add(this.label3);
            this.welcome_groupbox.Controls.Add(this.database_configuration_mysql_host_textbox);
            this.welcome_groupbox.Controls.Add(this.label6);
            this.welcome_groupbox.Controls.Add(this.database_configuration_mysql_database_textbox);
            this.welcome_groupbox.Controls.Add(this.label5);
            this.welcome_groupbox.Controls.Add(this.label1);
            this.welcome_groupbox.CustomBorderColor = System.Drawing.SystemColors.Control;
            this.welcome_groupbox.FillColor = System.Drawing.SystemColors.ControlLightLight;
            this.welcome_groupbox.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this.welcome_groupbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.welcome_groupbox.Location = new System.Drawing.Point(12, 39);
            this.welcome_groupbox.Name = "welcome_groupbox";
            this.welcome_groupbox.ShadowDecoration.Parent = this.welcome_groupbox;
            this.welcome_groupbox.Size = new System.Drawing.Size(475, 238);
            this.welcome_groupbox.TabIndex = 67;
            this.welcome_groupbox.Text = "Database Configuration";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(257, 16);
            this.label2.TabIndex = 73;
            this.label2.Text = "Please test your database connection now.";
            this.label2.Visible = false;
            // 
            // database_configuration_mysql_port_textbox
            // 
            this.database_configuration_mysql_port_textbox.Animated = true;
            this.database_configuration_mysql_port_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.database_configuration_mysql_port_textbox.DefaultText = "";
            this.database_configuration_mysql_port_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.database_configuration_mysql_port_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.database_configuration_mysql_port_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_port_textbox.DisabledState.Parent = this.database_configuration_mysql_port_textbox;
            this.database_configuration_mysql_port_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_port_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_port_textbox.FocusedState.Parent = this.database_configuration_mysql_port_textbox;
            this.database_configuration_mysql_port_textbox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.database_configuration_mysql_port_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.database_configuration_mysql_port_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_port_textbox.HoverState.Parent = this.database_configuration_mysql_port_textbox;
            this.database_configuration_mysql_port_textbox.Location = new System.Drawing.Point(150, 177);
            this.database_configuration_mysql_port_textbox.Name = "database_configuration_mysql_port_textbox";
            this.database_configuration_mysql_port_textbox.PasswordChar = '\0';
            this.database_configuration_mysql_port_textbox.PlaceholderText = "";
            this.database_configuration_mysql_port_textbox.SelectedText = "";
            this.database_configuration_mysql_port_textbox.ShadowDecoration.Parent = this.database_configuration_mysql_port_textbox;
            this.database_configuration_mysql_port_textbox.Size = new System.Drawing.Size(318, 22);
            this.database_configuration_mysql_port_textbox.TabIndex = 72;
            // 
            // database_configuration_mysql_password_textbox
            // 
            this.database_configuration_mysql_password_textbox.Animated = true;
            this.database_configuration_mysql_password_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.database_configuration_mysql_password_textbox.DefaultText = "";
            this.database_configuration_mysql_password_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.database_configuration_mysql_password_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.database_configuration_mysql_password_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_password_textbox.DisabledState.Parent = this.database_configuration_mysql_password_textbox;
            this.database_configuration_mysql_password_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_password_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_password_textbox.FocusedState.Parent = this.database_configuration_mysql_password_textbox;
            this.database_configuration_mysql_password_textbox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.database_configuration_mysql_password_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.database_configuration_mysql_password_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_password_textbox.HoverState.Parent = this.database_configuration_mysql_password_textbox;
            this.database_configuration_mysql_password_textbox.Location = new System.Drawing.Point(150, 149);
            this.database_configuration_mysql_password_textbox.Name = "database_configuration_mysql_password_textbox";
            this.database_configuration_mysql_password_textbox.PasswordChar = '\0';
            this.database_configuration_mysql_password_textbox.PlaceholderText = "";
            this.database_configuration_mysql_password_textbox.SelectedText = "";
            this.database_configuration_mysql_password_textbox.ShadowDecoration.Parent = this.database_configuration_mysql_password_textbox;
            this.database_configuration_mysql_password_textbox.Size = new System.Drawing.Size(318, 22);
            this.database_configuration_mysql_password_textbox.TabIndex = 71;
            this.database_configuration_mysql_password_textbox.UseSystemPasswordChar = true;
            // 
            // database_configuration_mysql_username_textbox
            // 
            this.database_configuration_mysql_username_textbox.Animated = true;
            this.database_configuration_mysql_username_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.database_configuration_mysql_username_textbox.DefaultText = "";
            this.database_configuration_mysql_username_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.database_configuration_mysql_username_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.database_configuration_mysql_username_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_username_textbox.DisabledState.Parent = this.database_configuration_mysql_username_textbox;
            this.database_configuration_mysql_username_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_username_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_username_textbox.FocusedState.Parent = this.database_configuration_mysql_username_textbox;
            this.database_configuration_mysql_username_textbox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.database_configuration_mysql_username_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.database_configuration_mysql_username_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_username_textbox.HoverState.Parent = this.database_configuration_mysql_username_textbox;
            this.database_configuration_mysql_username_textbox.Location = new System.Drawing.Point(150, 121);
            this.database_configuration_mysql_username_textbox.Name = "database_configuration_mysql_username_textbox";
            this.database_configuration_mysql_username_textbox.PasswordChar = '\0';
            this.database_configuration_mysql_username_textbox.PlaceholderText = "";
            this.database_configuration_mysql_username_textbox.SelectedText = "";
            this.database_configuration_mysql_username_textbox.ShadowDecoration.Parent = this.database_configuration_mysql_username_textbox;
            this.database_configuration_mysql_username_textbox.Size = new System.Drawing.Size(318, 22);
            this.database_configuration_mysql_username_textbox.TabIndex = 70;
            // 
            // database_configuration_mysql_host_textbox
            // 
            this.database_configuration_mysql_host_textbox.Animated = true;
            this.database_configuration_mysql_host_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.database_configuration_mysql_host_textbox.DefaultText = "";
            this.database_configuration_mysql_host_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.database_configuration_mysql_host_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.database_configuration_mysql_host_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_host_textbox.DisabledState.Parent = this.database_configuration_mysql_host_textbox;
            this.database_configuration_mysql_host_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_host_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_host_textbox.FocusedState.Parent = this.database_configuration_mysql_host_textbox;
            this.database_configuration_mysql_host_textbox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.database_configuration_mysql_host_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.database_configuration_mysql_host_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_host_textbox.HoverState.Parent = this.database_configuration_mysql_host_textbox;
            this.database_configuration_mysql_host_textbox.Location = new System.Drawing.Point(150, 65);
            this.database_configuration_mysql_host_textbox.Name = "database_configuration_mysql_host_textbox";
            this.database_configuration_mysql_host_textbox.PasswordChar = '\0';
            this.database_configuration_mysql_host_textbox.PlaceholderText = "";
            this.database_configuration_mysql_host_textbox.SelectedText = "";
            this.database_configuration_mysql_host_textbox.ShadowDecoration.Parent = this.database_configuration_mysql_host_textbox;
            this.database_configuration_mysql_host_textbox.Size = new System.Drawing.Size(318, 22);
            this.database_configuration_mysql_host_textbox.TabIndex = 68;
            // 
            // database_configuration_mysql_database_textbox
            // 
            this.database_configuration_mysql_database_textbox.Animated = true;
            this.database_configuration_mysql_database_textbox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.database_configuration_mysql_database_textbox.DefaultText = "";
            this.database_configuration_mysql_database_textbox.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.database_configuration_mysql_database_textbox.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.database_configuration_mysql_database_textbox.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_database_textbox.DisabledState.Parent = this.database_configuration_mysql_database_textbox;
            this.database_configuration_mysql_database_textbox.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.database_configuration_mysql_database_textbox.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_database_textbox.FocusedState.Parent = this.database_configuration_mysql_database_textbox;
            this.database_configuration_mysql_database_textbox.Font = new System.Drawing.Font("Arial", 9.75F);
            this.database_configuration_mysql_database_textbox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.database_configuration_mysql_database_textbox.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.database_configuration_mysql_database_textbox.HoverState.Parent = this.database_configuration_mysql_database_textbox;
            this.database_configuration_mysql_database_textbox.Location = new System.Drawing.Point(150, 93);
            this.database_configuration_mysql_database_textbox.Name = "database_configuration_mysql_database_textbox";
            this.database_configuration_mysql_database_textbox.PasswordChar = '\0';
            this.database_configuration_mysql_database_textbox.PlaceholderText = "";
            this.database_configuration_mysql_database_textbox.SelectedText = "";
            this.database_configuration_mysql_database_textbox.ShadowDecoration.Parent = this.database_configuration_mysql_database_textbox;
            this.database_configuration_mysql_database_textbox.Size = new System.Drawing.Size(318, 22);
            this.database_configuration_mysql_database_textbox.TabIndex = 69;
            // 
            // Setup_Assistant_Database
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(500, 289);
            this.Controls.Add(this.welcome_groupbox);
            this.Controls.Add(this.database_configuration_groupbox);
            this.Controls.Add(this.form_header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Setup_Assistant_Database";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Setup_Assistant_Database";
            this.Load += new System.EventHandler(this.Setup_Assistant_Database_Load);
            this.form_header.ResumeLayout(false);
            this.form_header.PerformLayout();
            this.welcome_groupbox.ResumeLayout(false);
            this.welcome_groupbox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel form_header;
        private System.Windows.Forms.Label header_title_label;
        private System.Windows.Forms.GroupBox database_configuration_groupbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Button database_configuration_check_connection_button;
        private Guna.UI2.WinForms.Guna2Button database_configuration_continue_button;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox2;
        private Guna.UI2.WinForms.Guna2ControlBox guna2ControlBox1;
        private Guna.UI2.WinForms.Guna2GroupBox welcome_groupbox;
        private Guna.UI2.WinForms.Guna2TextBox database_configuration_mysql_port_textbox;
        private Guna.UI2.WinForms.Guna2TextBox database_configuration_mysql_password_textbox;
        private Guna.UI2.WinForms.Guna2TextBox database_configuration_mysql_username_textbox;
        private Guna.UI2.WinForms.Guna2TextBox database_configuration_mysql_host_textbox;
        private Guna.UI2.WinForms.Guna2TextBox database_configuration_mysql_database_textbox;
        private System.Windows.Forms.Label label2;
    }
}