using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MeusPodcasts.Classes
{
    class Podcast
    {
        public string Nome { get; set; }
        public string URLFeed { get; set; }
        public string URLImagem { get; set; }
        public string URLImagemUltimoEpisodio { get; set; }
        public int QtdEpisodiosNovos { get; set; }
        public string QtdEpisodiosNovosDescricao {
            get 
            {
                if (this.QtdEpisodiosNovos == 1)
                    return "1 episódio novo";
                else if (this.QtdEpisodiosNovos > 1)
                    return this.QtdEpisodiosNovos.ToString() + " episódio(s) novo(s)";
                else
                    return "";
            }
        }
        public string ImagemEpisodiosNovos {
            get 
            {
                if (this.QtdEpisodiosNovos > 0)
                    return "Assets/Images/Podcast/PodcastComNovos.png";
                else
                    return "Assets/Images/Podcast/PodcastSemNovos.png";
            }
        }
        
        public ImageSource Imagem {
            get
            {
                if (String.IsNullOrEmpty(this.URLImagemUltimoEpisodio) == false)
                {
                    return new BitmapImage(new Uri(this.URLImagemUltimoEpisodio));
                }
                else
                {
                    if (String.IsNullOrEmpty(this.URLImagem) == false)
                    {
                        return new BitmapImage(new Uri(this.URLImagem));
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}