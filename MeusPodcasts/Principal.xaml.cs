using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MeusPodcasts.Classes;
using Windows.UI.ViewManagement;
//using Windows.ApplicationModel.DataTransfer;

namespace MeusPodcasts
{
    public sealed partial class Principal : MeusPodcasts.Common.LayoutAwarePage
    {
        //podcast
        private PodcastManager p_objPodManager;
        private Podcast p_objPodcastSelecionadoLista;
        private bool p_blClasseProntaParaUtilizacao = false;
        private bool p_blTodosLidos = false;
        //episódio
        private EpisodioManager p_objEpiManager;
        private List<Classes.Episodio> p_objListaEpisodios;
        private Classes.Episodio p_objEpisodioSelecionadoLista;
        //Popup
        Popup p_objPopup;
        //outros
        private bool p_blClasseProntaParaUtilizacao_Episodio = false;
        private bool p_blTodosLidos_Episodios = false;

        public Principal()
        {
            this.InitializeComponent();
            InicializarObjetoPodcastManager();
        }

        private void LerTodosPodcasts()
        {
            if (this.Pronto() == false) return;
            EscreverStatus("Atualizando lista de podcasts...");
            p_objPodManager.LerTodos();
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

        private bool Pronto_Episodios()
        {
            if (p_blClasseProntaParaUtilizacao_Episodio == false)
            {
                ExibirMensagem("A aplicação ainda está inicializando seus controles.\nPor favor aguarde.");
                return false;
            }
            return true;
        }

        private bool TodosLidos_Episodios()
        {
            if (p_blTodosLidos_Episodios == false)
            {
                ExibirMensagem("A aplicação ainda está lendo os episódios existentes.\nPor favor aguarde.");
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

        private void InicializarObjetoepisodioManager(Podcast v_objPodcast)
        {
            EscreverStatus("Lendo os podcasts. Por favor aguarde...");
            p_objEpiManager = new EpisodioManager(v_objPodcast);
            //eventos - delegates
            p_objEpiManager.AoFicarPronto += new EpisodioManager.AoFicarProntoHandler(EventoEpisodio_AoFicarPronto);
            p_objEpiManager.AoLerTodos += new EpisodioManager.AoLerTodosHandler(EventoEpisodio_AoLerTodos);
            p_objEpiManager.AoGravarEpisodio += new EpisodioManager.AoGravarEpisodioHandler(EventoEpisodio_AoGravar);
            p_objEpiManager.AoObterEpisodiosNovos += new EpisodioManager.AoObterEpisodiosNovosHandler(EventoEpisodio_AoObterEpisodiosNovos);
            p_objEpiManager.AoGerarErro += new EpisodioManager.AoGerarErroHandler(EventoEpisodio_AoGerarErro);
        }

        private void EventoEpisodio_AoGerarErro(string v_sErro)
        {
            ExibirMensagem(v_sErro);
            EscreverStatus("");
        }

        private void EventoEpisodio_AoObterEpisodiosNovos(Podcast v_objPodcast, int v_iQtdEpisodiosNovos, string v_sUrlImagemNova)
        {
            //quando obtem-se novos episódios, é necessário gravar a informação no objeto de podcasts
            p_objPodManager.GravarInformacaoEpisodiosNovos(v_objPodcast, v_iQtdEpisodiosNovos, v_sUrlImagemNova);
            ConfigurarImagemNenhumPodcastAinda();
        }

        private void EventoEpisodio_AoGravar(List<Classes.Episodio> v_objLista, Classes.EpisodioManager.EnumAcaoEpisodio v_eAcao)
        {
            EscreverStatus("");
            ConfigurarDadosListBoxEpisodios(v_objLista);
            switch (v_eAcao)
            {
                case Classes.EpisodioManager.EnumAcaoEpisodio.NovosEpisodiosAdicionados:
                    LerTodosPodcasts();// atualiza a lista de podcasts para exibir os números de episódios novos e a imagem também
                    break;
                case Classes.EpisodioManager.EnumAcaoEpisodio.MarcarComoLido:
                    //nenhuma ação
                    break;
            }
        }

        private void ConfigurarDadosListBoxEpisodios(List<Classes.Episodio> v_objLista)
        {
            p_objListaEpisodios = v_objLista;
            lvbEpisodios.ItemsSource = null;
            lvbEpisodios.ItemsSource = p_objListaEpisodios;
        }

        private void ConfigurarImagemNenhumPodcastAinda()
        {
            if (p_objListaEpisodios.Count == 0)
                imgNenhumPodcastAinda.Visibility = Windows.UI.Xaml.Visibility.Visible;
            else
                imgNenhumPodcastAinda.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void EventoEpisodio_AoLerTodos(List<Classes.Episodio> v_objLista)
        {
            EscreverStatus("");
            p_blTodosLidos_Episodios = true;
            ConfigurarDadosListBoxEpisodios(v_objLista);
            ConfigurarImagemNenhumPodcastAinda();
        }

        private void EventoEpisodio_AoFicarPronto(Podcast v_objPodcast)
        {
            p_blClasseProntaParaUtilizacao_Episodio = true;
            EscreverStatus("Lendo episódios...");
            p_objEpiManager.LerTodos(v_objPodcast);
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

        private void Evento_AoGravar(Podcast v_objPodcast, PodcastManager.EnumAcao v_eAcao)
        {
            switch (v_eAcao)
            {
                case PodcastManager.EnumAcao.Remover:
                    lvbEpisodios.ItemsSource = null;
                    LerTodosPodcasts();
                    break;
                case PodcastManager.EnumAcao.GravarInformacoesEpisodiosNovos:
                    //nenhuma ação
                    break;
                case PodcastManager.EnumAcao.GravarQtdeEpisodiosNovos:
                    //nenhuma ação
                    break;
            }
        }

        private void Evento_AoFicarPronto()
        {
            p_blClasseProntaParaUtilizacao = true;
            p_objPodManager.LerTodos();
        }

        private void Evento_AoLerTodos()
        {
            p_blTodosLidos = true;
            ConfigurarDadosListBoxPodcasts();
            EscreverStatus("");
        }

        private void ConfigurarDadosListBoxPodcasts()
        {
            lvbPodcasts.ItemsSource = null;
            lvbPodcasts.ItemsSource = p_objPodManager.ListaPodcasts;
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


        private void StackPanelPodcasts_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var stack = sender as StackPanel;
            if (stack != null)
            {
                Podcast pod = stack.DataContext as Podcast;
                if (pod == null)
                {
                    ExibirMensagem("Falha ao recuperar o podcast selecionado.");
                }
                else
                {
                    p_objPodcastSelecionadoLista = pod;
                    InicializarObjetoepisodioManager(p_objPodcastSelecionadoLista);
                }
            }
            else
            {
                ExibirMensagem("Falha ao recuperar o podcast selecionado. (2)");
            }
        }

        private void StackPanelEpisodio_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var stack = sender as StackPanel;
            if (stack != null)
            {
                Classes.Episodio epi = stack.DataContext as Classes.Episodio;
                if (epi == null)
                {
                    ExibirMensagem("Falha ao recuperar o episódio selecionado.");
                }
                else
                {
                    p_objEpisodioSelecionadoLista = epi;
                    ExibirPopupEpisodio();
                }
            }
            else
            {
                ExibirMensagem("Falha ao recuperar o episódio selecionado. (2)");
            }
        }

        private void ExibirPopupEpisodio()
        {
            if (p_objPopup != null)
            {
                p_objPopup.IsOpen = false;
                p_objPopup = null;
            }
            p_objPopup = new Popup();
            PopupEpisodio objControl = new PopupEpisodio();

            //Height="450" Width="900" (tamanhos do popup)
            p_objPopup.HorizontalOffset = (Window.Current.Bounds.Width / 2) - (450);
            p_objPopup.VerticalOffset = (Window.Current.Bounds.Height / 2) - (225);

            p_objPopup.Child = objControl;

            objControl.NomePodcast = p_objPodcastSelecionadoLista.Nome;
            objControl.TituloEpisodio = p_objEpisodioSelecionadoLista.Titulo;
            objControl.URLEpisodio = p_objEpisodioSelecionadoLista.URLSite;
            objControl.URLMp3 = p_objEpisodioSelecionadoLista.URLMp3;
            objControl.URLImagem = p_objEpisodioSelecionadoLista.URLImagem;
            objControl.DuracaoEpisodio = p_objEpisodioSelecionadoLista.Duracao;
            objControl.DataPublicacao = p_objEpisodioSelecionadoLista.DataPublicacao;
            objControl.CarregarInformacoesCampos();

            var transitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection();
            transitions.Add(new Windows.UI.Xaml.Media.Animation.PopupThemeTransition() { FromHorizontalOffset = 0, FromVerticalOffset = 100 });
            p_objPopup.ChildTransitions = transitions;
            p_objPopup.IsOpen = true;

            objControl.imgVoltar.Tapped += (s, args) =>
            {
                p_objPopup.IsOpen = false;
            };

            ExecutarTarefasDeAberturaDeEpisodio();
        }

        private void ExecutarTarefasDeAberturaDeEpisodio()
        {
            //marcar o episódio como lido
            p_objEpiManager.MarcarComoLido(p_objEpisodioSelecionadoLista);
            //gravar a quantidade de episódios novos no podcast
            int iQtde = RetornarQuantidadeEpisodiosNovosPodcastAtual();
            p_objPodManager.GravarQtdeEpisodiosNovos(p_objPodcastSelecionadoLista, iQtde);
        }

        private int RetornarQuantidadeEpisodiosNovosPodcastAtual()
        {
            int i = 0;
            foreach (Classes.Episodio ep in p_objListaEpisodios)
            {
                if (ep.Novo == "S") i++;
            }
            return i;
        }

        private void btnPodcastAdicionar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(NovoPodcast), null);
        }

        private async void btnPodcastRemover_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (lvbPodcasts.Items.Count == 0) return;
            if (p_objPodcastSelecionadoLista == null)
            {
                ExibirMensagem("Selecione um podcast na lista.");
                return;
            }
            var objMD = new MessageDialog("Deseja realmente apagar o podcast selecionado?");
            objMD.Commands.Add(new UICommand("Sim", (UICommandInvokedHandler) =>
            {
                ApagarPodcast();
            }));
            objMD.Commands.Add(new UICommand("Não"));
            IUICommand x = await objMD.ShowAsync();
        }

        private void ApagarPodcast()
        {
            if (this.Pronto() == false) return;
            if (this.TodosLidos() == false) return;
            p_objPodManager.Remover(p_objPodcastSelecionadoLista);
            p_objPodcastSelecionadoLista = null;
        }

        private void btnPodcastAtualizar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LerTodosPodcasts();
        }

        private void btnEpisodioAtualizar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BuscarNovosEpisodiosNoFeed();
        }

        private void BuscarNovosEpisodiosNoFeed()
        {
            if (lvbPodcasts.Items.Count == 0) return;
            if (p_objPodcastSelecionadoLista == null) return;
            if (this.Pronto_Episodios() == false) return;
            if (TratarConexaoInternet() == false) return;
            EscreverStatus("Procurando por novos episódios no feed. Por favor aguarde...");
            p_objEpiManager.BuscarNovosEpisodios(p_objPodcastSelecionadoLista);
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