using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.Write("Enter your name: ");
        string userName = Console.ReadLine();

        if (string.IsNullOrEmpty(userName))
        {
            userName = "Client";
        }

        string serverUrl = "http://localhost:5095/chat";
        var connection = new HubConnectionBuilder()
            .WithUrl(serverUrl)
            .Build();

        connection.On<string, string>("ReceiveMessage", (user, message) =>
        {
            Console.WriteLine();
            Console.WriteLine($"{user}: {message}");
            Console.Write("Enter a message: ");
        });

        await ConnectWithRetryAsync(connection);

        Console.WriteLine($"Connected to the server as {userName}! Type 'exit' to quit.");
        Console.Write("Enter a message: ");

        var inputTask = HandleInputAsync(connection, userName);
        var receiveTask = ReceiveLoop();

        await Task.WhenAll(inputTask, receiveTask);
    }

    static async Task ConnectWithRetryAsync(HubConnection connection)
    {
        while (true)
        {
            try
            {
                await connection.StartAsync();
                Console.WriteLine("✅ Successfully connected!");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection failed: {ex.Message}. Retrying in 5 seconds...");
                await Task.Delay(5000);
            }
        }
    }

    static async Task HandleInputAsync(HubConnection connection, string userName)
    {
        while (true)
        {
            var message = Console.ReadLine();

            if (message?.ToLower() == "exit")
            {
                await connection.StopAsync();
                break;
            }

            await connection.InvokeAsync("SendMessage", userName, message);
        }
    }

    static async Task ReceiveLoop()
    {
        while (true)
        {
            await Task.Delay(100);
        }
    }
}
