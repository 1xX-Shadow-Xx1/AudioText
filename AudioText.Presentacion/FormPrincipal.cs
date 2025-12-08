using AudioText.Helpers;
using AudioText.Interfaces;
using AudioText.Presentacion.Ventanas;
using AudioText.Services;

namespace AudioText.Presentacion
{
    /// <summary>
    /// Formulario principal de la aplicación AudioText.
    /// Actúa como el controlador central (Orquestador) que coordina la interacción entre la interfaz de usuario (UI)
    /// y los servicios de backend (Procesamiento de Audio, Encriptación, Conversión Binaria).
    /// </summary>
    public partial class FormPrincipal : Form
    {
        // Definición de dependencias utilizando Interfaces para desacoplar la implementación concreta.
        // Esto facilita el cambio de motores (ej. de Whisper a Gemini) sin modificar el resto del código (Principio DIP).
        private IProcesadorAudio _procesadorAudio;
        private readonly IEncriptador _encriptador;
        private IBinarioHexadecimalService _servicioBinarioHexadecimal;

        // Variables de estado para almacenar temporalmente los resultados de las operaciones en memoria.
        private string _textoTranscritorio = string.Empty; // Almacena el resultado de la transcripción (Speech-to-Text).
        private string _textoMaquina = string.Empty;       // Almacena la representación binaria/hexadecimal.
        private string _textoEncriptado = string.Empty;    // Almacena el texto cifrado.

        /// <summary>
        /// Constructor del formulario. Inicializa los componentes visuales y las dependencias por defecto.
        /// </summary>
        public FormPrincipal()
        {
            InitializeComponent();

            // Configuración inicial de los controles de selección (ComboBox).
            // Establecemos el índice 0 para asegurar que siempre haya una opción válida seleccionada al inicio.
            ConfigOpcionConverter.SelectedIndex = 0;
            ConfigOpcionConverterBinaryHex.SelectedIndex = 0;

            // Inicialización de Servicios (Simulación de Inyección de Dependencias Manual).
            // Por defecto, iniciamos con Whisper (Offline) y AES.
            this._procesadorAudio = new WhisperProcessorService();
            this._encriptador = new AesTextEncryptor();
            this._servicioBinarioHexadecimal = new BinarioService();
        }

        // --- SECCIÓN: GESTIÓN DE ARCHIVOS ---

