using System;
using System.Windows;
using System.Windows.Interop;

namespace WorldMap.Helper
{
    public class FullScreenCommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool isFullScreen = (bool.TryParse(parameter.ToString(), out isFullScreen)) ? isFullScreen : true;

            return Application.Current.Host.Content.IsFullScreen != isFullScreen;
        }

        public void Execute(object parameter)
        {
            bool isFullScreen = (bool.TryParse(parameter.ToString(), out isFullScreen)) ? isFullScreen : true;

            Application.Current.Host.Content.FullScreenOptions = FullScreenOptions.StaysFullScreenWhenUnfocused;
            Application.Current.Host.Content.IsFullScreen = isFullScreen;
        }

        public FullScreenCommand()
        {
            Application.Current.Host.Content.FullScreenChanged += (s, e) =>
            {
                EventHandler handler = CanExecuteChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            };
        }

    }
}
