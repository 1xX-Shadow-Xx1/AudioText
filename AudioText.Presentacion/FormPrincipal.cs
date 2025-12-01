using AudioText.Helpers;
using AudioText.Interfaces;
using AudioText.Services;

namespace AudioText.Presentacion
{
    public partial class FormPrincipal : Form
    {
        // Declaramos las interfaces (Dependencia del contrato, no de la clase concreta: DIP)
        private IProcesadorAudio _procesadorAudio;
        private readonly IEncriptador _encriptador;

        // Variable para almacenar temporalmente el texto transcrito
        private string _textoTranscritorio = string.Empty;

        // Criterio 2 y 3: Documentación en español.

        /// <summary>
        /// Constructor del formulario. Aquí se inicializan las dependencias (simulación de Inyección de Dependencia).
        /// </summary>

        public FormPrincipal()
        {
            InitializeComponent();

            // 1. Configurar la selección visual del ComboBox
            // El índice 0 es el primer elemento que agregaste a la lista ("Whisper ( local )")
            ConfigOpcionConverter.SelectedIndex = 0;

            // 2. Inicializar la lógica interna por defecto
            // Esto asegura que la variable _procesadorAudio no sea null y coincida con la selección visual
            this._procesadorAudio = new WhisperProcessorService();

            // Inicializar el encriptador
            this._encriptador = new AesTextEncryptor();
        }

        // --- MANEJO DE ARCHIVOS ---

        /// <summary>
        /// Evento que maneja el clic del botón 'Seleccionar Archivo de Audio'. (Criterio 1.1)
        /// Abre un diálogo para que el usuario suba un archivo de audio (.MP3 o equivalentes).
        /// </summary>
        private void btnSeleccionarAudio_Click(object sender, EventArgs e)
        {
            // Criterio 1.1: Solicitar la subida o la carga de un archivo de audio.

            // Configura el diálogo para aceptar MP3, WAV, o ambos.
            ofdSeleccionarAudio.Filter = "Archivos de Audio (*.mp3;*.wav)|*.mp3;*.wav|Todos los archivos (*.*)|*.*";
            ofdSeleccionarAudio.Title = "Seleccionar Archivo de Audio (MP3 o WAV)";

            if (ofdSeleccionarAudio.ShowDialog() == DialogResult.OK)
            {
                txtRutaArchivo.Text = ofdSeleccionarAudio.FileName;

                // Limpiar resultados anteriores
                txtResultadoTexto.Clear();
                txtResultadoEncriptado.Clear();
                _textoTranscritorio = string.Empty;

                MessageBox.Show($"Archivo seleccionado: {Path.GetFileName(ofdSeleccionarAudio.FileName)}", "Archivo Cargado");
            }
        }

        // --- CONVERSIÓN DE AUDIO A TEXTO ---

