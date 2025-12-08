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
            ConfigOpcionConverter = new ComboBox();
            grpCodigoMaquina = new GroupBox();
            txtResultadoTextoMaquina = new TextBox();
            btnVerBinarioHex = new Button();
            ConfigOpcionConverterBinaryHex = new ComboBox();
            grpConversion.SuspendLayout();
            grpEncriptacion.SuspendLayout();
            grpCodigoMaquina.SuspendLayout();
            SuspendLayout();
            // 
            // ofdSeleccionarAudio
            // 
            ofdSeleccionarAudio.FileName = "OpenFileDialog";
            // 
            // txtRutaArchivo
            // 
            txtRutaArchivo.Location = new Point(198, 44);
            txtRutaArchivo.Name = "txtRutaArchivo";
            txtRutaArchivo.ReadOnly = true;
            txtRutaArchivo.Size = new Size(902, 23);
            txtRutaArchivo.TabIndex = 0;
            // 
            // btnSeleccionarAudio
            // 
            btnSeleccionarAudio.Location = new Point(1122, 44);
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
            grpConversion.Location = new Point(199, 421);
            grpConversion.Name = "grpConversion";
            grpConversion.Size = new Size(902, 223);
            grpConversion.TabIndex = 2;
            grpConversion.TabStop = false;
            grpConversion.Text = "2. Conversión de Audio a Texto";
            // 
            // txtResultadoTexto
            // 
            txtResultadoTexto.Location = new Point(9, 13);
            txtResultadoTexto.Multiline = true;
            txtResultadoTexto.Name = "txtResultadoTexto";
            txtResultadoTexto.ScrollBars = ScrollBars.Vertical;
            txtResultadoTexto.Size = new Size(887, 204);
            txtResultadoTexto.TabIndex = 4;
            // 
            // btnConvertirATexto
            // 
            btnConvertirATexto.Location = new Point(957, 650);
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
            grpEncriptacion.Location = new Point(199, 709);
            grpEncriptacion.Name = "grpEncriptacion";
            grpEncriptacion.Size = new Size(902, 269);
            grpEncriptacion.TabIndex = 5;
            grpEncriptacion.TabStop = false;
            grpEncriptacion.Text = "3. Encriptación de Texto";
            // 
            // txtResultadoEncriptado
            // 
            txtResultadoEncriptado.Location = new Point(6, 22);
            txtResultadoEncriptado.Multiline = true;
            txtResultadoEncriptado.Name = "txtResultadoEncriptado";
            txtResultadoEncriptado.ScrollBars = ScrollBars.Vertical;
            txtResultadoEncriptado.Size = new Size(883, 231);
            txtResultadoEncriptado.TabIndex = 7;
            // 
            // btnEncriptar
            // 
            btnEncriptar.Location = new Point(895, 994);
            btnEncriptar.Name = "btnEncriptar";
            btnEncriptar.Size = new Size(75, 23);
            btnEncriptar.TabIndex = 6;
            btnEncriptar.Text = "Encriptar";
            btnEncriptar.UseVisualStyleBackColor = true;
            btnEncriptar.Click += btnEncriptar_Click;
            // 
            // btnDesencriptar
            // 
            btnDesencriptar.Location = new Point(999, 994);
            btnDesencriptar.Name = "btnDesencriptar";
            btnDesencriptar.Size = new Size(89, 25);
            btnDesencriptar.TabIndex = 8;
            btnDesencriptar.Text = "Desencriptar";
            btnDesencriptar.UseVisualStyleBackColor = true;
            btnDesencriptar.Click += btnDesencriptar_Click;
            // 
            // ConfigOpcionConverter
            // 
            ConfigOpcionConverter.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigOpcionConverter.FormattingEnabled = true;
            ConfigOpcionConverter.Items.AddRange(new object[] { "Whisper ( local )", "Gemini ( Nube )" });
            ConfigOpcionConverter.Location = new Point(805, 651);
            ConfigOpcionConverter.Name = "ConfigOpcionConverter";
            ConfigOpcionConverter.Size = new Size(146, 23);
            ConfigOpcionConverter.TabIndex = 10;
            // 
            // grpCodigoMaquina
            // 
            grpCodigoMaquina.Controls.Add(txtResultadoTextoMaquina);
            grpCodigoMaquina.Location = new Point(199, 127);
            grpCodigoMaquina.Name = "grpCodigoMaquina";
            grpCodigoMaquina.Size = new Size(902, 223);
            grpCodigoMaquina.TabIndex = 11;
            grpCodigoMaquina.TabStop = false;
            grpCodigoMaquina.Text = "1. Conversión de archivo al codigo de Maquina";
            // 
            // txtResultadoTextoMaquina
            // 
            txtResultadoTextoMaquina.Location = new Point(9, 13);
            txtResultadoTextoMaquina.Multiline = true;
            txtResultadoTextoMaquina.Name = "txtResultadoTextoMaquina";
            txtResultadoTextoMaquina.ScrollBars = ScrollBars.Vertical;
            txtResultadoTextoMaquina.Size = new Size(887, 204);
            txtResultadoTextoMaquina.TabIndex = 4;
            // 
            // btnVerBinarioHex
            // 
            btnVerBinarioHex.Location = new Point(957, 356);
            btnVerBinarioHex.Name = "btnVerBinarioHex";
            btnVerBinarioHex.Size = new Size(131, 23);
            btnVerBinarioHex.TabIndex = 12;
            btnVerBinarioHex.Text = "Convertir";
            btnVerBinarioHex.UseVisualStyleBackColor = true;
            btnVerBinarioHex.Click += btnVerBinario_Click;
            // 
            // ConfigOpcionConverterBinaryHex
            // 
            ConfigOpcionConverterBinaryHex.DropDownStyle = ComboBoxStyle.DropDownList;
            ConfigOpcionConverterBinaryHex.FormattingEnabled = true;
            ConfigOpcionConverterBinaryHex.Items.AddRange(new object[] { "Binario", "Hexadecimal" });
            ConfigOpcionConverterBinaryHex.Location = new Point(805, 357);
            ConfigOpcionConverterBinaryHex.Name = "ConfigOpcionConverterBinaryHex";
            ConfigOpcionConverterBinaryHex.Size = new Size(146, 23);
            ConfigOpcionConverterBinaryHex.TabIndex = 13;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1282, 1050);
            Controls.Add(ConfigOpcionConverterBinaryHex);
            Controls.Add(btnVerBinarioHex);
            Controls.Add(grpCodigoMaquina);
            Controls.Add(ConfigOpcionConverter);
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
            grpCodigoMaquina.ResumeLayout(false);
            grpCodigoMaquina.PerformLayout();
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
        private ComboBox ConfigOpcionConverter;
        private GroupBox grpCodigoMaquina;
        private TextBox txtResultadoTextoMaquina;
        private Button btnVerBinarioHex;
        private ComboBox ConfigOpcionConverterBinaryHex;
    }
}
