
namespace Cursor_Position
{
    partial class Cursor_XYZ
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
            this.xyz_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // xyz_label
            // 
            this.xyz_label.AutoSize = true;
            this.xyz_label.ForeColor = System.Drawing.Color.White;
            this.xyz_label.Location = new System.Drawing.Point(12, 9);
            this.xyz_label.Name = "xyz_label";
            this.xyz_label.Size = new System.Drawing.Size(10, 13);
            this.xyz_label.TabIndex = 1;
            this.xyz_label.Text = "-";
            this.xyz_label.MouseDown += new System.Windows.Forms.MouseEventHandler(this.xyz_label_MouseDown);
            this.xyz_label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.xyz_label_MouseMove);
            // 
            // Main_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(85, 28);
            this.Controls.Add(this.xyz_label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Main_Form";
            this.Text = "Cursor Position";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Main_Form_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_Form_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Main_Form_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label xyz_label;
    }
}

