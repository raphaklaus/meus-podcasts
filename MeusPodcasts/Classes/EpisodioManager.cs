using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;

namespace MeusPodcasts.Classes
{
    class EpisodioManager
    {
        public event AoGerarErroHandler AoGerarErro;
        public delegate void AoGerarErroHandler(string v_sErro);

        public event AoFicarProntoHandler AoFicarPronto;
        public delegate void AoFicarProntoHandler(Podcast v_objPodcast);

        public event AoLerTodosHandler AoLerTodos;
        public delegate void AoLerTodosHandler(List<Episodio> v_objLista);

        public event AoGravarEpisodioHandler AoGravarEpisodio;
        public delegate void AoGravarEpisodioHandler(List<Episodio> v_objLista, EnumAcaoEpisodio v_eAcao);

        public event AoObterEpisodiosNovosHandler AoObterEpisodiosNovos;
        public delegate void AoObterEpisodiosNovosHandler(Podcast v_objPodcast, int v_iQtdEpisodiosNovos, string v_sUrlImagemNova);

        private List<Episodio> p_objLista;
        private Podcast p_objPodcast;

        private string p_sNomeArquivoXml = "";

        private FeedManager p_objFeedManager;

        public enum EnumAcaoEpisodio { NovosEpisodiosAdicionados, MarcarComoLido }
        public EpisodioManager(Podcast v_objPodcast)
        {
            p_objPodcast = v_objPodcast;
            p_sNomeArquivoXml = p_objPodcast.Nome + ".xml";
            CriarArquivoXML(v_objPodcast);
            InicializarObjetoFeedManager();
        }

        private void InicializarObjetoFeedManager()
        {
            p_objFeedManager = new FeedManager();
            p_objFeedManager.AoLerFeedEpisodio += new FeedManager.AoLerFeedEpisodioHandler(EventoFeed_AoLerFeedEpisodio);
            p_objFeedManager.AoGerarErro += new FeedManager.AoGerarErroHandler(EventoFeed_AoGerarErro);
        }

        public void MarcarComoLido(Episodio v_objEpisodio)
        {
            int i = RetornarEpisodioListaIndice(v_objEpisodio);
            if (i > -1)
            {
                p_objLista[i].Novo = "N";
                Gravar(EnumAcaoEpisodio.MarcarComoLido);
            }
        }

        private int RetornarEpisodioListaIndice(Episodio v_objEpisodio)
        {
            int i = 0;
            foreach (Episodio p in p_objLista)
            {
                if (p.Titulo.ToLower() == v_objEpisodio.Titulo.ToLower())
                {
                    return i;
                }
                i++;
            }
            return -1;
        }

        private void EventoFeed_AoGerarErro(string v_sErro)
        {
            if (AoGerarErro != null) { AoGerarErro(v_sErro); }
        }

        private void EventoFeed_AoLerFeedEpisodio(List<FeedEpisodio> v_objListaFeedEpisodio)
        {
            string sPrimeiraImagemAdicionada = "";
            int iQtdeEpisodiosNovos = 0;

            Episodio objUltimoEpisodioBaixado = null;
            if (p_objLista.Count > 0)
            {
                objUltimoEpisodioBaixado = p_objLista[0];
            }

            foreach (FeedEpisodio e in v_objListaFeedEpisodio)
            {
                if (objUltimoEpisodioBaixado != null) // primeira vez que baixa qualquer episódio
                {
                    if (e.Titulo.ToLower() == objUltimoEpisodioBaixado.Titulo.ToLower())
                    {
                        break;
                    }
                }
                else
                {
                    iQtdeEpisodiosNovos += 1;
                    if (String.IsNullOrEmpty(sPrimeiraImagemAdicionada) == true)
                        sPrimeiraImagemAdicionada = e.URLImagem;
                    Episodio objNovoEp = new Episodio();
                    objNovoEp.Titulo = e.Titulo;
                    objNovoEp.URLSite = e.URLSite;
                    objNovoEp.URLMp3 = e.URLMp3;
                    objNovoEp.URLImagem = e.URLImagem;
                    objNovoEp.Duracao = e.Duracao;
                    objNovoEp.DataPublicacao = e.DataPublicacao;
                    objNovoEp.Descricao = e.Descricao;
                    objNovoEp.Novo = "S";
                    p_objLista.Add(objNovoEp);
                }
            }
            if (AoObterEpisodiosNovos != null) { AoObterEpisodiosNovos(p_objPodcast, iQtdeEpisodiosNovos, sPrimeiraImagemAdicionada); }
            Gravar(EnumAcaoEpisodio.NovosEpisodiosAdicionados);
        }

