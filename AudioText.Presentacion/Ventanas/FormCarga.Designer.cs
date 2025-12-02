namespace AudioText.Presentacion.Ventanas
{
    partial class FormCarga
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
            pbProgreso = new ProgressBar();
            lblMensaje = new Label();
            SuspendLayout();
            // 
            // pbProgreso
            // 
            pbProgreso.Location = new Point(12, 38);
            pbProgreso.Name = "pbProgreso";
            pbProgreso.Size = new Size(387, 31);
            pbProgreso.TabIndex = 10;
            // 
            // lblMensaje
            // 
            lblMensaje.AutoSize = true;
            lblMensaje.Location = new Point(12, 9);
            lblMensaje.Name = "lblMensaje";
            lblMensaje.Size = new Size(78, 15);
            lblMensaje.TabIndex = 11;
            lblMensaje.Text = "Procesando...";
            // 
            // FormCarga
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(411, 112);
            Controls.Add(lblMensaje);
            Controls.Add(pbProgreso);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "FormCarga";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Transcribiendo el audio";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar pbProgreso;
        private Label lblMensaje;
    }
}