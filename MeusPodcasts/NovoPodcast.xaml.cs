using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MeusPodcasts.Classes;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Syndication;
using Windows.Storage;

namespace MeusPodcasts
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class NovoPodcast : MeusPodcasts.Common.LayoutAwarePage
    {
        //podcast
        private PodcastManager p_objPodManager;
        private Podcast p_objPodcastSendoAdicionado;
        //feed
        private FeedManager p_objFeedManager;
        //outros
        private bool p_blClasseProntaParaUtilizacao = false;
        private bool p_blURLFeedVericada = false;
        private bool p_blTodosLidos = false;

        public NovoPodcast()
        {
            this.InitializeComponent();
            InicializarObjetosTela();
            InicializarObjetoPodcastManager();
            InicializarObjetoFeedManager();
        }

        private void InicializarObjetosTela()
        {
            SugestaoManager objSugManager = new SugestaoManager();
            lsbSugestoes.ItemsSource = objSugManager.Sugestoes;
            stackPanelVerificar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void InicializarObjetoFeedManager()
        {
            p_objFeedManager = new FeedManager();
            //eventos - delegates
            p_objFeedManager.AoLerFeedPodcast += new FeedManager.AoLerFeedPodcastHandler(AoLerFeedPodcast);
            p_objFeedManager.AoGerarErro += new FeedManager.AoGerarErroHandler(AoGerarErroFeed);
        }

        private void AoGerarErroFeed(string v_sErro)
        {
            ExibirMensagem(v_sErro);
            EscreverStatus("");
        }

        private void InicializarObjetoPodcastManager()
        {
            p_objPodManager = new PodcastManager();
            //eventos - delegates
            p_objPodManager.AoFicarPronto += new PodcastManager.AoFicarProntoHandler(Evento_AoFicarPronto);
            p_objPodManager.AoLerTodos += new PodcastManager.AoLerTodosHandler(Evento_AoLerTodos);
            p_objPodManager.AoGravar += new PodcastManager.AoGravarHandler(Evento_AoGravar);
            //inicialização
            p_objPodManager.Inicializar();
        }

        private void AoLerFeedPodcast(FeedPodcast v_objFeedPodcast)
        {
            //configura objeto para posterior adição à biblioteca
            p_objPodcastSendoAdicionado = new Podcast();
            p_objPodcastSendoAdicionado.Nome = v_objFeedPodcast.Nome;
            p_objPodcastSendoAdicionado.URLFeed = v_objFeedPodcast.URLFeed;
            p_objPodcastSendoAdicionado.URLImagem = v_objFeedPodcast.URLImagem;
            //configura stack panel de exibição
            ConfigurarItensStackPanel(v_objFeedPodcast.Nome, v_objFeedPodcast.Descricao, v_objFeedPodcast.URLSite, v_objFeedPodcast.URLImagem, Windows.UI.Xaml.Visibility.Visible);
            p_blURLFeedVericada = true;
            EscreverStatus("");
        }

        private void Evento_AoGravar(Podcast v_objPodcast, PodcastManager.EnumAcao v_eAcao)
        {
            switch (v_eAcao)
            {
                case PodcastManager.EnumAcao.Adicionar:
                    ControlesPosAdicaoPodcasst();
                    break;
            }
        }

        private void ControlesPosAdicaoPodcasst()
        {
            ExibirMensagem("Podcast adicionado com sucesso.");
            stackPanelVerificar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            txtURLFeed.Text = String.Empty;
        }

        private void Evento_AoFicarPronto()
        {
            p_blClasseProntaParaUtilizacao = true;
            p_objPodManager.LerTodos();
        }

        private void Evento_AoLerTodos()
        {
            p_blTodosLidos = true;
        }

        private void EscreverStatus(string v_sStatus)
        {
            txtStatus.Text = v_sStatus.ToString();
        }

        private bool Pronto()
        {
            if (p_blClasseProntaParaUtilizacao == false)
            {
                ExibirMensagem("A aplicação ainda está inicializando seus controles.\nPor favor aguarde.");
                return false;
            }
            return true;
        }

        private bool TodosLidos()
        {
            if (p_blTodosLidos == false)
            {
                ExibirMensagem("A aplicação ainda está lendo os podcasts existentes.\nPor favor aguarde.");
                return false;
            }
            return true;
        }

        private bool URLVerificada()
        {
            if (String.IsNullOrEmpty(txtURLFeed.Text) == true) //significa que foi adicionado através das sugestões
            {
                return true;
            }
            if (p_blURLFeedVericada == false)
            {
                ExibirMensagem("A URL informada ainda não foi verificada.\nClique em 'Verificar' e tente novamente");
                return false;
            }
            return true;
        }

        private async void ExibirMensagem(string v_sMensagem)
        {
            var objMD = new MessageDialog(v_sMensagem);
            objMD.Commands.Add(new UICommand("Ok"));
            IUICommand x = await objMD.ShowAsync();
        }

        private void btnVoltar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        private void btnAdicionarPodcast_Click(object sender, RoutedEventArgs e)
        {
            if (this.Pronto() == false) return;
            if (this.URLVerificada() == false) return;
            string sRet = p_objPodManager.Adicionar(p_objPodcastSendoAdicionado);
            if (String.IsNullOrWhiteSpace(sRet) == false)
            {
                ExibirMensagem(sRet);
            }
        }

        private void VerificarPodcast(string v_sURLFeed)
        {
            if (this.TodosLidos() == false) return;
            if (String.IsNullOrEmpty(v_sURLFeed) == true) return;
            if (this.TratarConexaoInternet() == false) return;
            
            // limpeza
            ConfigurarItensStackPanel("", "", "", "", Windows.UI.Xaml.Visibility.Collapsed);
            p_objPodcastSendoAdicionado = null;
            // status
            EscreverStatus("Aguarde, verificando feed (" + v_sURLFeed + ")");
            // chamada do método assíncrono
            p_objFeedManager.LerFeedPodcast(v_sURLFeed);//é pra continuar mesmo... sem problemas
        }


        private void ConfigurarItensStackPanel(string v_sNome, string v_sDescricao, string v_sURLSite, string v_sURLImagem, Windows.UI.Xaml.Visibility v_eVisibility)
        {
            txbNomePodcast.Text = v_sNome;
            txbDescricaoPodcast.Text = v_sDescricao;
            txbSite.Text = v_sURLSite;
            if (String.IsNullOrEmpty(v_sURLSite) == false)
            {
                hprSite.NavigateUri = new Uri(v_sURLSite);
            }
            if (String.IsNullOrEmpty(v_sNome) == true) //quando é limpeza
            {
                imgPodcast.Source = null;
            }
            else
            {
                if (String.IsNullOrEmpty(v_sURLImagem) == false)
                {
                    imgPodcast.Source = new BitmapImage(new Uri(v_sURLImagem));
                }
                else
                {
                    ConfigurarImagem("Assets\\Images\\Podcast\\Podcast200x200.png");
                }
            }
            stackPanelVerificar.Visibility = v_eVisibility;
        }

        private async void ConfigurarImagem(string v_sCaminhoImagem)
        {
            StorageFile placeholderImage = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(v_sCaminhoImagem);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(await placeholderImage.OpenAsync(FileAccessMode.Read));
            imgPodcast.Source = bitmapImage;
        }

        private void btnVerificar_Click(object sender, RoutedEventArgs e)
        {
            VerificarPodcast(txtURLFeed.Text);
        }

        private void txtURLFeed_TextChanged(object sender, TextChangedEventArgs e)
        {
            p_blURLFeedVericada = false;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }
        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void StackPanelSugestao_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var stack = sender as StackPanel;
            if (stack != null)
            {
                Sugestao sug = stack.DataContext as Sugestao;
                if (sug == null)
                {
                    ExibirMensagem("Falha ao recuperar a sugestão selecionada.");
                }
                else
                {
                    txtURLFeed.Text = "";
                    VerificarPodcast(sug.URLFeed);
                }
            }
            else
            {
                ExibirMensagem("Falha ao recuperar a sugestão selecionada. (2)");
            }
        }

        private bool TratarConexaoInternet()
        {
            try
            {
                EscreverStatus("Verificando conexão com a internet...");
                if (Util.PossuiConexaoInternet() == false)
                {
                    ExibirMensagem("Sem conexão com a internet.");
                    EscreverStatus("");
                    return false;
                }
                return true;
            }
            finally
            {
                EscreverStatus("");
            }
        }
    }
}
