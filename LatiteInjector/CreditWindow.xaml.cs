using LatiteInjector.Properties;
using LatiteInjector.Utils;
using System.Windows;
using System.Windows.Input;

namespace LatiteInjector
{
    /// <summary>
    /// Interaction logic for CreditWindow.xaml
    /// </summary>
    public partial class CreditWindow
    {
        public CreditWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            if (!Settings.Default.DiscordPresence) return;

            if (!Injector.IsMinecraftRunning()) DiscordPresence.IdlePresence();
            else DiscordPresence.PlayingPresence();
        }

        private void Window_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}
