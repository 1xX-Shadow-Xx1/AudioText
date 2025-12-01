namespace AudioText.Presentacion
{
    partial class FormPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ofdSeleccionarAudio = new OpenFileDialog();
            txtRutaArchivo = new TextBox();
            btnSeleccionarAudio = new Button();
            grpConversion = new GroupBox();
            txtResultadoTexto = new TextBox();
            btnConvertirATexto = new Button();
            grpEncriptacion = new GroupBox();
            txtResultadoEncriptado = new TextBox();
            btnEncriptar = new Button();
            btnDesencriptar = new Button();
            pbProgreso = new ProgressBar();
            ConfigOpcionConverter = new ComboBox();
            grpConversion.SuspendLayout();
            grpEncriptacion.SuspendLayout();
            SuspendLayout();
            // 
            // ofdSeleccionarAudio
            // 
            ofdSeleccionarAudio.FileName = "OpenFileDialog";
            // 
            // txtRutaArchivo
            // 
            txtRutaArchivo.Location = new Point(35, 48);
            txtRutaArchivo.Name = "txtRutaArchivo";
            txtRutaArchivo.ReadOnly = true;
            txtRutaArchivo.Size = new Size(902, 23);
            txtRutaArchivo.TabIndex = 0;
            // 
            // btnSeleccionarAudio
            // 
            btnSeleccionarAudio.Location = new Point(959, 48);
            btnSeleccionarAudio.Name = "btnSeleccionarAudio";
            btnSeleccionarAudio.Size = new Size(75, 23);
            btnSeleccionarAudio.TabIndex = 1;
            btnSeleccionarAudio.Text = "Seleccionar";
            btnSeleccionarAudio.UseVisualStyleBackColor = true;
            btnSeleccionarAudio.Click += btnSeleccionarAudio_Click;
            // 
            // grpConversion
            // 
            grpConversion.Controls.Add(txtResultadoTexto);
            grpConversion.Location = new Point(35, 102);
            grpConversion.Name = "grpConversion";
            grpConversion.Size = new Size(902, 223);
            grpConversion.TabIndex = 2;
            grpConversion.TabStop = false;
            grpConversion.Text = "1. Conversión de Audio a Texto";
            // 
            // txtResultadoTexto
            // 
            txtResultadoTexto.Location = new Point(9, 13);
            txtResultadoTexto.Multiline = true;
            txtResultadoTexto.Name = "txtResultadoTexto";
            txtResultadoTexto.Size = new Size(887, 204);
            txtResultadoTexto.TabIndex = 4;
            // 
            // btnConvertirATexto
            // 
            btnConvertirATexto.Location = new Point(793, 331);
            btnConvertirATexto.Name = "btnConvertirATexto";
            btnConvertirATexto.Size = new Size(131, 23);
            btnConvertirATexto.TabIndex = 3;
            btnConvertirATexto.Text = "Convertir a Texto";
            btnConvertirATexto.UseVisualStyleBackColor = true;
            btnConvertirATexto.Click += btnConvertirATexto_Click;
            // 
            // grpEncriptacion
            // 
            grpEncriptacion.Controls.Add(txtResultadoEncriptado);
            grpEncriptacion.Location = new Point(35, 390);
            grpEncriptacion.Name = "grpEncriptacion";
            grpEncriptacion.Size = new Size(902, 269);
            grpEncriptacion.TabIndex = 5;
            grpEncriptacion.TabStop = false;
            grpEncriptacion.Text = "2. Encriptación de Texto";
            // 
            // txtResultadoEncriptado
            // 
            txtResultadoEncriptado.Location = new Point(6, 22);
            txtResultadoEncriptado.Multiline = true;
            txtResultadoEncriptado.Name = "txtResultadoEncriptado";
            txtResultadoEncriptado.Size = new Size(883, 231);
            txtResultadoEncriptado.TabIndex = 7;
            // 
            // btnEncriptar
            // 
            btnEncriptar.Location = new Point(731, 675);
            btnEncriptar.Name = "btnEncriptar";
            btnEncriptar.Size = new Size(75, 23);
            btnEncriptar.TabIndex = 6;
            btnEncriptar.Text = "Encriptar";
            btnEncriptar.UseVisualStyleBackColor = true;
            btnEncriptar.Click += btnEncriptar_Click;
            // 
            // btnDesencriptar
            // 
            btnDesencriptar.Location = new Point(835, 675);
            btnDesencriptar.Name = "btnDesencriptar";
            btnDesencriptar.Size = new Size(89, 25);
            btnDesencriptar.TabIndex = 8;
            btnDesencriptar.Text = "Desencriptar";
            btnDesencriptar.UseVisualStyleBackColor = true;
            btnDesencriptar.Click += btnDesencriptar_Click;
            // 
            // pbProgreso
            // 
            pbProgreso.Location = new Point(35, 331);
            pbProgreso.Name = "pbProgreso";
            pbProgreso.Size = new Size(518, 23);
            pbProgreso.TabIndex = 9;
            // 
            // ConfigOpcionConverter
            // 
            ConfigOpcionConverter.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigOpcionConverter.FormattingEnabled = true;
            ConfigOpcionConverter.Items.AddRange(new object[] { "Whisper ( local )", "Gemini ( Nube )" });
            ConfigOpcionConverter.Location = new Point(641, 332);
            ConfigOpcionConverter.Name = "ConfigOpcionConverter";
            ConfigOpcionConverter.Size = new Size(146, 23);
            ConfigOpcionConverter.TabIndex = 10;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1282, 728);
            Controls.Add(ConfigOpcionConverter);
            Controls.Add(pbProgreso);
            Controls.Add(btnDesencriptar);
            Controls.Add(btnEncriptar);
            Controls.Add(grpEncriptacion);
            Controls.Add(btnConvertirATexto);
            Controls.Add(grpConversion);
            Controls.Add(btnSeleccionarAudio);
            Controls.Add(txtRutaArchivo);
            Name = "FormPrincipal";
            Text = "AudioText - Conversor y Encriptador";
            grpConversion.ResumeLayout(false);
            grpConversion.PerformLayout();
            grpEncriptacion.ResumeLayout(false);
            grpEncriptacion.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog ofdSeleccionarAudio;
        private TextBox txtRutaArchivo;
        private Button btnSeleccionarAudio;
        private GroupBox grpConversion;
        private Button btnConvertirATexto;
        private TextBox txtResultadoTexto;
        private GroupBox grpEncriptacion;
        private Button btnEncriptar;
        private TextBox txtResultadoEncriptado;
        private Button btnDesencriptar;
        private ProgressBar pbProgreso;
        private ComboBox ConfigOpcionConverter;
    }
}
