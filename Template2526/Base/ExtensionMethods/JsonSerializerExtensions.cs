using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Base.ExtensionMethods
{
    public static partial class JsonSerializerExtensions
    {
        public static T? DeserializeAnonymousType<T>(string json, JsonSerializerOptions? options = default)
            => JsonSerializer.Deserialize<T>(json, options);

        public static ValueTask<TValue?> DeserializeAnonymousTypeAsync<TValue>(Stream stream, JsonSerializerOptions? options = default, CancellationToken cancellationToken = default)
            => JsonSerializer.DeserializeAsync<TValue>(stream, options, cancellationToken); // Method to deserialize from a stream added for completeness
    }
}
