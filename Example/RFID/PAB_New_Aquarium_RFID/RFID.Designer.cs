namespace PAB_New_Aquarium_RFID
{
    partial class RFID
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
            this.FormLabel = new System.Windows.Forms.Label();
            this.FormButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // FormLabel
            // 
            this.FormLabel.AutoSize = true;
            this.FormLabel.Location = new System.Drawing.Point(13, 673);
            this.FormLabel.Name = "FormLabel";
            this.FormLabel.Size = new System.Drawing.Size(100, 17);
            this.FormLabel.TabIndex = 0;
            this.FormLabel.Text = "Connection ID:";
            // 
            // FormButton
            // 
            this.FormButton.Location = new System.Drawing.Point(16, 647);
            this.FormButton.Name = "FormButton";
            this.FormButton.Size = new System.Drawing.Size(75, 23);
            this.FormButton.TabIndex = 1;
            this.FormButton.Text = "Minimize";
            this.FormButton.UseVisualStyleBackColor = true;
            this.FormButton.Click += new System.EventHandler(this.FormButton_Click);
            // 
            // RFID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.FormButton);
            this.Controls.Add(this.FormLabel);
            this.Name = "RFID";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "RFID";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label FormLabel;
        private System.Windows.Forms.Button FormButton;
    }
}

