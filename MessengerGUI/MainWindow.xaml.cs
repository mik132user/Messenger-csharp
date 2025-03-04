using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;

namespace MessengerGUI
{
    public partial class MainWindow : Window
    {
        private HubConnection _connection;
        private string _userName;
        public ObservableCollection<string> Messages { get; set; } = new();

        public MainWindow()
        {
            Console.WriteLine("MainWindow constructor called");
            InitializeComponent();
            MessagesList.ItemsSource = Messages;
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameInput.Text))
            {
                _userName = NameInput.Text;
                Console.WriteLine("User name entered: " + _userName);

                NameInputPanel.Visibility = Visibility.Collapsed;
                ChatPanel.Visibility = Visibility.Visible;

                _connection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5095/chat")
                    .Build();

                _connection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Messages.Add($"{user}: {message}");
                    });
                });

                Console.WriteLine("HubConnection created");
                ConnectToServer();
            }
            else
            {
                MessageBox.Show("Please enter a name.");
            }
        }

        private async void ConnectToServer()
        {
            int retryCount = 0;
            const int maxRetries = 5;

            while (retryCount < maxRetries)
            {
                try
                {
                    Console.WriteLine("Attempting to connect to server...");
                    await _connection.StartAsync();
                    Messages.Add("✅ Connected to server");
                    Console.WriteLine("Connected to server");
                    break;
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Messages.Add($"❌ Error: {ex.Message}, retrying in 5 seconds...");
                    Console.WriteLine($"Connection error: {ex.Message}");
                    await Task.Delay(5000);
                }
            }

            if (retryCount >= maxRetries)
            {
                Messages.Add("❌ Failed to connect to server after multiple attempts.");
                Console.WriteLine("Failed to connect to server after multiple attempts.");
                MessageBox.Show("Failed to connect to the server. Please try again later.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageInput.Text))
            {
                await _connection.InvokeAsync("SendMessage", _userName, MessageInput.Text);
                MessageInput.Text = "";
                MessageInput.Focus();
            }
        }
    }
}