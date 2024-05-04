using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YoutubeDownloader
{
    public partial class Menu : Form
    {
        private DescargaVideos formDescargaVideos;
        private DescargaMp3 formDescargaMp3;

        public Menu()
        {
            InitializeComponent();
            // Hacer que esta ventana sea un contenedor MDI
            this.IsMdiContainer = true;
            //Abro por defecto el mp3
            AbrirFormGeneral(ref formDescargaMp3);
        }

        private void AbrirFormdeVideo(object sender, EventArgs e)
        {
            AbrirFormGeneral(ref formDescargaVideos);
        }

        private void AbrirFormMp3(object sender, EventArgs e)
        {
            AbrirFormGeneral(ref formDescargaMp3);
        }

        private void AbrirFormGeneral<T>(ref T formulario) where T : Form, new()
        {
            if (formulario == null || formulario.IsDisposed)
            {
                T nuevoFormulario = new T();
                //Dentro del padre
                nuevoFormulario.MdiParent = this;
                //Sin controles de cerrar
                nuevoFormulario.ControlBox = false;
                //Sin bordes
                nuevoFormulario.FormBorderStyle = FormBorderStyle.None;
                // Establecer la ubicación centrada
                nuevoFormulario.StartPosition = FormStartPosition.CenterScreen;
                formulario = nuevoFormulario;
            }

            formulario.Show();
            formulario.BringToFront();
        }
    }
}
