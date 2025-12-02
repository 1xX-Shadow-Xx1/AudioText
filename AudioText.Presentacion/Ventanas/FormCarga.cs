using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioText.Presentacion.Ventanas
{
    public partial class FormCarga : Form
    {
        // Evento para avisar al FormPrincipal que se quiere cancelar
        public event EventHandler OnCancelarClick;

        public FormCarga()
        {
            InitializeComponent();
            this.ControlBox = false; // Quitar botón X
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            lblMensaje.Text = "Cancelando operación...";
            btnCancelar.Enabled = false; // Evitar doble clic

            // Disparamos el evento para que el FormPrincipal detenga el proceso
            OnCancelarClick?.Invoke(this, EventArgs.Empty);
        }

        // Método seguro para actualizar la barra desde otro hilo
        public void ActualizarProgreso(int porcentaje)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ActualizarProgreso(porcentaje)));
            }
            else
            {
                // Aseguramos que el valor esté entre 0 y 100
                pbProgreso.Value = Math.Clamp(porcentaje, 0, 100);
            }
        }

        // Método seguro para actualizar el texto desde otro hilo
        public void ActualizarMensaje(string texto)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ActualizarMensaje(texto)));
            }
            else
            {
                // Cortamos el texto si es muy largo para que no rompa el diseño
                if (texto.Length > 50) texto = texto.Substring(0, 47) + "...";
                lblMensaje.Text = texto;
            }
        }
    }
}
