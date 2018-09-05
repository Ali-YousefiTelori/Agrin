using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Agrin.Desktop.Views.Toolbox
{
    public class TabMenuControl : UserControl
    {
        public TabMenuControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
