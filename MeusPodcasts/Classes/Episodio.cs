using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MeusPodcasts.Classes
{
    class Episodio
    {
        private string p_sDataPublicacao;
        public string Titulo { get; set; }
        public string URLSite { get; set; }
        public string URLMp3 { get; set; }
        public string URLImagem { get; set; }
        public string Duracao { get; set; }
        public string Descricao { get; set; }
        public string Novo { get; set; }

        public string DataPublicacao {
            get
            {
                //return p_sDataPublicacao;
                DateTime objData;
                bool convertido = DateTime.TryParse(p_sDataPublicacao, out objData);
                if (convertido == true)
                    return objData.ToString("dd/MM/yyyy");
                else
                    return p_sDataPublicacao;
            }
            set
            {
                p_sDataPublicacao = value;
            }
        }

        public string ImagemEpisodiosNovos
        {
            get
            {
                if (this.Novo == "S")
                    return "Assets/Images/Podcast/PodcastComNovos.png";
                else
                    return "Assets/Images/Podcast/PodcastSemNovos.png";
            }
        }
        public Object Imagem
        {
            get
            {
                if (String.IsNullOrEmpty(this.URLImagem) == false)
                {
                    return new BitmapImage(new Uri(this.URLImagem));
                }
                else
                {
                    return "Assets/Images/Podcast/Podcast200x200.png";
                }
            }
        }
        
    }
}
