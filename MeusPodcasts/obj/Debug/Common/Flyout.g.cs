﻿

#pragma checksum "C:\Users\visouza\Documents\Visual Studio 2012\Projects\Win8AppBox\Win8AppBox\Common\Flyout.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F87555FEB1CF1A19362F814036E5B7B8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TCD.Windows8.Controls
{
    partial class Flyout : global::Windows.UI.Xaml.Controls.UserControl, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 111 "..\..\Common\Flyout.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.Popup)(target)).Closed += this.OnPopupClosed;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 132 "..\..\Common\Flyout.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.BackButton_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


