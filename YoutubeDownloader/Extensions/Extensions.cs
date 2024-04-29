using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.BaseClass;

namespace YoutubeDownloader.Extensions
{
    public static class Extensions
    {
        public static List<EncoladoView> EncoladoToView(this List<Encolado> lista)
        {
            var listaview = from a in lista
                            select new EncoladoView(
                                a.Nombre.Length > Constantes.Maxlenght ? a.Nombre.Substring(0, Constantes.Maxlenght) : a.Nombre);
            return listaview.ToList();
        }

        public static List<DownloadedVideoView> DownloadedVideoToView(this List<DownloadedVideo> lista)
        {
            var listaview = lista.Select(a => new DownloadedVideoView(
                a.Titulo.Length > Constantes.Maxlenght ? a.Titulo.Substring(0, Constantes.Maxlenght) : a.Titulo,
                a.Extension,
                a.TiempoDescarga,
                a.Ubicacion)
            );

            return listaview.ToList();
        }
    }
}
