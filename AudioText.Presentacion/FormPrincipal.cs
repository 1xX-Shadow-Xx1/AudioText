using AudioText.Helpers;
using AudioText.Interfaces;
using AudioText.Presentacion.Ventanas;
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

            // 2. Configurar servicios
            string opcion = ConfigOpcionConverter.Text;

            if (opcion.Contains("Whisper"))
            {
                this._procesadorAudio = new WhisperProcessorService();
            }
            else if (opcion.Contains("Gemini"))
            {
                this._procesadorAudio = new GeminiProcessorService();
            }


            // 3. CAPTURAR VALORES DE LA UI ANTES DE ENTRAR AL HILO
            string rutaOriginal = txtRutaArchivo.Text;

            // 4. Preparar la Interfaz
            btnConvertirATexto.Enabled = false; // Deshabilitamos AQUI, no dentro del Task
            txtResultadoTexto.Clear();
            txtResultadoEncriptado.Clear();
            _textoTranscritorio = string.Empty;

            using (var cts = new CancellationTokenSource())
            {
                using (var formCarga = new FormCarga())
                {
                    // formCarga.OnCancelarClick += (s, args) => cts.Cancel(); // Removed

                    try
                    {
                        // Lanzamos la tarea pesada
                        Task<string> tareaProcesamiento = Task.Run(async () =>
                        {
                            try
                            {
                                // AQUI DENTRO SOLO LÓGICA, NADA DE 'txtRutaArchivo.Text' NI 'btn...'

                                string rutaProcesar = rutaOriginal; // Usamos la variable capturada arriba

                                // Reportar estado a la ventanita (FormCarga maneja su propio Invoke seguro)
                                formCarga.ActualizarMensaje("Verificando audio...");
                                formCarga.ActualizarProgreso(5);

                                // Lógica de preparación de audio (sin tocar UI principal)
                                if (opcion.Contains("Whisper"))
                                {
                                    formCarga.ActualizarMensaje("Convirtiendo a WAV 16kHz...");
                                    rutaProcesar = await Task.Run(() =>
                                       AudioConverterHelper.PrepararAudioParaProcesamiento(rutaOriginal,
                                       Path.Combine(Path.GetTempPath(), "temp_whisper_16k.wav")));
                                }
                                else
                                {
                                    // Lógica para Gemini/Google
                                    string ext = Path.GetExtension(rutaProcesar).ToLower();
                                    if (ext != ".mp3" && ext != ".wav" && ext != ".m4a")
                                    {
                                        formCarga.ActualizarMensaje("Normalizando formato...");
                                        rutaProcesar = await Task.Run(() =>
                                           AudioConverterHelper.PrepararAudioParaProcesamiento(rutaOriginal,
                                           Path.Combine(Path.GetTempPath(), "temp_universal.wav")));
                                    }
                                }

                                cts.Token.ThrowIfCancellationRequested();

                                // Transcripción
                                formCarga.ActualizarMensaje($"Motor IA ({opcion}) trabajando...");

                                // Reportadores seguros
                                var progTexto = new Progress<string>(t => { /* Vacío para no mostrar texto */ });
                                var progPorc = new Progress<int>(p => formCarga.ActualizarProgreso(p));

                                return await _procesadorAudio.ConvertirATextoAsync(rutaProcesar, progTexto, progPorc, cts.Token);
                            }
                            finally
                            {
                                // Cerramos la ventanita de carga de forma segura
                                formCarga.Invoke(new Action(() => formCarga.Close()));
                            }
                        }, cts.Token);

                        // Mostramos la ventana de carga (Bloquea la UI hasta que se cierre)
                        formCarga.ShowDialog(this);

                        // ---------------------------------------------------------
                        // DE VUELTA A LA ZONA SEGURA (HILO UI)
                        // ---------------------------------------------------------

                        // Esperamos el resultado (o excepción)
                        string textoFinal = await tareaProcesamiento;

                        // AQUI SÍ podemos tocar cajas de texto y botones de nuevo
                        txtResultadoTexto.Text = textoFinal;
                        _textoTranscritorio = textoFinal;

                        MessageBox.Show("¡Proceso completado!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show("Operación cancelada por el usuario.", "Cancelado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    catch (Exception ex)
                    {
                        // Desenvolver error si viene en AggregateException
                        string msg = ex.InnerException?.Message ?? ex.Message;
                        MessageBox.Show($"Error: {msg}", "Fallo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        // Restaurar el botón (SIEMPRE EN HILO UI)
                        btnConvertirATexto.Enabled = true;
                    }
                }
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
                txtResultadoEncriptado.Text = textoDesencriptado;

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