        private async void Gravar(EnumAcaoEpisodio eAcao)
        {
            string sXMLCompleto = RetornarXMLCompleto();
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sNomeArquivoXml, CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(objArquivo, sXMLCompleto);
            if (AoGravarEpisodio != null) { AoGravarEpisodio(p_objLista, eAcao); }
        }

        private string RetornarXMLCompleto()
        {
            XElement objXml = new XElement("episodios");
            foreach (var objPod in p_objLista)
            {
                XElement objElement = new XElement("item");
                objElement.Add(new XElement("titulo", objPod.Titulo),
                new XElement("urlsite", objPod.URLSite),
                new XElement("urlmp3", objPod.URLMp3),
                new XElement("urlimagem", objPod.URLImagem),
                new XElement("duracao", objPod.Duracao),
                new XElement("datapublicacao", objPod.DataPublicacao),
                new XElement("descricao", objPod.Descricao),
                new XElement("novo", objPod.Novo));
                objXml.Add(objElement);
            }
            return objXml.ToString();
        }

        private async void CriarArquivoXML(Podcast v_objPodcast)
        {
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sNomeArquivoXml, CreationCollisionOption.OpenIfExists);
            //forca recriar
            //StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sNomeArquivoXml, CreationCollisionOption.ReplaceExisting);
            objArquivo = null;
            if (AoFicarPronto != null) { AoFicarPronto(v_objPodcast); }
        }

        public void Adicionar(Episodio v_objEpisodio)
        {
            p_objLista.Add(v_objEpisodio);
        }

        public void BuscarNovosEpisodios(Podcast v_objPodcast)
        {
            p_objFeedManager.LerFeedEpisodios(v_objPodcast.URLFeed);
        }

        private void InicializarObjetoListaEpisodios()
        {
            p_objLista = new System.Collections.Generic.List<Episodio>();
        }

        public async void LerTodos(Podcast v_objPodcast)
        {
            /// inicializa lista
            InicializarObjetoListaEpisodios();
            // abre arquivo de episodios
            StorageFile objArquivo = await ApplicationData.Current.LocalFolder.CreateFileAsync(p_sNomeArquivoXml, CreationCollisionOption.OpenIfExists);
            string sXMLCompleto = await FileIO.ReadTextAsync(objArquivo);
            //lê o arquivo
            XDocument objXML = new XDocument();
            Stream objStream = await objArquivo.OpenStreamForReadAsync();
            if (objStream.Length > 0)
            {
                objXML = XDocument.Load(objStream);
                //faz uma consulta linq
                var objConsulta = from p in objXML.Elements("episodios").Elements("item")
                                  select p;
                foreach (var objRegistro in objConsulta)
                {
                    Episodio objEp = new Episodio();
                    objEp.Titulo = objRegistro.Element("titulo").Value.ToString();
                    objEp.URLSite = objRegistro.Element("urlsite").Value.ToString();
                    objEp.URLMp3 = objRegistro.Element("urlmp3").Value.ToString();
                    objEp.URLImagem = objRegistro.Element("urlimagem").Value.ToString();
                    objEp.Duracao = objRegistro.Element("duracao").Value.ToString();
                    objEp.DataPublicacao = objRegistro.Element("datapublicacao").Value.ToString();
                    objEp.Descricao = objRegistro.Element("descricao").Value.ToString();
                    objEp.Novo = objRegistro.Element("novo").Value.ToString();
                    p_objLista.Add(objEp);
                }
            }
            if (AoLerTodos != null) { AoLerTodos(p_objLista); }
        }
    }
}
