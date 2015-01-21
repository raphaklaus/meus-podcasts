using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeusPodcasts.Classes
{
    class FeedPodcast
    {
        public string Nome { get; set; }
        public string URLFeed { get; set; }
        public string URLSite { get; set; }
        public string URLImagem { get; set; }
        public string Descricao { get; set; }
    }

    class FeedEpisodio
    {
        public string Titulo { get; set; }
        public string Descricao { get; set; }
        public string DataPublicacao { get; set; }
        public string URLSite { get; set; }
        public string URLMp3 { get; set; }
        public string URLImagem { get; set; }
        public string Duracao { get; set; }
    }
}
