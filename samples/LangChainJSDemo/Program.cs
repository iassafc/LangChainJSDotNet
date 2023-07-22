using LangChainJSDotNet;
using Microsoft.Extensions.Configuration;

namespace LangChainJSDemo
{
    public class HostObject
    {
        public async Task<string> FooAsync(string input)
        {
            // Simulate an async operation
            await Task.Delay(100);
            return "super baz";
        }
    }

    internal class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                                    .AddJsonFile("appsettings.json", optional: false)
                                                                    .AddUserSecrets<Program>()
                                                                    .Build();

        static async Task Main(string[] args)
        {
            //await RunAgent();

            await RunChain();
        }

        static async Task RunAgent()
        {
            using var langchainjs = new LangChainJS(enableDebugging: false);

            langchainjs.SetEnvironmentVariable("OPENAI_API_KEY", Configuration["OPENAI_API_KEY"]);

            langchainjs.AddHostObject("host", new HostObject());

            langchainjs.Setup(@"
                const model = new ChatOpenAI({ temperature: 0 });

                const tools = [
                    new DynamicTool({
                        name: ""FOO"",
                        description:
                            ""call this to get the value of foo. input should be the name of the user."",
                        func: async (input) => await host.FooAsync(input),
                    }),
                    new DynamicTool({
                        name: ""BAR"",
                        description:
                            ""call this to get the value of bar. input should be an empty string."",
                        func: async () => ""baz"",
                    }),
                    // Input must be an object with 'high' and 'low' numbers.
                    new DynamicStructuredTool({
                      name: ""number-substructor"",
                      description: ""substructs one number by the other."",
                      schema: z.object({
                        high: z.number().describe(""The upper number""),
                        low: z.number().describe(""The lower number""),
                      }),
                      func: async ({ high, low }) => (high - low).toString() // Outputs must be strings
                    }),
                ];

                globalThis.call = async (input) => {

                    // zero-shot-react-description
                    const executor = await initializeAgentExecutorWithOptions(tools, model, {
                        agentType: ""structured-chat-zero-shot-react-description"",
                        //verbose: true
                    });

                    console.log(`Executing with input ""${input}""...`);

                    const result = await executor.call({ input });

                    //console.log(`Got output ${result.output}`);

                    return result.output;
                };
            ");

            string result = await langchainjs.InvokeAsync<string>("call", "What is the result if you substruct 8 by 5?");

            Console.WriteLine(result);
        }

        static async Task RunChain()
        {
            using var langchainjs = new LangChainJS(enableDebugging: false);

            langchainjs.SetEnvironmentVariable("OPENAI_API_KEY", Configuration["OPENAI_API_KEY"]);

            langchainjs.Setup(@"

                const model = new OpenAI({ temperature: 0.9 });

                const template = new PromptTemplate({
                                                      template: 'What is a good name for a company that makes {product}?',
                                                      inputVariables: ['product'],
                                                    });

                chain = new LLMChain({ llm: model, prompt: template });

                globalThis.call = async (prompt) => {
                    const res = await chain.call({ product: prompt });
                    return res.text.trim();
                }

                globalThis.add = (a, b) => {
                    return a+b;
                }
            ");

            int total = langchainjs.Invoke<int>("add", 1, 2);

            string result = await langchainjs.InvokeAsync<string>("call", "bread");

            Console.WriteLine(result);
        }
    }
}