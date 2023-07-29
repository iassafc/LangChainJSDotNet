// whatwg-fetch
import { Headers, Request, Response, DOMException, fetch } from 'whatwg-fetch';
globalThis.Headers = Headers;
globalThis.Request = Request;
globalThis.Response = Response;
globalThis.DOMException = DOMException;
globalThis.fetch = fetch;

// text-encoding
import { TextEncoder, TextDecoder } from 'text-encoding';
globalThis.TextEncoder = TextEncoder;
globalThis.TextDecoder = TextDecoder;

// abortcontroller-polyfill
import 'abortcontroller-polyfill/dist/polyfill-patch-fetch';
AbortSignal.timeout = (duration) =>
{
  const controller = new AbortController;
  setTimeout(() => controller.abort(), duration);
  return controller.signal;
};

// web-streams-polyfill
import 'web-streams-polyfill/polyfill';

// mock-xmlhttprequest
import { newMockXhr } from 'mock-xmlhttprequest';

const MockXhr = newMockXhr();

MockXhr.onSend = async function (request) {
    try {
        let headers = request.requestHeaders.getHash();

        let str_body = request.body;
        if (typeof request.body !== 'string') {
            // If request.body is not a string, decode it using TextDecoder
            let decoder = new TextDecoder("utf-8");
            str_body = decoder.decode(request.body);
        }

        // asyn response from host HttpClient
        let jsonResponse = await hostHttp.SendAsync(request.url, request.method, headers, str_body);

        let response = JSON.parse(jsonResponse);

        request.respond(response.StatusCode, response.Headers, response.Content);
    } catch(err) {
        console.error(err);
    }
}

// Install in the global context so "new XMLHttpRequest()" creates MockXhr instances
globalThis.XMLHttpRequest = MockXhr;