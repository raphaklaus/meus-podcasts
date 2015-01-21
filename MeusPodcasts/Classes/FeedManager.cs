using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using Windows.Data.Xml.Dom;

namespace MeusPodcasts.Classes
{
    class FeedManager
    {
        public event AoGerarErroHandler AoGerarErro;
        public delegate void AoGerarErroHandler(string v_sErro);

        public event AoLerFeedPodcastHandler AoLerFeedPodcast;
        public delegate void AoLerFeedPodcastHandler(FeedPodcast v_objFeedPodcast);

        public event AoLerFeedEpisodioHandler AoLerFeedEpisodio;
        public delegate void AoLerFeedEpisodioHandler(List<FeedEpisodio> v_objLista);

        private List<FeedEpisodio> p_objListaFeedEpisodio;

        public async Task LerFeedPodcast(string v_sURLFeed)
        {
            try
            {
                SyndicationClient objSyndcationClient = new SyndicationClient();
                Uri objFeedUri = new Uri(v_sURLFeed);
                SyndicationFeed objSyndicationFeed = await objSyndcationClient.RetrieveFeedAsync(objFeedUri);
                FeedPodcast objFeedPodcast = new FeedPodcast();

                objFeedPodcast.Nome = objSyndicationFeed.Title.Text;
                objFeedPodcast.URLFeed = v_sURLFeed;
                if (objSyndicationFeed.Subtitle != null && String.IsNullOrEmpty(objSyndicationFeed.Subtitle.Text) == false)
                    objFeedPodcast.Descricao = objSyndicationFeed.Subtitle.Text;
                if (objSyndicationFeed.Links[0] != null && String.IsNullOrEmpty(objSyndicationFeed.Links[0].NodeValue) == false)
                    objFeedPodcast.URLSite = objSyndicationFeed.Links[0].NodeValue;
                if (String.IsNullOrEmpty(objSyndicationFeed.ImageUri.ToString()) == false)
                    objFeedPodcast.URLImagem = objSyndicationFeed.ImageUri.ToString();
                if (AoLerFeedPodcast != null) { AoLerFeedPodcast(objFeedPodcast); }
            }
            catch (Exception e)
            {
                if (AoGerarErro != null) { AoGerarErro("Erro ao ler feed do podcast.\n" + e.ToString()); }
            }
        }

        public async void LerFeedEpisodios(string v_sURLFeed)
        {
            try
            {
                p_objListaFeedEpisodio = new List<FeedEpisodio>();
                SyndicationClient objSyndcationClient = new SyndicationClient();
                Uri objFeedUri = new Uri(v_sURLFeed);
                SyndicationFeed objSyndicationFeed = await objSyndcationClient.RetrieveFeedAsync(objFeedUri);
                FeedPodcast objFeedPodcast = new FeedPodcast();

                foreach (SyndicationItem item in objSyndicationFeed.Items)
                {
                    FeedEpisodio objFeedEpi = new FeedEpisodio();
                    objFeedEpi.Titulo = item.Title.Text;
                    objFeedEpi.DataPublicacao = item.PublishedDate.DateTime.ToString();
                    if (objSyndicationFeed.SourceFormat == SyndicationFormat.Atom10)
                    {
                        objFeedEpi.Descricao = item.Content.Text;
                        objFeedEpi.URLSite = "";
                    }
                    if (objSyndicationFeed.SourceFormat == SyndicationFormat.Rss20)
                    {
                        IXmlNode xmlNodeSelect;
                        XmlDocument xmlDoc = item.GetXmlDocument(SyndicationFormat.Rss20);
                        IXmlNode xmlNodeItem = xmlDoc.GetElementsByTagName("item")[0];
                        //link
                        if (item != null)
                        {
                            if (item.Links[0] != null)
                                objFeedEpi.URLSite = item.Links[0].Uri.ToString();
                        }
                        //descrição
                        xmlNodeSelect = xmlNodeItem.SelectSingleNode("description");
                        if (xmlNodeSelect != null)
                        {
                            if (xmlNodeSelect.ChildNodes[0] != null)
                            {
                                if (xmlNodeSelect.ChildNodes[0].NodeValue != null)
                                    objFeedEpi.Descricao = xmlNodeSelect.ChildNodes[0].NodeValue.ToString();
                            }
                        }

                        // mp3
                        xmlNodeSelect = xmlNodeItem.SelectSingleNode("enclosure");
                        if (xmlNodeSelect != null)
                        {
                            if (xmlNodeSelect.Attributes[0] != null)
                            {
                                if (xmlNodeSelect.Attributes[0].NodeValue != null)
                                    objFeedEpi.URLMp3 = xmlNodeSelect.Attributes[0].NodeValue.ToString();
                            }
                        }
                        // para testes:
                        //string x = xmlNodeItem.GetXml();

                        if (xmlNodeItem != null)
                        {
                            int iQtd = xmlNodeItem.ChildNodes.Count;
                            for (int i = 0; i < iQtd; i++)
                            {
                                // duração
                                if (xmlNodeItem.ChildNodes[i].NodeName.ToLower().ToString() == "duration")
                                {
                                    if (xmlNodeItem.ChildNodes[i] != null)
                                    {
                                        if (xmlNodeItem.ChildNodes[i].ChildNodes[0] != null)
                                            objFeedEpi.Duracao = xmlNodeItem.ChildNodes[i].ChildNodes[0].NodeValue.ToString();
                                    }
                                }
                                // imagem
                                if (xmlNodeItem.ChildNodes[i].NodeName.ToLower().ToString() == "image")
                                {
                                    if (xmlNodeItem.ChildNodes[i] != null)
                                    {
                                        if (xmlNodeItem.ChildNodes[i].Attributes[0] != null)
                                            objFeedEpi.URLImagem = xmlNodeItem.ChildNodes[i].Attributes[0].NodeValue.ToString();
                                    }
                                }
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(objFeedEpi.URLMp3) == false) // previne de adicionar o que não é podcast
                    {
                        p_objListaFeedEpisodio.Add(objFeedEpi);
                    }
                }
                if (AoLerFeedEpisodio != null) { AoLerFeedEpisodio(p_objListaFeedEpisodio); }
            }
            catch (Exception e)
            {
                if (AoGerarErro != null) { AoGerarErro("Erro ao ler feed de episódios podcast.\n" + e.ToString()); }
            }
        }

    }
}