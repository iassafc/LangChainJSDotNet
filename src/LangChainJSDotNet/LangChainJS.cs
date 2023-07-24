using Microsoft.ClearScript.JavaScript;
using Microsoft.ClearScript.V8;
using Microsoft.ClearScript;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using System.Net.Http;

namespace LangChainJSDotNet
{
    public class LangChainJS : IDisposable
    {
        protected readonly IScriptEngine _engine;

        protected readonly HttpClient _httpClient;

        public LangChainJS(bool enableDebugging = false, HttpClient httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();

            V8ScriptEngineFlags flags = V8ScriptEngineFlags.EnableTaskPromiseConversion;
            if (enableDebugging)
            {
                flags |= V8ScriptEngineFlags.EnableRemoteDebugging
                        | V8ScriptEngineFlags.EnableDebugging
                        | V8ScriptEngineFlags.AwaitDebuggerAndPauseOnStart;
            }
            _engine = new V8ScriptEngine(flags);

            // Enable file loading of any library type
            _engine.DocumentSettings.AccessFlags = DocumentAccessFlags.EnableFileLoading | DocumentAccessFlags.AllowCategoryMismatch;

            InitJSEnvironment();
        }

        public void Dispose()
        {
            if (_engine != null)
            {
                _engine.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public void Setup(string code)
        {
            _engine.Execute(code);
        }

        public void SetEnvironmentVariable(string name, string value)
        {
            _engine.Script.process.env[name] = value;
        }

        public void AddHostObject(string itemName, object target)
        {
            _engine.AddHostObject(itemName, target);
        }

        public void AddHostType(Type type)
        {
            _engine.AddHostType(type);
        }

        public T Invoke<T>(string funcName, params object[] args)
        {
            var result = _engine.Invoke(funcName, args);

            // Convert the result to the desired type and return it
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public void Invoke(string funcName, params object[] args)
        {
            _engine.Invoke(funcName, args);
        }

        public async Task<T> InvokeAsync<T>(string funcName, params object[] args)
        {
            var result = await _engine.Invoke(funcName, args).ToTask();

            // Convert the result to the desired type and return it
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public async Task InvokeAsync(string funcName, params object[] args)
        {
            await _engine.Invoke(funcName, args).ToTask();
        }

        private void InitJSEnvironment()
        {
            // implement console
            _engine.AddHostType(typeof(Console));
            _engine.Execute(@"
                console = {
                    log: value => Console.WriteLine(value),
                    warn: value => Console.WriteLine('WARNING: {0}', value),
                    error: value => Console.WriteLine('ERROR: {0}', value)
                };
            ");

            // implement setTimeout
            Action<ScriptObject, int> setTimeout = (func, delay) => {
                var timer = new Timer(_ => func.Invoke(false));
                timer.Change(delay, Timeout.Infinite);
            };
            _engine.Script._setTimeout = setTimeout;
            _engine.Execute(@"
                function setTimeout(func, delay) {
                    let args = Array.prototype.slice.call(arguments, 2);
                    _setTimeout(func.bind(undefined, ...args), delay || 0);
                }
            ");

            // Implement self
            _engine.Execute(@"globalThis.self = globalThis");

            // Implement crypto API (getRandomValues)
            _engine.AddHostType("MSCrypto", HostItemFlags.PrivateAccess, typeof(Crypto));
            _engine.Execute(@"
                crypto = {
                    getRandomValues: array => MSCrypto.GetRandomValues(array)
                };
            ");

            // Simulate process API globally for setting environment variables
            _engine.Execute(@"
                process = {
                    env: {}
                };
            ");

            // Required to implement XMLHttpRequest
            _engine.AddHostObject("hostHttp", HostItemFlags.PrivateAccess, new Http(_httpClient));

            // Implement WHATWG URL Standard:
            _engine.Execute("url.js", LoadScript("LangChainJSDotNet.dist.url.js"));

            // Implement everything else:
            _engine.Execute("bundle.js", LoadScript("LangChainJSDotNet.dist.bundle.js"));
        }

        private string LoadScript(string name)
        {
            // Get the current assembly
            var assembly = Assembly.GetExecutingAssembly();

            // Get the stream to the embedded resource
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                // Read the stream and convert it to a string
                using (var reader = new StreamReader(stream))
                {
                    var script = reader.ReadToEnd();

                    return script;
                }
            }
        }
    }
}