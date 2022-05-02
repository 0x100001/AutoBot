namespace Auto_Bot___Client
{
    partial class Main_Form
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main_Form));
            this.restart_bot_auto = new System.Windows.Forms.Timer(this.components);
            this.monitoring_timer = new System.Windows.Forms.Timer(this.components);
            this.openvpn_profile_combobox = new System.Windows.Forms.ComboBox();
            this.open_menu_button = new System.Windows.Forms.Button();
            this.menu_panel = new System.Windows.Forms.Panel();
            this.open_logfile_button = new System.Windows.Forms.Button();
            this.test_button = new System.Windows.Forms.Button();
            this.send_telegram_message_button = new System.Windows.Forms.Button();
            this.send_telegram_message_textbox = new System.Windows.Forms.TextBox();
            this.update_client_button = new System.Windows.Forms.Button();
            this.useragent_name_label = new System.Windows.Forms.Label();
            this.useragent_string_label = new System.Windows.Forms.Label();
            this.today_day_label = new System.Windows.Forms.Label();
            this.sheduled_sunday_label = new System.Windows.Forms.Label();
            this.sheduled_saturday_label = new System.Windows.Forms.Label();
            this.sheduled_friday_label = new System.Windows.Forms.Label();
            this.sheduled_thursday_label = new System.Windows.Forms.Label();
            this.sheduled_wednesday_label = new System.Windows.Forms.Label();
            this.sheduled_tuesday_label = new System.Windows.Forms.Label();
            this.sheduled_monday_label = new System.Windows.Forms.Label();
            this.open_chrome_button = new System.Windows.Forms.Button();
            this.file_explorer_button = new System.Windows.Forms.Button();
            this.cmd_button = new System.Windows.Forms.Button();
            this.reset_bot_settings_button = new System.Windows.Forms.Button();
            this.restart_bot_button = new System.Windows.Forms.Button();
            this.task_manager_button = new System.Windows.Forms.Button();
            this.runtime_label = new System.Windows.Forms.Label();
            this.client_name_label = new System.Windows.Forms.Label();
            this.spotify_album_listbox = new System.Windows.Forms.ListBox();
            this.spotify_credentials_datagridview = new System.Windows.Forms.DataGridView();
            this.menu_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spotify_credentials_datagridview)).BeginInit();
            this.SuspendLayout();
            // 
            // restart_bot_auto
            // 
            this.restart_bot_auto.Interval = 900000;
            this.restart_bot_auto.Tick += new System.EventHandler(this.restart_bot_auto_Tick);
            // 
            // monitoring_timer
            // 
            this.monitoring_timer.Enabled = true;
            this.monitoring_timer.Interval = 600000;
            this.monitoring_timer.Tick += new System.EventHandler(this.monitoring_timer_Tick);
            // 
            // openvpn_profile_combobox
            // 
            this.openvpn_profile_combobox.FormattingEnabled = true;
            this.openvpn_profile_combobox.Location = new System.Drawing.Point(383, 42);
            this.openvpn_profile_combobox.Name = "openvpn_profile_combobox";
            this.openvpn_profile_combobox.Size = new System.Drawing.Size(142, 21);
            this.openvpn_profile_combobox.TabIndex = 67;
            this.openvpn_profile_combobox.Visible = false;
            // 
            // open_menu_button
            // 
            this.open_menu_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.open_menu_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_menu_button.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_menu_button.ForeColor = System.Drawing.Color.White;
            this.open_menu_button.Location = new System.Drawing.Point(12, 30);
            this.open_menu_button.Name = "open_menu_button";
            this.open_menu_button.Size = new System.Drawing.Size(173, 54);
            this.open_menu_button.TabIndex = 69;
            this.open_menu_button.Text = "Open Menu";
            this.open_menu_button.UseVisualStyleBackColor = false;
            this.open_menu_button.Click += new System.EventHandler(this.open_menu_button_Click);
            // 
            // menu_panel
            // 
            this.menu_panel.BackColor = System.Drawing.Color.DimGray;
            this.menu_panel.Controls.Add(this.open_logfile_button);
            this.menu_panel.Controls.Add(this.test_button);
            this.menu_panel.Controls.Add(this.send_telegram_message_button);
            this.menu_panel.Controls.Add(this.send_telegram_message_textbox);
            this.menu_panel.Controls.Add(this.update_client_button);
            this.menu_panel.Controls.Add(this.useragent_name_label);
            this.menu_panel.Controls.Add(this.useragent_string_label);
            this.menu_panel.Controls.Add(this.today_day_label);
            this.menu_panel.Controls.Add(this.sheduled_sunday_label);
            this.menu_panel.Controls.Add(this.sheduled_saturday_label);
            this.menu_panel.Controls.Add(this.sheduled_friday_label);
            this.menu_panel.Controls.Add(this.sheduled_thursday_label);
            this.menu_panel.Controls.Add(this.sheduled_wednesday_label);
            this.menu_panel.Controls.Add(this.sheduled_tuesday_label);
            this.menu_panel.Controls.Add(this.sheduled_monday_label);
            this.menu_panel.Controls.Add(this.open_chrome_button);
            this.menu_panel.Controls.Add(this.file_explorer_button);
            this.menu_panel.Controls.Add(this.cmd_button);
            this.menu_panel.Controls.Add(this.reset_bot_settings_button);
            this.menu_panel.Controls.Add(this.restart_bot_button);
            this.menu_panel.Controls.Add(this.task_manager_button);
            this.menu_panel.Location = new System.Drawing.Point(12, 90);
            this.menu_panel.Name = "menu_panel";
            this.menu_panel.Size = new System.Drawing.Size(555, 459);
            this.menu_panel.TabIndex = 68;
            this.menu_panel.Visible = false;
            // 
            // open_logfile_button
            // 
            this.open_logfile_button.BackColor = System.Drawing.Color.LimeGreen;
            this.open_logfile_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_logfile_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_logfile_button.ForeColor = System.Drawing.Color.White;
            this.open_logfile_button.Location = new System.Drawing.Point(6, 400);
            this.open_logfile_button.Name = "open_logfile_button";
            this.open_logfile_button.Size = new System.Drawing.Size(543, 50);
            this.open_logfile_button.TabIndex = 76;
            this.open_logfile_button.Text = "Open Logfile";
            this.open_logfile_button.UseVisualStyleBackColor = false;
            this.open_logfile_button.Click += new System.EventHandler(this.open_logfile_button_Click);
            // 
            // test_button
            // 
            this.test_button.Location = new System.Drawing.Point(315, 371);
            this.test_button.Name = "test_button";
            this.test_button.Size = new System.Drawing.Size(234, 23);
            this.test_button.TabIndex = 75;
            this.test_button.Text = "Test Button";
            this.test_button.UseVisualStyleBackColor = true;
            this.test_button.Visible = false;
            // 
            // send_telegram_message_button
            // 
            this.send_telegram_message_button.BackColor = System.Drawing.Color.DarkViolet;
            this.send_telegram_message_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.send_telegram_message_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.send_telegram_message_button.ForeColor = System.Drawing.Color.White;
            this.send_telegram_message_button.Location = new System.Drawing.Point(315, 318);
            this.send_telegram_message_button.Name = "send_telegram_message_button";
            this.send_telegram_message_button.Size = new System.Drawing.Size(234, 50);
            this.send_telegram_message_button.TabIndex = 41;
            this.send_telegram_message_button.Text = "Send Telegram";
            this.send_telegram_message_button.UseVisualStyleBackColor = false;
            this.send_telegram_message_button.Click += new System.EventHandler(this.send_telegram_message_button_Click);
            // 
            // send_telegram_message_textbox
            // 
            this.send_telegram_message_textbox.Location = new System.Drawing.Point(315, 227);
            this.send_telegram_message_textbox.Multiline = true;
            this.send_telegram_message_textbox.Name = "send_telegram_message_textbox";
            this.send_telegram_message_textbox.Size = new System.Drawing.Size(234, 85);
            this.send_telegram_message_textbox.TabIndex = 41;
            // 
            // update_client_button
            // 
            this.update_client_button.BackColor = System.Drawing.Color.DarkViolet;
            this.update_client_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.update_client_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.update_client_button.ForeColor = System.Drawing.Color.White;
            this.update_client_button.Location = new System.Drawing.Point(3, 171);
            this.update_client_button.Name = "update_client_button";
            this.update_client_button.Size = new System.Drawing.Size(546, 50);
            this.update_client_button.TabIndex = 40;
            this.update_client_button.Text = "Update Cllient";
            this.update_client_button.UseVisualStyleBackColor = false;
            this.update_client_button.Click += new System.EventHandler(this.update_client_button_Click);
            // 
            // useragent_name_label
            // 
            this.useragent_name_label.AutoSize = true;
            this.useragent_name_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useragent_name_label.ForeColor = System.Drawing.SystemColors.Control;
            this.useragent_name_label.Location = new System.Drawing.Point(3, 360);
            this.useragent_name_label.Name = "useragent_name_label";
            this.useragent_name_label.Size = new System.Drawing.Size(155, 17);
            this.useragent_name_label.TabIndex = 39;
            this.useragent_name_label.Text = "Useragent Name: Default";
            // 
            // useragent_string_label
            // 
            this.useragent_string_label.AutoSize = true;
            this.useragent_string_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.useragent_string_label.ForeColor = System.Drawing.SystemColors.Control;
            this.useragent_string_label.Location = new System.Drawing.Point(3, 377);
            this.useragent_string_label.Name = "useragent_string_label";
            this.useragent_string_label.Size = new System.Drawing.Size(154, 17);
            this.useragent_string_label.TabIndex = 38;
            this.useragent_string_label.Text = "Useragent String: Default";
            // 
            // today_day_label
            // 
            this.today_day_label.AutoSize = true;
            this.today_day_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.today_day_label.ForeColor = System.Drawing.SystemColors.Control;
            this.today_day_label.Location = new System.Drawing.Point(3, 224);
            this.today_day_label.Name = "today_day_label";
            this.today_day_label.Size = new System.Drawing.Size(46, 17);
            this.today_day_label.TabIndex = 37;
            this.today_day_label.Text = "Today:";
            // 
            // sheduled_sunday_label
            // 
            this.sheduled_sunday_label.AutoSize = true;
            this.sheduled_sunday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_sunday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_sunday_label.Location = new System.Drawing.Point(3, 343);
            this.sheduled_sunday_label.Name = "sheduled_sunday_label";
            this.sheduled_sunday_label.Size = new System.Drawing.Size(66, 17);
            this.sheduled_sunday_label.TabIndex = 36;
            this.sheduled_sunday_label.Text = "Is Sunday:";
            // 
            // sheduled_saturday_label
            // 
            this.sheduled_saturday_label.AutoSize = true;
            this.sheduled_saturday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_saturday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_saturday_label.Location = new System.Drawing.Point(3, 326);
            this.sheduled_saturday_label.Name = "sheduled_saturday_label";
            this.sheduled_saturday_label.Size = new System.Drawing.Size(75, 17);
            this.sheduled_saturday_label.TabIndex = 35;
            this.sheduled_saturday_label.Text = "Is Saturday:";
            // 
            // sheduled_friday_label
            // 
            this.sheduled_friday_label.AutoSize = true;
            this.sheduled_friday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_friday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_friday_label.Location = new System.Drawing.Point(3, 309);
            this.sheduled_friday_label.Name = "sheduled_friday_label";
            this.sheduled_friday_label.Size = new System.Drawing.Size(59, 17);
            this.sheduled_friday_label.TabIndex = 34;
            this.sheduled_friday_label.Text = "Is Friday:";
            // 
            // sheduled_thursday_label
            // 
            this.sheduled_thursday_label.AutoSize = true;
            this.sheduled_thursday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_thursday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_thursday_label.Location = new System.Drawing.Point(3, 292);
            this.sheduled_thursday_label.Name = "sheduled_thursday_label";
            this.sheduled_thursday_label.Size = new System.Drawing.Size(77, 17);
            this.sheduled_thursday_label.TabIndex = 33;
            this.sheduled_thursday_label.Text = "Is Thursday:";
            // 
            // sheduled_wednesday_label
            // 
            this.sheduled_wednesday_label.AutoSize = true;
            this.sheduled_wednesday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_wednesday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_wednesday_label.Location = new System.Drawing.Point(3, 275);
            this.sheduled_wednesday_label.Name = "sheduled_wednesday_label";
            this.sheduled_wednesday_label.Size = new System.Drawing.Size(91, 17);
            this.sheduled_wednesday_label.TabIndex = 32;
            this.sheduled_wednesday_label.Text = "Is Wednesday:";
            // 
            // sheduled_tuesday_label
            // 
            this.sheduled_tuesday_label.AutoSize = true;
            this.sheduled_tuesday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_tuesday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_tuesday_label.Location = new System.Drawing.Point(3, 258);
            this.sheduled_tuesday_label.Name = "sheduled_tuesday_label";
            this.sheduled_tuesday_label.Size = new System.Drawing.Size(72, 17);
            this.sheduled_tuesday_label.TabIndex = 31;
            this.sheduled_tuesday_label.Text = "Is Tuesday:";
            // 
            // sheduled_monday_label
            // 
            this.sheduled_monday_label.AutoSize = true;
            this.sheduled_monday_label.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sheduled_monday_label.ForeColor = System.Drawing.SystemColors.Control;
            this.sheduled_monday_label.Location = new System.Drawing.Point(3, 241);
            this.sheduled_monday_label.Name = "sheduled_monday_label";
            this.sheduled_monday_label.Size = new System.Drawing.Size(72, 17);
            this.sheduled_monday_label.TabIndex = 30;
            this.sheduled_monday_label.Text = "Is Monday:";
            // 
            // open_chrome_button
            // 
            this.open_chrome_button.BackColor = System.Drawing.Color.CadetBlue;
            this.open_chrome_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.open_chrome_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.open_chrome_button.ForeColor = System.Drawing.Color.White;
            this.open_chrome_button.Location = new System.Drawing.Point(3, 115);
            this.open_chrome_button.Name = "open_chrome_button";
            this.open_chrome_button.Size = new System.Drawing.Size(546, 50);
            this.open_chrome_button.TabIndex = 29;
            this.open_chrome_button.Text = "Google Chrome";
            this.open_chrome_button.UseVisualStyleBackColor = false;
            this.open_chrome_button.Click += new System.EventHandler(this.open_chrome_button_Click);
            // 
            // file_explorer_button
            // 
            this.file_explorer_button.BackColor = System.Drawing.Color.LimeGreen;
            this.file_explorer_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.file_explorer_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.file_explorer_button.ForeColor = System.Drawing.Color.White;
            this.file_explorer_button.Location = new System.Drawing.Point(277, 59);
            this.file_explorer_button.Name = "file_explorer_button";
            this.file_explorer_button.Size = new System.Drawing.Size(272, 50);
            this.file_explorer_button.TabIndex = 27;
            this.file_explorer_button.Text = "Open File Explorer";
            this.file_explorer_button.UseVisualStyleBackColor = false;
            this.file_explorer_button.Click += new System.EventHandler(this.file_explorer_button_Click);
            // 
            // cmd_button
            // 
            this.cmd_button.BackColor = System.Drawing.Color.LimeGreen;
            this.cmd_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmd_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmd_button.ForeColor = System.Drawing.Color.White;
            this.cmd_button.Location = new System.Drawing.Point(3, 59);
            this.cmd_button.Name = "cmd_button";
            this.cmd_button.Size = new System.Drawing.Size(268, 50);
            this.cmd_button.TabIndex = 28;
            this.cmd_button.Text = "CMD";
            this.cmd_button.UseVisualStyleBackColor = false;
            this.cmd_button.Click += new System.EventHandler(this.cmd_button_Click);
            // 
            // reset_bot_settings_button
            // 
            this.reset_bot_settings_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.reset_bot_settings_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.reset_bot_settings_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reset_bot_settings_button.ForeColor = System.Drawing.Color.White;
            this.reset_bot_settings_button.Location = new System.Drawing.Point(371, 3);
            this.reset_bot_settings_button.Name = "reset_bot_settings_button";
            this.reset_bot_settings_button.Size = new System.Drawing.Size(178, 50);
            this.reset_bot_settings_button.TabIndex = 26;
            this.reset_bot_settings_button.Text = "Reset Bot Settings";
            this.reset_bot_settings_button.UseVisualStyleBackColor = false;
            this.reset_bot_settings_button.Click += new System.EventHandler(this.reset_bot_settings_button_Click);
            // 
            // restart_bot_button
            // 
            this.restart_bot_button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.restart_bot_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restart_bot_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.restart_bot_button.ForeColor = System.Drawing.Color.White;
            this.restart_bot_button.Location = new System.Drawing.Point(3, 3);
            this.restart_bot_button.Name = "restart_bot_button";
            this.restart_bot_button.Size = new System.Drawing.Size(178, 50);
            this.restart_bot_button.TabIndex = 24;
            this.restart_bot_button.Text = "Restart Bot";
            this.restart_bot_button.UseVisualStyleBackColor = false;
            this.restart_bot_button.Click += new System.EventHandler(this.restart_bot_button_Click);
            // 
            // task_manager_button
            // 
            this.task_manager_button.BackColor = System.Drawing.Color.LimeGreen;
            this.task_manager_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.task_manager_button.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.task_manager_button.ForeColor = System.Drawing.Color.White;
            this.task_manager_button.Location = new System.Drawing.Point(187, 3);
            this.task_manager_button.Name = "task_manager_button";
            this.task_manager_button.Size = new System.Drawing.Size(178, 50);
            this.task_manager_button.TabIndex = 25;
            this.task_manager_button.Text = "Task Manager";
            this.task_manager_button.UseVisualStyleBackColor = false;
            this.task_manager_button.Click += new System.EventHandler(this.task_manager_button_Click);
            // 
            // runtime_label
            // 
            this.runtime_label.AutoSize = true;
            this.runtime_label.Dock = System.Windows.Forms.DockStyle.Right;
            this.runtime_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runtime_label.ForeColor = System.Drawing.Color.White;
            this.runtime_label.Location = new System.Drawing.Point(1076, 0);
            this.runtime_label.Name = "runtime_label";
            this.runtime_label.Size = new System.Drawing.Size(14, 20);
            this.runtime_label.TabIndex = 70;
            this.runtime_label.Text = "-";
            // 
            // client_name_label
            // 
            this.client_name_label.AutoSize = true;
            this.client_name_label.Dock = System.Windows.Forms.DockStyle.Left;
            this.client_name_label.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.client_name_label.ForeColor = System.Drawing.Color.White;
            this.client_name_label.Location = new System.Drawing.Point(0, 0);
            this.client_name_label.Name = "client_name_label";
            this.client_name_label.Size = new System.Drawing.Size(99, 21);
            this.client_name_label.TabIndex = 71;
            this.client_name_label.Text = "Client Name:";
            // 
            // spotify_album_listbox
            // 
            this.spotify_album_listbox.FormattingEnabled = true;
            this.spotify_album_listbox.Location = new System.Drawing.Point(573, 28);
            this.spotify_album_listbox.Name = "spotify_album_listbox";
            this.spotify_album_listbox.Size = new System.Drawing.Size(228, 56);
            this.spotify_album_listbox.TabIndex = 72;
            this.spotify_album_listbox.Visible = false;
            // 
            // spotify_credentials_datagridview
            // 
            this.spotify_credentials_datagridview.AllowUserToAddRows = false;
            this.spotify_credentials_datagridview.AllowUserToDeleteRows = false;
            this.spotify_credentials_datagridview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.spotify_credentials_datagridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.spotify_credentials_datagridview.Location = new System.Drawing.Point(573, 90);
            this.spotify_credentials_datagridview.MultiSelect = false;
            this.spotify_credentials_datagridview.Name = "spotify_credentials_datagridview";
            this.spotify_credentials_datagridview.ReadOnly = true;
            this.spotify_credentials_datagridview.RowHeadersWidth = 5;
            this.spotify_credentials_datagridview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.spotify_credentials_datagridview.Size = new System.Drawing.Size(462, 405);
            this.spotify_credentials_datagridview.TabIndex = 74;
            this.spotify_credentials_datagridview.Visible = false;
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(1090, 656);
            this.Controls.Add(this.spotify_credentials_datagridview);
            this.Controls.Add(this.spotify_album_listbox);
            this.Controls.Add(this.client_name_label);
            this.Controls.Add(this.runtime_label);
            this.Controls.Add(this.open_menu_button);
            this.Controls.Add(this.menu_panel);
            this.Controls.Add(this.openvpn_profile_combobox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Bot - Client";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Gray;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.menu_panel.ResumeLayout(false);
            this.menu_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spotify_credentials_datagridview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer restart_bot_auto;
        private System.Windows.Forms.Button open_menu_button;
        private System.Windows.Forms.Panel menu_panel;
        private System.Windows.Forms.Button file_explorer_button;
        private System.Windows.Forms.Button cmd_button;
        private System.Windows.Forms.Button reset_bot_settings_button;
        private System.Windows.Forms.Button restart_bot_button;
        private System.Windows.Forms.Button task_manager_button;
        private System.Windows.Forms.Label runtime_label;
        private System.Windows.Forms.Label client_name_label;
        private System.Windows.Forms.Button open_chrome_button;
        private System.Windows.Forms.Label sheduled_monday_label;
        private System.Windows.Forms.Label sheduled_sunday_label;
        private System.Windows.Forms.Label sheduled_saturday_label;
        private System.Windows.Forms.Label sheduled_friday_label;
        private System.Windows.Forms.Label sheduled_thursday_label;
        private System.Windows.Forms.Label sheduled_wednesday_label;
        private System.Windows.Forms.Label sheduled_tuesday_label;
        private System.Windows.Forms.Label today_day_label;
        private System.Windows.Forms.Label useragent_string_label;
        private System.Windows.Forms.Label useragent_name_label;
        private System.Windows.Forms.Timer monitoring_timer;
        private System.Windows.Forms.Button update_client_button;
        public System.Windows.Forms.ListBox spotify_album_listbox;
        private System.Windows.Forms.Button send_telegram_message_button;
        private System.Windows.Forms.TextBox send_telegram_message_textbox;
        private System.Windows.Forms.DataGridView spotify_credentials_datagridview;
        private System.Windows.Forms.Button test_button;
        public System.Windows.Forms.ComboBox openvpn_profile_combobox;
        private System.Windows.Forms.Button open_logfile_button;
    }
}

