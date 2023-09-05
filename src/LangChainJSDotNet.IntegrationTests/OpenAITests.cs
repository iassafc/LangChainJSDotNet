using Microsoft.Extensions.Configuration;
using Xunit;

namespace LangChainJSDotNet.IntegrationTests
{
    public class OpenAITests
    {
        private IConfiguration Configuration { get; set; }

        private static HttpClient? _httpClient;

        public OpenAITests()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .AddUserSecrets<OpenAITests>();

            Configuration = builder.Build();

            // Allow self signed certificates for integration testing with mock API
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _httpClient = new HttpClient(httpClientHandler);
        }

        [Theory]
        [InlineData("1+1?", "2")]
        public async Task OpenAI_Returns_Expected(string text, string expected)
        {
            using var langchainjs = new LangChainJS(false, _httpClient);

            langchainjs.SetEnvironmentVariable("OPENAI_API_KEY", Configuration["OPENAI_API_KEY"]);

            langchainjs.Setup(@$"

                const model = new OpenAI({{ temperature: 0 }});

                globalThis.run = async () => {{

                    const res = await model.call('{text}');

                    return res.trim();
                }}
            ");

            var result = await langchainjs.InvokeAsync<string>("run");

            Assert.Equal(expected, result);
        }
    }
}