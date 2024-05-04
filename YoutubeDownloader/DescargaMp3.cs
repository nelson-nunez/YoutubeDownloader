using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeDownloader.BaseClass;
using YoutubeDownloader.Extensions;
using YoutubeExplode;

namespace YoutubeDownloader
{
    public partial class DescargaMp3 : Form
    {
        #region Vars
       
        // Set the output directory path here
        string outputDirectory = @"";

        // List of YouTube video URLs to download
        List<Encolado> colaUrls = new List<Encolado>();

        List<DownloadedVideo> downloadscompleted = new List<DownloadedVideo>();

        YoutubeClient youtube = new YoutubeClient();

        SemaphoreSlim semaphore = new SemaphoreSlim(1);


        #endregion

        public DescargaMp3()
        {
            InitializeComponent();
            //Cargando preferencias iniciales de los grids
            dataGridView1.ConfigurarGrids();
            dataGridView2.ConfigurarGrids();

            dataGridView1.Mostrar(colaUrls.EncoladoToView());
            dataGridView2.Mostrar(downloadscompleted.DownloadedVideoToView());
        }

        #region Buttons

        //Boton descarga
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(outputDirectory))
                    SelectDirectory();

                //Inciando hilo descargador
                Thread thread1 = new Thread(new ThreadStart(DescargarVideosAsync));
                thread1.Start();
                thread1.Join();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //Boton encolar
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                AgregarUrl();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        //Boton directorio descargas
        private void button3_Click(object sender, EventArgs e)
        {
            SelectDirectory();
        }

        #endregion

        #region Descarga

        private async void DescargarVideosAsync()
        {
            while (true)
            {
                // Esperar un  tiempo antes de revisar la cola 
                await Task.Delay(1000);
                // Copiar la lista actual de URLs para evitar problemas de concurrencia
                List<Encolado> urlsParaDescargar;
                lock (colaUrls)
                {
                    urlsParaDescargar = new List<Encolado>(colaUrls);
                    colaUrls.Clear();
                }

                foreach (var itemurl in urlsParaDescargar)
                {
                    try
                    {
                        UpdateStatusLabel(label2, $"Descargando {itemurl.Nombre}");
                        await semaphore.WaitAsync();
                        var response = await youtube.DownloadMP3Async(itemurl.Url, outputDirectory);
                        downloadscompleted.Add(response);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error durante la descarga de {itemurl}: {ex.Message}");
                    }
                    finally
                    {
                        semaphore.Release();
                        //Actualizo datos
                        UpdateStatusLabel(label2, $"Completado {itemurl.Nombre}");
                        dataGridView1.Mostrar(colaUrls.EncoladoToView());
                        dataGridView2.Mostrar(downloadscompleted.DownloadedVideoToView());
                    }
                }
                UpdateStatusLabel(label2, $"...");
            }
        }

        private async void AgregarUrl()
        {
            try
            {
                var encolados = await ObtenerUrls();
                lock (colaUrls)
                {
                    foreach (var item in encolados)
                    {
                        if (!colaUrls.Any(x => x.Url == item.Url) && !downloadscompleted.Any(x => x.Url == item.Url))
                            colaUrls.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Mostrar solo los nuevos URLs agregados fuera del bloqueo
                dataGridView1.Mostrar(colaUrls.EncoladoToView());
            }
        }

        private void UpdateStatusLabel(Label label, string message)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action<Label, string>(UpdateStatusLabel), label, message);
                return;
            }

            label.Text = message;
        }

        private void SelectDirectory()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                outputDirectory = folderBrowserDialog1.SelectedPath;
                label5.Text = outputDirectory;
            }
        }

        private async Task<List<Encolado>> ObtenerUrls()
        {
            var encolados = new List<Encolado>();
            //Obtengo texto del portapapeles
            var textocopiado = Clipboard.GetText();
            encolados = await YoutubeClientExtensions.ObtenerURLsYTTitleAsync(textocopiado);
            //Sino obtengo del input
            if (!encolados.Any())
            {
                var texto = Interaction.InputBox("Ingrese la url del video");
                encolados = await YoutubeClientExtensions.ObtenerURLsYTTitleAsync(texto);
            }
            return encolados;
        }

        #endregion
    }
}
