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
            return "5";
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
            //await RunModelAndPrint();

            //await RunModelAndReturn();

            //await RunChain();

            await RunAgent();
        }

        static async Task RunModelAndPrint()
        {
            using var langchainjs = new LangChainJS();

            langchainjs.SetEnvironmentVariable("OPENAI_API_KEY", Configuration["OPENAI_API_KEY"]);

            langchainjs.Setup(@"

                const model = new OpenAI();

                globalThis.run = async () => {

                    const result = await model.call('What would be a good company name a company that makes colorful socks?');

                    console.log(result.trim());
                }
            ");

            await langchainjs.InvokeAsync("run");
        }

        static async Task RunModelAndReturn()
        {
            using var langchainjs = new LangChainJS();

            langchainjs.SetEnvironmentVariable("OPENAI_API_KEY", Configuration["OPENAI_API_KEY"]);

            langchainjs.Setup(@"

                const model = new OpenAI({ temperature: 0.9 });

                globalThis.run = async () => {

                    const res = await model.call('What would be a good company name a company that makes colorful socks?');

                    return res.trim();
                }
            ");

            var result = await langchainjs.InvokeAsync<string>("run");

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

                globalThis.run = async (prompt) => {
                    const res = await chain.call({ product: prompt });
                    return res.text.trim();
                }
            ");

            string result = await langchainjs.InvokeAsync<string>("run", "colorful socks");

            Console.WriteLine(result);
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

                globalThis.run = async (input) => {

                    // zero-shot-react-description
                    const executor = await initializeAgentExecutorWithOptions(tools, model, {
                        agentType: ""structured-chat-zero-shot-react-description"",
                        /* verbose: true */
                    });

                    console.log(`Agent input: ""${input}""...`);

                    const result = await executor.call({ input });

                    return result.output;
                };
            ");

            string result = await langchainjs.InvokeAsync<string>("run", "What is the result if you substruct 8 by foo?");

            Console.WriteLine(result);
        }
    }
}