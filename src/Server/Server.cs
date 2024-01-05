namespace Pong.Server;

// Socket Listener acts as a server and listens to the incoming
// messages on the specified port and protocol.
public class PongServer
{
    public static void Main(string[] args)
    {

        _ = new Task(() => new SocketListener().StartServer());

        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(serverOptions =>
            {
                // Set properties and call methods on options
            })
            .UseStartup<Startup>();
        });

        builder.Build().Run();
    }
}
