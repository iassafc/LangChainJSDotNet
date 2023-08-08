# 🦜️🔗🟪 - LangChainJSDotNet
  
⚡ A thin .NET wrapper around the official LangChain.js library⚡

Looking for the official JS/TS or Python versions of LangChain? Check out:

* [LangChain.js](https://github.com/hwchase17/langchainjs) (JS/TS)
* [LangChain](https://github.com/hwchase17/langchain) (Python)


## 🤔 What is this?

**LangChainJSDotNet** provides a seamless interface for .NET developers to run LangChain based code with minimal adjustments.

The official JS/TS version tracks the official Python version closely, therefore LangChain code could be easily ported and run in .NET using **LangChainJSDotNet**, potentially exposing all of the latest AI advancements and features from LangChain and its vast ecosystem to .NET developers.

While integrating Python code in .NET presents challenges, Microsoft's [ClearScript](https://github.com/microsoft/ClearScript) library greatly simplifies the integration process with JavaScript code. Hence, for now, this library focuses exclusively on wrapping the JS version of LangChain.

### Features

- No porting required: Use the official LangChain.js library in .NET.
- The latest LangChain features are readily available.
- Async support: LangChain agents can await .NET async methods.
- Connect from .NET to the new [LangSmith Platform](https://blog.langchain.dev/announcing-langsmith/).
- Debugging capability: Support for debugging LangChain.js code.

### Versioning

This library employs a four-part SemVer-like version scheme. The initial three parts mirror the version of LangChain.js that the library embeds and wraps. The fourth part, always starting at 1, is reserved for incremental bug fixes or non-breaking feature additions.

For instance, `v0.0.124.1` of this library embeds and wraps `v0.0.124` of LangChain.js.

While the first part currently matches LangChain.js's major version, it might be repurposed in the future to also indicate major breaking changes specific to this library.

### Installation

Install the [LangChainJSDotNet NuGet package](https://www.nuget.org/packages/LangChainJSDotNet#readme-body-tab).

You may use the .NET command-line interface:

    dotnet add package LangChainJSDotNet

This command will download and install LangChainJSDotNet along with all its required dependencies.

## 💡 Usage

```csharp
using var langchainjs = new LangChainJS();

langchainjs.Setup(@"

    const model = new OpenAI({ openAIApiKey: 'API_KEY' });

    globalThis.run = async () => {

        const result = await model.call('What is a good name for a company that makes colorful socks?');

        console.log(result.trim());
    }
");

await langchainjs.InvokeAsync("run");
```
```shell
Socktacular!
```

### Chains

```csharp
using var langchainjs = new LangChainJS();

langchainjs.SetEnvironmentVariable("OPENAI_API_KEY", "API_KEY");

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
```
```shell
BrightSox
```

### Agents

See [sample code](./samples/LangChainJSDemo/Program.cs) for an example of a  ReAct agent calling dynamic tools.

```shell
Agent input: "What is the result if you substruct 8 by foo?"...
The result is 3
```

