using System;
using System.Windows;

namespace MessengerGUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Console.WriteLine("Application started");
            base.OnStartup(e);
        }
    }
}