﻿

#pragma checksum "C:\Users\Fellipe\Desktop\MeusPodcasts\MeusPodcasts\Principal.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "381F550EF8BF868763A81AC04BDD34FC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MeusPodcasts
{
    partial class Principal : global::MeusPodcasts.Common.LayoutAwarePage, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 249 "..\..\Principal.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerReleased += this.StackPanelEpisodio_PointerReleased;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 214 "..\..\Principal.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.btnEpisodioAtualizar_Tapped;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 177 "..\..\Principal.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).PointerReleased += this.StackPanelPodcasts_PointerReleased;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 142 "..\..\Principal.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.btnPodcastAdicionar_Tapped;
                 #line default
                 #line hidden
                break;
            case 5:
                #line 143 "..\..\Principal.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.btnPodcastRemover_Tapped;
                 #line default
                 #line hidden
                break;
            case 6:
                #line 144 "..\..\Principal.xaml"
                ((global::Windows.UI.Xaml.UIElement)(target)).Tapped += this.btnPodcastAtualizar_Tapped;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

