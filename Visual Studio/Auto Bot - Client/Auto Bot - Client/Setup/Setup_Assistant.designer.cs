namespace Auto_Bot___Client.Setup
{
    partial class Setup_Assistant
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
            this.form_header = new System.Windows.Forms.Panel();
            this.header_title_label = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.client_already_existing_checkbox = new System.Windows.Forms.CheckBox();
            this.client_name_textbox = new System.Windows.Forms.TextBox();
            this.database_configuration_continue_button = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.hive_name_combobox = new System.Windows.Forms.ComboBox();
            this.form_header.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // form_header
            // 
            this.form_header.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(1)))), ((int)(((byte)(18)))));
            this.form_header.Controls.Add(this.header_title_label);
            this.form_header.Dock = System.Windows.Forms.DockStyle.Top;
            this.form_header.Location = new System.Drawing.Point(0, 0);
            this.form_header.Name = "form_header";
            this.form_header.Size = new System.Drawing.Size(299, 33);
            this.form_header.TabIndex = 5;
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
            this.header_title_label.Size = new System.Drawing.Size(122, 18);
            this.header_title_label.TabIndex = 1;
            this.header_title_label.Text = "Auto Bot - Setup";
            this.header_title_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseDown);
            this.header_title_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.header_title_label_MouseMove);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.client_already_existing_checkbox);
            this.groupBox1.Controls.Add(this.client_name_textbox);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 116);
            this.groupBox1.TabIndex = 68;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "What should we call the client?";
            // 
            // client_already_existing_checkbox
            // 
            this.client_already_existing_checkbox.AutoSize = true;
            this.client_already_existing_checkbox.Location = new System.Drawing.Point(6, 28);
            this.client_already_existing_checkbox.Name = "client_already_existing_checkbox";
            this.client_already_existing_checkbox.Size = new System.Drawing.Size(263, 25);
            this.client_already_existing_checkbox.TabIndex = 74;
            this.client_already_existing_checkbox.Text = "Client already existing? Check me.";
            this.client_already_existing_checkbox.UseVisualStyleBackColor = true;
            // 
            // client_name_textbox
            // 
            this.client_name_textbox.Location = new System.Drawing.Point(6, 59);
            this.client_name_textbox.Name = "client_name_textbox";
            this.client_name_textbox.Size = new System.Drawing.Size(263, 29);
            this.client_name_textbox.TabIndex = 73;
            // 
            // database_configuration_continue_button
            // 
            this.database_configuration_continue_button.Location = new System.Drawing.Point(12, 252);
            this.database_configuration_continue_button.Name = "database_configuration_continue_button";
            this.database_configuration_continue_button.Size = new System.Drawing.Size(275, 22);
            this.database_configuration_continue_button.TabIndex = 69;
            this.database_configuration_continue_button.Text = "Finish Setup";
            this.database_configuration_continue_button.UseVisualStyleBackColor = true;
            this.database_configuration_continue_button.Click += new System.EventHandler(this.database_configuration_continue_button_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.hive_name_combobox);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.groupBox2.Location = new System.Drawing.Point(12, 161);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(275, 85);
            this.groupBox2.TabIndex = 70;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "What hive does the client use?";
            // 
            // hive_name_combobox
            // 
            this.hive_name_combobox.FormattingEnabled = true;
            this.hive_name_combobox.Location = new System.Drawing.Point(6, 28);
            this.hive_name_combobox.Name = "hive_name_combobox";
            this.hive_name_combobox.Size = new System.Drawing.Size(263, 29);
            this.hive_name_combobox.TabIndex = 75;
            // 
            // Setup_Assistant
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(299, 289);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.database_configuration_continue_button);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.form_header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Setup_Assistant";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Bot - Client";
            this.Load += new System.EventHandler(this.Setup_Assistant_Load);
            this.form_header.ResumeLayout(false);
            this.form_header.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel form_header;
        private System.Windows.Forms.Label header_title_label;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox client_already_existing_checkbox;
        private System.Windows.Forms.TextBox client_name_textbox;
        private System.Windows.Forms.Button database_configuration_continue_button;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox hive_name_combobox;
    }
}