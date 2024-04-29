using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.BaseClass
{
    public class DownloadedVideo
    {
        public string Titulo { get; set; } 
        public DateTime TiempoDescarga { get; set; }
        public string Extension { get; set; } 
        public string Ubicacion { get; set; }
        public string Url { get; set; }

        public DownloadedVideo(string title, string url, string extension, DateTime tiempoDescarga, string ubicacion)
        {
            Titulo = title;  
            Url = url;
            Extension = extension;
            TiempoDescarga = tiempoDescarga;
            Ubicacion = ubicacion;
        }
    }

    public class DownloadedVideoView
    {
        public string Titulo { get; set; }
        public DateTime TiempoDescarga { get; set; }
        public string Extension { get; set; }
        public string Ubicacion { get; set; }

        public DownloadedVideoView(string title, string extension, DateTime tiempoDescarga, string ubicacion)
        {
            Titulo = title;
            Extension = extension;
            TiempoDescarga = tiempoDescarga;
            Ubicacion = ubicacion;
        }
    }
}
