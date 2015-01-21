using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using Windows.Storage;
using System.Collections.Specialized;

namespace MeusPodcasts.Classes
{
    class PodcastManager
    {
        public enum EnumAcao { Adicionar, Remover, GravarInformacoesEpisodiosNovos, GravarQtdeEpisodiosNovos }

        public event AoFicarProntoHandler AoFicarPronto;
        public delegate void AoFicarProntoHandler();

        public event AoGravarHandler AoGravar;
        public delegate void AoGravarHandler(Podcast objPodcast, EnumAcao eAcao);

        public event AoLerTodosHandler AoLerTodos;
        public delegate void AoLerTodosHandler();

        private ObservableCollection<Podcast> p_objLista;

        private const string p_sARQUIVOPODCASTS = "podcasts.xml";

        public ObservableCollection<Podcast> ListaPodcasts
        {
            get {return p_objLista;}
        }

        public void Inicializar()
        {
            CriarArquivoXML();
            InicializarObjetoListaPodcast();
        }

        private async void CriarArquivoXML()
        {
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sARQUIVOPODCASTS, CreationCollisionOption.OpenIfExists);
            //forca recriar
            //StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sARQUIVOPODCASTS, CreationCollisionOption.ReplaceExisting);
            objArquivo = null;
            if (AoFicarPronto != null) { AoFicarPronto(); }
        }
        
        private void InicializarObjetoListaPodcast()
        {
            p_objLista = new ObservableCollection<Podcast>();
        }

        public string Adicionar(Podcast v_objPodcast)
        {
            if (VerificarPodcastJaExistente(v_objPodcast) == false)
            {
                return "O podcast informado já existe.";
            }
            p_objLista.Add(v_objPodcast);
            Gravar(v_objPodcast,EnumAcao.Adicionar);
            return "";
        }

        private bool VerificarPodcastJaExistente(Podcast v_objPodcast)
        {
            foreach (Podcast p in p_objLista)
            {
                if (p.Nome.ToUpper() == v_objPodcast.Nome.ToUpper() && p.URLFeed.ToUpper() == v_objPodcast.URLFeed.ToUpper())
                {
                    return false;
                }
            }
            return true;
        }

        public void Remover(Podcast v_objPodcast)
        {
            Podcast p = RetornarPodcastLista(v_objPodcast);
            if (p != null)
            {
                p_objLista.Remove(p);
                Gravar(v_objPodcast, EnumAcao.Remover);
                ExcluirArquivoEpisodiosPodcast(p);
            }
        }

        private async void ExcluirArquivoEpisodiosPodcast(Podcast v_objPodcast)
        {
            //recria o arquivo em branco
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(v_objPodcast.Nome + ".xml", CreationCollisionOption.ReplaceExisting);
            objArquivo = null;
        }

        private Podcast RetornarPodcastLista(Podcast v_objPodcast)
        {
            foreach (Podcast p in p_objLista)
            {
                if (p.Nome.ToLower() == v_objPodcast.Nome.ToLower())
                {
                    return p;
                }
            }
            return null;
        }

        public void GravarInformacaoEpisodiosNovos(Podcast v_objPodcast, int v_iQtdEpisodiosNovos, string v_sUrlImagemNova)
        {
            int i = RetornarPodcastListaIndice(v_objPodcast);
            if (i > -1)
            {
                p_objLista[i].QtdEpisodiosNovos = v_iQtdEpisodiosNovos;
                p_objLista[i].URLImagemUltimoEpisodio = v_sUrlImagemNova;
                Gravar(v_objPodcast, EnumAcao.GravarInformacoesEpisodiosNovos);
            }
        }

        private int RetornarPodcastListaIndice(Podcast v_objPodcast)
        {
            int i = 0;
            foreach (Podcast p in p_objLista)
            {
                if (p.Nome.ToLower() == v_objPodcast.Nome.ToLower())
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        public void GravarQtdeEpisodiosNovos(Podcast v_objPodcast, int iQtde)
        {
            int i = RetornarPodcastListaIndice(v_objPodcast);
            if (i > -1)
            {
                p_objLista[i].QtdEpisodiosNovos = iQtde;
                Gravar(v_objPodcast, EnumAcao.GravarQtdeEpisodiosNovos);
            }
        }

        private async void Gravar(Podcast v_objPodcast, EnumAcao eAcao)
        {
            string sXMLCompleto = RetornarXMLCompleto();
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sARQUIVOPODCASTS, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(objArquivo, sXMLCompleto);
            if (AoGravar != null) { AoGravar(v_objPodcast, eAcao); }
        }

        private string RetornarXMLCompleto()
        {
            XElement objXml = new XElement("podcasts");
            foreach (var objPod in p_objLista)
            {
                XElement objElement = new XElement("item");
                objElement.Add(new XElement("nome", objPod.Nome),
                new XElement("urlfeed", objPod.URLFeed),
                new XElement("urlimagem", objPod.URLImagem),
                new XElement("urlimagemultimoepisodio", objPod.URLImagemUltimoEpisodio),
                new XElement("qtdepisodiosnovos", objPod.QtdEpisodiosNovos.ToString()));
                objXml.Add(objElement);
            }
            return objXml.ToString();
        }

        public async void LerTodos()
        {
            // inicializa lista
            InicializarObjetoListaPodcast();
            // abre arquivo de podcasts
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sARQUIVOPODCASTS, CreationCollisionOption.OpenIfExists);
            string sXMLCompleto = await FileIO.ReadTextAsync(objArquivo);
            //lê o arquivo
            XDocument objXML = new XDocument();
            Stream objStream = await objArquivo.OpenStreamForReadAsync();
            if (objStream.Length > 0)
            {
                objXML = XDocument.Load(objStream);
                //faz uma consulta linq
                var objConsulta = from p in objXML.Elements("podcasts").Elements("item")
                                  select p;
                foreach (var objRegistro in objConsulta)
                {
                    Podcast objPod = new Podcast();
                    objPod.Nome = objRegistro.Element("nome").Value.ToString();
                    objPod.URLFeed = objRegistro.Element("urlfeed").Value.ToString();
                    objPod.URLImagem = objRegistro.Element("urlimagem").Value.ToString();
                    objPod.URLImagemUltimoEpisodio = objRegistro.Element("urlimagemultimoepisodio").Value.ToString();
                    int iQtd = 0;
                    bool blConv = int.TryParse(objRegistro.Element("qtdepisodiosnovos").Value.ToString(), out iQtd);
                    if (blConv==true)
                        objPod.QtdEpisodiosNovos = iQtd;
                    else
                        objPod.QtdEpisodiosNovos = 0;
                    p_objLista.Add(objPod);
                }
            }
            if (AoLerTodos != null) { AoLerTodos(); }
        }
    }
}
