using MovieBase.ClientLib;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieBase.Wpf;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MessageClient? messageClient;

    public MainWindow()
    {
        InitializeComponent();
        InitializeMessaging();
    }

    private async void InitializeMessaging()
    {
        messageClient = new MessageClient();
        if(await messageClient.Initialize())
        {
            messageClient.MessageReceived += UpdateStatus;
            UpdateStatus("Connection open");
            await Task.Delay(3000);
            messageClient.SendMessage("Hello from Wpf");
        }
    }

    private void UpdateStatus(string message)
    {
        Dispatcher.Invoke(new Action(() => status.Text = $"{DateTime.Now}: {message}"));
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var client = new MoviesClient(new HttpClient { BaseAddress = new Uri("https://localhost:7184/movies/", UriKind.Absolute) });
        var movies = await client.GetMovies();
        this.list.ItemsSource = movies;
    }
}