        /// <summary>
        /// Evento que maneja el clic del botón 'Convertir a Texto'. (Criterio 1.2)
        /// Realiza la conversión utilizando el servicio IProcesadorAudio.
        /// </summary>
        private async void btnConvertirATexto_Click(object sender, EventArgs e)
        {

            // 1. Validaciones Iniciales
            if (string.IsNullOrEmpty(txtRutaArchivo.Text) || !File.Exists(txtRutaArchivo.Text))
            {
                MessageBox.Show("Por favor, seleccione un archivo de audio válido.", "Archivo Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Selección del Cerebro (Servicio)
            string opcion = ConfigOpcionConverter.Text;

            if (opcion.Contains("Whisper"))
            {
                this._procesadorAudio = new WhisperProcessorService();
            }
            else if (opcion.Contains("Gemini"))
            {
                this._procesadorAudio = new GeminiProcessorService();
            }

            try
            {
                // 3. Configurar Interfaz Visual
                btnConvertirATexto.Enabled = false;
                txtResultadoTexto.Clear();
                txtResultadoTexto.Text = $"Iniciando transcripción con {opcion}...\r\n";
                pbProgreso.Value = 0;

                string rutaOriginal = txtRutaArchivo.Text;
                string rutaParaProcesar = rutaOriginal;

                // 4. Preparación Inteligente del Audio
                if (opcion.Contains("Whisper"))
                {
                    // WHISPER: Es "delicado". Exige WAV 16kHz Mono.
                    txtResultadoTexto.AppendText(">> Optimizando audio para motor local (WAV 16kHz)...\r\n");

                    rutaParaProcesar = await Task.Run(() =>
                       AudioConverterHelper.PrepararAudioParaProcesamiento(rutaOriginal,
                       Path.Combine(Path.GetTempPath(), "temp_whisper_16k.wav")));
                }
                else
                {
                    // GEMINI: Es "flexible". Prefiere el archivo original (MP3/M4A) para subir rápido.
                    // Solo convertimos si es un formato muy extraño que la API no soporte.
                    string ext = Path.GetExtension(rutaOriginal).ToLower();
                    if (ext != ".mp3" && ext != ".wav" && ext != ".m4a" && ext != ".ogg")
                    {
                        txtResultadoTexto.AppendText(">> Formato raro detectado. Convirtiendo a estándar...\r\n");
                        rutaParaProcesar = await Task.Run(() =>
                           AudioConverterHelper.PrepararAudioParaProcesamiento(rutaOriginal,
                           Path.Combine(Path.GetTempPath(), "temp_universal.wav")));
                    }
                    else
                    {
                        txtResultadoTexto.AppendText(">> Enviando archivo original a la IA (Carga rápida)...\r\n");
                    }
                }

                // 5. Configurar Reportes de Progreso
                var reporteTexto = new Progress<string>(texto =>
                {
                    txtResultadoTexto.AppendText(texto);
                    // Hacer scroll automático al final
                    txtResultadoTexto.SelectionStart = txtResultadoTexto.Text.Length;
                    txtResultadoTexto.ScrollToCaret();
                });

                var reportePorcentaje = new Progress<int>(p => pbProgreso.Value = p);

                // 6. ¡EJECUTAR!
                string textoFinal = await _procesadorAudio.ConvertirATextoAsync(rutaParaProcesar, reporteTexto, reportePorcentaje);

                // 🚨 AQUÍ ESTABA EL ERROR: Recibíamos el texto pero no lo mostrábamos.
                // AGREGA ESTAS LÍNEAS:

                txtResultadoTexto.AppendText("\r\n\r\n============== RESULTADO ==============\r\n\r\n");
                txtResultadoTexto.AppendText(textoFinal); // <--- ¡Esto es lo que faltaba!

                // Guardar en variable global para encriptar
                _textoTranscritorio = textoFinal;

                MessageBox.Show("¡Proceso Terminado Exitosamente!", "Transcripción Lista", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show($"Error de Formato: {ex.Message}\n\nNota: Para MP3 se requiere una librería de terceros (como NAudio) para la conversión a WAV antes de transcribir.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NotImplementedException ex)
            {
                MessageBox.Show($"Error de Conversión Interna: {ex.Message}", "Conversión Pendiente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error en el procesamiento: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                pbProgreso.Value = 0;
            }
            finally
            {
                btnConvertirATexto.Enabled = true;
            }
        }

        // --- ENCRIPTACIÓN Y DESENCRIPTACIÓN ---

        /// <summary>
        /// Evento que maneja el clic del botón 'Encriptar Texto'. (Criterio 1.3)
        /// Encripta el texto del área de resultado de la conversión.
        /// </summary>
        private void btnEncriptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_textoTranscritorio))
            {
                MessageBox.Show("Primero debe convertir un audio a texto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Criterio 1.3: Al presionar "Encriptar Texto" deberá convertir en AES.
                string textoCifrado = _encriptador.Encriptar(_textoTranscritorio);

                txtResultadoEncriptado.Text = textoCifrado;
                MessageBox.Show("Texto encriptado correctamente con AES.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al encriptar: {ex.Message}", "Error de Encriptación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento que maneja el clic del botón 'Desencriptar Texto'. (Criterio 1.3 Opcional)
        /// Desencripta el texto del área de resultado de la encriptación.
        /// </summary>
        private void btnDesencriptar_Click(object sender, EventArgs e)
        {
            string textoCifrado = txtResultadoEncriptado.Text;

            if (string.IsNullOrEmpty(textoCifrado))
            {
                MessageBox.Show("Primero debe encriptar el texto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Criterio 1.3 Opcional: Desencriptar.
                string textoDesencriptado = _encriptador.Desencriptar(textoCifrado);

                // Mostramos el texto original en la caja de resultado de la conversión,
                // verificando que la operación fue exitosa.
                txtResultadoTexto.Text = textoDesencriptado;

                // Nota para el usuario: Se confirma que la desencriptación es exitosa.
                MessageBox.Show("Desencriptación completada. El texto original ha sido restaurado en la caja de conversión.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al desencriptar. Clave o formato incorrecto: {ex.Message}", "Error de Desencriptación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
