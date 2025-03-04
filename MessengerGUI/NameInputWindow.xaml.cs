using System;
using System.Windows;

namespace MessengerGUI
{
    public partial class NameInputWindow : Window
    {
        public string UserName { get; private set; } = string.Empty;

        public NameInputWindow()
        {
            InitializeComponent();
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameInput.Text))
            {
                UserName = NameInput.Text;
                Console.WriteLine("User name entered: " + UserName);
                this.DialogResult = true; 
                Console.WriteLine("DialogResult set to true");
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a name.");
            }
        }
    }
}