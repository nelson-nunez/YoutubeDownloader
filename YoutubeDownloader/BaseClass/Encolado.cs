using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.BaseClass
{
    public class Encolado
    {
        public string Nombre { get; set; }
        public string Url { get; set; }

        public Encolado(string url, string nombre)
        {
            Url = url;
            Nombre = nombre;
        }
    }

    public class EncoladoView
    {
        public string Titulo { get; set; }

        public EncoladoView(string titulo)
        {
            Titulo = titulo;
        }
    }
}
