using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MeusPodcasts.Classes
{
    class Sugestao
    {
        public string Nome { get; set; }
        public string URLFeed { get; set; }
        public string URLImagem { get; set; }
        public ImageSource Imagem { get; set; }

        public Sugestao(string v_sNome, string v_sURLFeed, string v_sURLImagem)
        {
            this.Nome = v_sNome;
            this.URLFeed = v_sURLFeed;
            this.URLImagem = v_sURLImagem;
            this.Imagem = new BitmapImage(new Uri(URLImagem));
        }
    }

    class SugestaoManager{
        private List<Sugestao> p_objLista;
        public List<Sugestao> Sugestoes
        {
            get {return p_objLista;}
        }

        public SugestaoManager()
        {
            p_objLista = new List<Sugestao>();
            p_objLista.Add(new Sugestao("GorilaCast", "http://www.gorilapolar.com.br/podcast/feed/GorilaCast.xml", "http://www.gorilapolar.com.br/podcast/feed/GorilaCast.jpg"));
            p_objLista.Add(new Sugestao("Nerdcast", "http://feed.nerdcast.com.br", "http://jovemnerd.ig.com.br/wp-content/themes/default/images/itunes_small.jpg"));
            p_objLista.Add(new Sugestao("No Barquinho", "http://feeds.feedburner.com/nobarquinho", "http://nobarquinho.com/wp-content/uploads/2012/10/logo_nobarquinho_20121011_v10_144x144.png"));
            p_objLista.Add(new Sugestao("RapaduraCast", "http://feeds.feedburner.com/rapaduracast", "http://www.cinemacomrapadura.com.br/rapaduracast/wp-content/uploads/2012/07/rclogogigante.jpg"));
            p_objLista.Add(new Sugestao("Irmãos.com", "http://www.irmaos.com/podcast/programa.rss", "http://www.irmaos.com/podcast/icones/logo.jpg"));
            p_objLista.Add(new Sugestao("Papo de Gordo", "http://papodegordo.mtv.uol.com.br/categoria/podcast/feed/", "http://papodegordo.mtv.uol.com.br/wp-content/uploads/2010/powerpress/bt_pdg_new.jpg"));
            p_objLista.Add(new Sugestao("Bibotalk", "http://feeds.feedburner.com/bibocast", "http://bibotalk.com.br/site/wp-content/uploads/2012/10/itunes144.jpg"));
            //p_objLista.Add(new Sugestao("", "", ""));
        }
    }
}
