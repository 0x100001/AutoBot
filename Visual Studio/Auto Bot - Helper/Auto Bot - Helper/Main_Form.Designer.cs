
namespace Auto_Bot___Helper
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.screenshot_delete_all_button = new System.Windows.Forms.Button();
            this.screenshot_send_via_mail_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.screenshot_ss_name_textbox = new System.Windows.Forms.TextBox();
            this.screenshot_take_ss_button = new System.Windows.Forms.Button();
            this.show_cursor_xyz_form_button = new System.Windows.Forms.Button();
            this.start_cursor_recorder_timer = new System.Windows.Forms.Button();
            this.cursor_recorder_timer = new System.Windows.Forms.Timer(this.components);
            this.stop_cursor_recorder_timer = new System.Windows.Forms.Button();
            this.play_cursor_recording_button = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.screenshot_delete_all_button);
            this.groupBox1.Controls.Add(this.screenshot_send_via_mail_button);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.screenshot_ss_name_textbox);
            this.groupBox1.Controls.Add(this.screenshot_take_ss_button);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(268, 164);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Screenshot";
            // 
            // screenshot_delete_all_button
            // 
            this.screenshot_delete_all_button.Location = new System.Drawing.Point(6, 92);
            this.screenshot_delete_all_button.Name = "screenshot_delete_all_button";
            this.screenshot_delete_all_button.Size = new System.Drawing.Size(256, 30);
            this.screenshot_delete_all_button.TabIndex = 6;
            this.screenshot_delete_all_button.Text = "Delete all SS";
            this.screenshot_delete_all_button.UseVisualStyleBackColor = true;
            this.screenshot_delete_all_button.Click += new System.EventHandler(this.screenshot_delete_all_button_Click);
            // 
            // screenshot_send_via_mail_button
            // 
            this.screenshot_send_via_mail_button.Location = new System.Drawing.Point(6, 128);
            this.screenshot_send_via_mail_button.Name = "screenshot_send_via_mail_button";
            this.screenshot_send_via_mail_button.Size = new System.Drawing.Size(256, 30);
            this.screenshot_send_via_mail_button.TabIndex = 5;
            this.screenshot_send_via_mail_button.Text = "Send via Mail";
            this.screenshot_send_via_mail_button.UseVisualStyleBackColor = true;
            this.screenshot_send_via_mail_button.Click += new System.EventHandler(this.screenshot_send_via_mail_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Name:";
            // 
            // screenshot_ss_name_textbox
            // 
            this.screenshot_ss_name_textbox.Location = new System.Drawing.Point(61, 25);
            this.screenshot_ss_name_textbox.Name = "screenshot_ss_name_textbox";
            this.screenshot_ss_name_textbox.Size = new System.Drawing.Size(201, 25);
            this.screenshot_ss_name_textbox.TabIndex = 2;
            // 
            // screenshot_take_ss_button
            // 
            this.screenshot_take_ss_button.Location = new System.Drawing.Point(6, 56);
            this.screenshot_take_ss_button.Name = "screenshot_take_ss_button";
            this.screenshot_take_ss_button.Size = new System.Drawing.Size(256, 30);
            this.screenshot_take_ss_button.TabIndex = 1;
            this.screenshot_take_ss_button.Text = "Take SS";
            this.screenshot_take_ss_button.UseVisualStyleBackColor = true;
            this.screenshot_take_ss_button.Click += new System.EventHandler(this.screenshot_take_ss_button_Click);
            // 
            // show_cursor_xyz_form_button
            // 
            this.show_cursor_xyz_form_button.Location = new System.Drawing.Point(12, 184);
            this.show_cursor_xyz_form_button.Name = "show_cursor_xyz_form_button";
            this.show_cursor_xyz_form_button.Size = new System.Drawing.Size(268, 30);
            this.show_cursor_xyz_form_button.TabIndex = 7;
            this.show_cursor_xyz_form_button.Text = "Cursor XYZ";
            this.show_cursor_xyz_form_button.UseVisualStyleBackColor = true;
            this.show_cursor_xyz_form_button.Click += new System.EventHandler(this.show_cursor_xyz_form_button_Click);
            // 
            // start_cursor_recorder_timer
            // 
            this.start_cursor_recorder_timer.Location = new System.Drawing.Point(12, 220);
            this.start_cursor_recorder_timer.Name = "start_cursor_recorder_timer";
            this.start_cursor_recorder_timer.Size = new System.Drawing.Size(268, 30);
            this.start_cursor_recorder_timer.TabIndex = 8;
            this.start_cursor_recorder_timer.Text = "Start Cursor Recorder";
            this.start_cursor_recorder_timer.UseVisualStyleBackColor = true;
            this.start_cursor_recorder_timer.Click += new System.EventHandler(this.start_cursor_recorder_timer_Click);
            // 
            // cursor_recorder_timer
            // 
            this.cursor_recorder_timer.Tick += new System.EventHandler(this.cursor_recorder_timer_Tick);
            // 
            // stop_cursor_recorder_timer
            // 
            this.stop_cursor_recorder_timer.Location = new System.Drawing.Point(12, 256);
            this.stop_cursor_recorder_timer.Name = "stop_cursor_recorder_timer";
            this.stop_cursor_recorder_timer.Size = new System.Drawing.Size(268, 30);
            this.stop_cursor_recorder_timer.TabIndex = 9;
            this.stop_cursor_recorder_timer.Text = "Stop Cursor Recorder";
            this.stop_cursor_recorder_timer.UseVisualStyleBackColor = true;
            this.stop_cursor_recorder_timer.Click += new System.EventHandler(this.stop_cursor_recorder_timer_Click);
            // 
            // play_cursor_recording_button
            // 
            this.play_cursor_recording_button.Location = new System.Drawing.Point(12, 292);
            this.play_cursor_recording_button.Name = "play_cursor_recording_button";
            this.play_cursor_recording_button.Size = new System.Drawing.Size(268, 30);
            this.play_cursor_recording_button.TabIndex = 10;
            this.play_cursor_recording_button.Text = "Play Cursor Recording";
            this.play_cursor_recording_button.UseVisualStyleBackColor = true;
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(292, 335);
            this.Controls.Add(this.play_cursor_recording_button);
            this.Controls.Add(this.stop_cursor_recorder_timer);
            this.Controls.Add(this.start_cursor_recorder_timer);
            this.Controls.Add(this.show_cursor_xyz_form_button);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Main_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Auto Bot - Helper";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox screenshot_ss_name_textbox;
        private System.Windows.Forms.Button screenshot_take_ss_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button screenshot_send_via_mail_button;
        private System.Windows.Forms.Button screenshot_delete_all_button;
        private System.Windows.Forms.Button show_cursor_xyz_form_button;
        private System.Windows.Forms.Button start_cursor_recorder_timer;
        private System.Windows.Forms.Timer cursor_recorder_timer;
        private System.Windows.Forms.Button stop_cursor_recorder_timer;
        private System.Windows.Forms.Button play_cursor_recording_button;
    }
}