        /// <summary>
        /// Maneja el evento de clic para seleccionar un archivo de audio.
        /// Abre un cuadro de diálogo nativo de Windows filtrado para archivos de audio compatibles.
        /// </summary>
        private void btnSeleccionarAudio_Click(object sender, EventArgs e)
        {
            // Configuramos el filtro para sugerir formatos compatibles con nuestros servicios (MP3, WAV).
            ofdSeleccionarAudio.Filter = "Archivos de Audio (*.mp3;*.wav)|*.mp3;*.wav|Todos los archivos (*.*)|*.*";
            ofdSeleccionarAudio.Title = "Seleccionar Archivo de Audio (MP3 o WAV)";

            if (ofdSeleccionarAudio.ShowDialog() == DialogResult.OK)
            {
                txtRutaArchivo.Text = ofdSeleccionarAudio.FileName;

                // Limpieza de Estado:
                // Al cargar un nuevo archivo, debemos limpiar los resultados de operaciones anteriores
                // para evitar inconsistencias entre el archivo seleccionado y los datos mostrados.
                txtResultadoTexto.Clear();
                txtResultadoEncriptado.Clear();
                txtResultadoTextoMaquina.Clear();
                _textoTranscritorio = string.Empty;
                _textoMaquina = string.Empty;

                MessageBox.Show($"Archivo cargado exitosamente: {Path.GetFileName(ofdSeleccionarAudio.FileName)}", "Archivo Seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- SECCIÓN: TRANSCRIPCIÓN DE AUDIO (CORE) ---

        /// <summary>
        /// Inicia el proceso de conversión de Audio a Texto.
        /// Este método maneja la lógica de UI, validación y orquestación de hilos en segundo plano.
        /// </summary>
        private async void btnConvertirATexto_Click(object sender, EventArgs e)
        {
            // 1. Validación de Entrada
            if (string.IsNullOrEmpty(txtRutaArchivo.Text) || !File.Exists(txtRutaArchivo.Text))
            {
                MessageBox.Show("Por favor, seleccione un archivo de audio válido antes de continuar.", "Archivo Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Selección de Estrategia (Strategy Pattern)
            // Determinamos qué servicio de procesamiento usar basado en la selección del usuario.
            string opcion = ConfigOpcionConverter.Text;

            if (opcion.Contains("Whisper"))
            {
                this._procesadorAudio = new WhisperProcessorService();
            }
            else if (opcion.Contains("Gemini"))
            {
                this._procesadorAudio = new GeminiProcessorService();
            }

            // 3. Preparación de UI y Estado
            string rutaOriginal = txtRutaArchivo.Text;
            btnConvertirATexto.Enabled = false; // Bloqueamos el botón para evitar múltiples clics (Reentrancia).
            txtResultadoTexto.Clear();
            txtResultadoEncriptado.Clear();
            _textoTranscritorio = string.Empty;

            // Instanciamos la ventana de carga modal.
            using (var formCarga = new FormCarga())
            {
                try
                {
                    // 4. Ejecución en Segundo Plano (Background Thread)
                    // Usamos Task.Run para mover el trabajo pesado fuera del hilo de la UI y evitar que la ventana se congele ("No responde").
                    Task<string> tareaProcesamiento = Task.Run(async () =>
                    {
                        try
                        {
                            // IMPORTANTE: Dentro de este bloque NO se debe acceder a controles de UI directamente.
                            string rutaProcesar = rutaOriginal;

                            // Comunicación segura con la ventana de carga (Thread-Safe).
                            formCarga.ActualizarMensaje("Verificando integridad del archivo...");
                            formCarga.ActualizarProgreso(5);

                            // 5. Pre-procesamiento / Normalización de Audio
                            if (opcion.Contains("Whisper"))
                            {
                                formCarga.ActualizarMensaje("Optimizando audio para Whisper (WAV 16kHz)...");
                                // Whisper requiere un formato específico para funcionar óptimamente.
                                rutaProcesar = await Task.Run(() =>
                                   AudioConverterHelper.PrepararAudioParaProcesamiento(rutaOriginal,
                                   Path.Combine(Path.GetTempPath(), "temp_whisper_16k.wav")));
                            }
                            else
                            {
                                // Para Gemini, verificamos si es necesario convertir formatos no estándar.
                                string ext = Path.GetExtension(rutaProcesar).ToLower();
                                if (ext != ".mp3" && ext != ".wav" && ext != ".m4a")
                                {
                                    formCarga.ActualizarMensaje("Normalizando formato de audio...");
                                    rutaProcesar = await Task.Run(() =>
                                       AudioConverterHelper.PrepararAudioParaProcesamiento(rutaOriginal,
                                       Path.Combine(Path.GetTempPath(), "temp_universal.wav")));
                                }
                            }

                            // 6. Transcripción
                            formCarga.ActualizarMensaje($"Ejecutando motor de IA ({opcion})...");

                            // Configuración de reportadores de progreso.
                            var progTexto = new Progress<string>(t => { /* Opcional: Streaming de texto en tiempo real */ });
                            var progPorc = new Progress<int>(p => formCarga.ActualizarProgreso(p));

                            return await _procesadorAudio.ConvertirATextoAsync(rutaProcesar, progTexto, progPorc);
                        }
                        finally
                        {
                            // Aseguramos que la ventana de carga se cierre al terminar, ocurra error o no.
                            formCarga.Invoke(new Action(() => formCarga.Close()));
                        }
                    });

                    // Mostramos la ventana de carga de forma Modal (bloquea interacción con la ventana principal).
                    formCarga.ShowDialog(this);

                    // ---------------------------------------------------------
                    // RETORNO AL HILO DE UI (Contexto Seguro)
                    // ---------------------------------------------------------

                    // Esperamos el resultado final. Si hubo excepción en el Task, se lanzará aquí.
                    string textoFinal = await tareaProcesamiento;

                    // Actualizamos la UI con el resultado.
                    txtResultadoTexto.Text = textoFinal;
                    _textoTranscritorio = textoFinal;

                    MessageBox.Show("¡La transcripción se ha completado exitosamente!", "Proceso Finalizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Manejo robusto de errores: Extraemos el mensaje real si está envuelto.
                    string msg = ex.InnerException?.Message ?? ex.Message;
                    MessageBox.Show($"Ocurrió un error durante el proceso:\n{msg}", "Error de Procesamiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Restauramos el estado de la UI (siempre se ejecuta).
                    btnConvertirATexto.Enabled = true;
                }
            }
        }

        // --- SECCIÓN: SEGURIDAD (ENCRIPTACIÓN) ---

        /// <summary>
        /// Encripta el texto transcrito utilizando el algoritmo seleccionado (AES).
        /// </summary>
        private void btnEncriptar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_textoTranscritorio))
            {
                MessageBox.Show("No hay texto para encriptar. Por favor, realice una transcripción primero.", "Falta Texto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Delegamos la lógica criptográfica al servicio _encriptador.
                string textoCifrado = _encriptador.Encriptar(_textoTranscritorio);
                
                // Actualizamos estado y UI.
                _textoEncriptado = textoCifrado;
                txtResultadoEncriptado.Text = textoCifrado;
                
                MessageBox.Show("El texto ha sido encriptado y protegido correctamente.", "Encriptación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al intentar encriptar: {ex.Message}", "Fallo de Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Intenta desencriptar el texto cifrado para verificar la reversibilidad del proceso.
        /// </summary>
        private void btnDesencriptar_Click(object sender, EventArgs e)
        {
            string textoCifrado = _textoEncriptado;

            if (string.IsNullOrEmpty(textoCifrado))
            {
                MessageBox.Show("No hay contenido encriptado para procesar.", "Acción Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string textoDesencriptado = _encriptador.Desencriptar(textoCifrado);

                // Verificación de redundancia visual.
                if (textoDesencriptado == txtResultadoEncriptado.Text)
                {
                    MessageBox.Show("El texto ya se encuentra desencriptado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                    return;
                }

                // Restauramos el texto original en la caja correspondiente para validación visual.
                txtResultadoEncriptado.Text = textoDesencriptado;

                MessageBox.Show("El texto original ha sido recuperado exitosamente.", "Desencriptación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo desencriptar el texto. Posible corrupción de datos o clave incorrecta.\nDetalle: {ex.Message}", "Error de Desencriptación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- SECCIÓN: ANÁLISIS BINARIO/HEXADECIMAL ---

        /// <summary>
        /// Inicia la extracción y visualización del código máquina (Binario o Hexadecimal) del archivo.
        /// </summary>
        private async void btnVerBinario_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtRutaArchivo.Text) || !File.Exists(txtRutaArchivo.Text))
            {
                MessageBox.Show("Debe seleccionar un archivo válido para analizar.", "Archivo Requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Configuración dinámica del servicio según selección del usuario.
            int opcion = ConfigOpcionConverter.SelectedIndex;

            if (opcion == 0)
            {
                this._servicioBinarioHexadecimal = new BinarioService();
            }
            else if (opcion == 1)
            {
                this._servicioBinarioHexadecimal = new HexadecimalConverterService();
            }

            using (var formCarga = new FormCarga())
            {
                try
                {
                    btnVerBinarioHex.Enabled = false;
                    txtResultadoTextoMaquina.Clear();
                    _textoMaquina = string.Empty;

                    // Ejecución asíncrona para lectura de archivos grandes.
                    Task<string> tareaHex = Task.Run(async () =>
                    {
                        try
                        {
                            formCarga.ActualizarMensaje("Analizando estructura del archivo...");
                            var progPorc = new Progress<int>(p => formCarga.ActualizarProgreso(p));

                            return await _servicioBinarioHexadecimal.ObtenerConvercionDelArchivoAsync(txtRutaArchivo.Text, progPorc);
                        }
                        finally
                        {
                            formCarga.Invoke(new Action(() => formCarga.Close()));
                        }
                    });

                    formCarga.ShowDialog(this);

                    string codigoMaquina = await tareaHex;

                    // Optimización de rendimiento UI:
                    // Si el resultado es masivo, truncamos la visualización para evitar que la interfaz se congele al renderizar el texto.
                    // Sin embargo, mantenemos el resultado completo en memoria (_textoMaquina) por si se necesita procesar.
                    if (codigoMaquina.Length > 50000)
                    {
                        txtResultadoTextoMaquina.Text = "⚠️ VISTA PREVIA LIMITADA (Archivo Grande)\r\n" +
                                                 "Mostrando los primeros 50,000 caracteres:\r\n\r\n" +
                                                 codigoMaquina.Substring(0, 50000) + "...";
                    }
                    else
                    {
                        txtResultadoTextoMaquina.Text = codigoMaquina;
                    }

                    _textoMaquina = codigoMaquina;

                    MessageBox.Show("Análisis de código máquina completado.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error durante el análisis: {ex.Message}", "Fallo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnVerBinarioHex.Enabled = true;
                }
            }
        }
    }
}
