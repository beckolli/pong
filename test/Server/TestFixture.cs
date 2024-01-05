using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Pong.Server.Test
{
    public class TestFixture : IDisposable
    {
        // readonly string _ipAddress = "127.0.0.1";
        // readonly string _uri = "ws://127.0.0.1:11000";

        public HttpClient HttpClient;
        public WebSocketClient WebSocketClient;

        public TestFixture()
        {
            IWebHostBuilder builder = new WebHostBuilder().UseStartup<Startup>();
            TestServer testServer = new(builder);
            HttpClient = testServer.CreateClient();
            WebSocketClient = testServer.CreateWebSocketClient();

        }

        public virtual void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}