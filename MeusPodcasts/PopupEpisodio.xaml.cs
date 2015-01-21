using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MeusPodcasts
{
    public sealed partial class PopupEpisodio : UserControl
    {
        public string NomePodcast { get; set; }
        public string TituloEpisodio { get; set; }
        public string URLImagem { get; set; }
        public string URLEpisodio { get; set; }
        public string URLMp3 { get; set; }
        public string DescricaoEpisodio { get; set; }
        public string DuracaoEpisodio { get; set; }
        public string DataPublicacao { get; set; }

        public PopupEpisodio()
        {
            this.InitializeComponent();
            /*
            DataTransferManager.GetForCurrentView().DataRequested -= this.ShareTextHandler;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested -= this.ShareTextHandler;
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);
             */
        }

        /*
        private void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = this.NomePodcast;
            request.Data.Properties.Description = this.TituloEpisodio;
            request.Data.Properties.Description = txbEpisodioTitulo.Text;
            request.Data.SetUri(new Uri(this.URLEpisodio));
        }
         */

        public void CarregarInformacoesCampos()
        {
            txbEpisodioTitulo.Text = this.TituloEpisodio;
            if (String.IsNullOrEmpty(this.URLImagem) == false)
            {
                imgEpisodio.Source = new BitmapImage(new Uri(this.URLImagem));
            }
            if (String.IsNullOrEmpty(this.DescricaoEpisodio) == false)
            {
                scvScroll.Visibility = Windows.UI.Xaml.Visibility.Visible;
                txbDescricaoPodcast.Text = this.DescricaoEpisodio;
            }
            else
            {
                scvScroll.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                txbDescricaoPodcast.Text = "";
            }
            if (String.IsNullOrEmpty(this.DuracaoEpisodio)== false)
            {
                txbDuracao.Text = "Duração: " + this.DuracaoEpisodio;
            }
            else
            {
                txbDuracao.Text = "";
            }
            if (String.IsNullOrEmpty(this.DataPublicacao) == false)
            {
                txbPublicacao.Text = "Publicado em " + this.DataPublicacao;
            }
            else
            {
                txbPublicacao.Text = "";
            }
            if (String.IsNullOrEmpty(this.URLEpisodio) == false)
            {
                hplSite.NavigateUri = new Uri(this.URLEpisodio);
                txbSite.Text = this.URLEpisodio;
                //btnCompartilhar.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                hplSite.NavigateUri = null;
                txbSite.Text = "";
                //btnCompartilhar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            bool mp3 = false;
            string extensao = this.URLMp3.ToLower().Substring(this.URLMp3.Length - 3, 3);
            if (String.IsNullOrEmpty(this.URLMp3) == false)
            {
                if (extensao == "mp3" || extensao == "wma")
                    mp3 = true;
            }
            if (mp3 == true)
            {
                btnOuvir.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                ExibirMensagem("O formato de áudio deste episódio (" + extensao + ") não é reconhecido");
                btnOuvir.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async void ExibirMensagem(string v_sMensagem)
        {
            var objMD = new MessageDialog(v_sMensagem);
            objMD.Commands.Add(new UICommand("Ok"));
            IUICommand x = await objMD.ShowAsync();
        }

        private void btnOuvir_Click(object sender, RoutedEventArgs e)
        {
            if (mdePodcast.CurrentState == MediaElementState.Buffering || mdePodcast.CurrentState == MediaElementState.Opening)
                SomParar();
            else if (mdePodcast.CurrentState == MediaElementState.Closed || mdePodcast.CurrentState == MediaElementState.Stopped)
                SomIniciar();
            else if (mdePodcast.CurrentState == MediaElementState.Paused)
                SomContinuar();
            else if (mdePodcast.CurrentState == MediaElementState.Playing)
                SomPausar();
        }

        private void SomPausar()
        {
            mdePodcast.Pause();
            btnOuvir.Content = "Continuar";
        }

        private void SomContinuar()
        {
            mdePodcast.Play();
            btnOuvir.Content = "Pausar";
        }

        private void SomParar()
        {
            mdePodcast.Stop();
            btnOuvir.Content = "Ouvir";
        }

        private void SomIniciar()
        {
            mdePodcast.Source = new Uri(this.URLMp3);
            mdePodcast.MediaFailed += mdePodcast_MediaFailed;
            mdePodcast.MediaOpened += mdePodcast_MediaOpened;
            mdePodcast.MediaEnded += mdePodcast_MediaEnded;
        }

        private void mdePodcast_MediaEnded(object sender, RoutedEventArgs e)
        {
            btnOuvir.Content = "Ouvir";
        }

        private void mdePodcast_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            btnOuvir.Content = "Ouvir";
        }

        private void mdePodcast_MediaOpened(object sender, RoutedEventArgs e)
        {
            btnOuvir.Content = "Pausar";
        }

        private void btnCompartilhar_Click_1(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void EscreverStatus(string v_sStatus)
        {
            txbStatus.Text = v_sStatus.ToString();
        }
    }
}