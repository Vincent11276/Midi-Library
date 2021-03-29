
namespace Midi_Visualizer
{
    partial class Form1
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
            this.FileSelectBtn = new System.Windows.Forms.Button();
            this.filePath_tb = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ImportBtn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileSelectBtn
            // 
            this.FileSelectBtn.Location = new System.Drawing.Point(268, 17);
            this.FileSelectBtn.Name = "FileSelectBtn";
            this.FileSelectBtn.Size = new System.Drawing.Size(75, 23);
            this.FileSelectBtn.TabIndex = 0;
            this.FileSelectBtn.Text = "Select File";
            this.FileSelectBtn.Click += new System.EventHandler(this.FileSelectBtn_Click);
            // 
            // filePath_tb
            // 
            this.filePath_tb.Location = new System.Drawing.Point(6, 19);
            this.filePath_tb.Name = "filePath_tb";
            this.filePath_tb.Size = new System.Drawing.Size(256, 20);
            this.filePath_tb.TabIndex = 1;
            this.filePath_tb.Text = "No File Selected";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ImportBtn);
            this.groupBox1.Controls.Add(this.filePath_tb);
            this.groupBox1.Controls.Add(this.FileSelectBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 115);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Import File";
            // 
            // ImportBtn
            // 
            this.ImportBtn.Location = new System.Drawing.Point(131, 86);
            this.ImportBtn.Name = "ImportBtn";
            this.ImportBtn.Size = new System.Drawing.Size(75, 23);
            this.ImportBtn.TabIndex = 3;
            this.ImportBtn.Text = "Import";
            this.ImportBtn.UseVisualStyleBackColor = true;
            this.ImportBtn.Click += new System.EventHandler(this.ImportBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 139);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button FileSelectBtn;
        private System.Windows.Forms.TextBox filePath_tb;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button ImportBtn;
    }
}

