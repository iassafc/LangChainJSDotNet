using System;
using System.Security.Cryptography;
using Microsoft.ClearScript.JavaScript;

namespace LangChainJSDotNet
{
    internal sealed class Crypto
    {
        public static unsafe ITypedArray GetRandomValues(ITypedArray array)
        {
            var size = Convert.ToInt32(array.Size);
            array.InvokeWithDirectAccess(pBytes => {
                RandomNumberGenerator.Fill(new Span<byte>(pBytes.ToPointer(), size));
            });
            return array;
        }
    }
}
