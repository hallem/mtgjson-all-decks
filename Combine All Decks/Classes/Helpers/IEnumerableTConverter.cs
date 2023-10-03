using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming
// ReSharper disable JoinNullCheckWithUsage

namespace Combine.Classes.Helpers;

// Modified DictionaryTEnumTValueConverter
// from https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-5-0#custom-converter-patterns
public class IEnumerableTConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType)
        {
            return false;
        }

        var realType = typeToConvert.GetGenericTypeDefinition();
        
        return !realType.IsAssignableTo(typeof(IEnumerable<>));
    }

    public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
    {
        Type genericType = type.GetGenericArguments()[0];

        JsonConverter converter = (JsonConverter)Activator.CreateInstance(
            typeof(ICollectionTConverterInner<,>).MakeGenericType(type, genericType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: new object[] { type, options },
            culture: null);

        return converter;
    }

    private class ICollectionTConverterInner<T, U> : JsonConverter<T> where T : IEnumerable<U>
    {
        private readonly JsonConverter<T> _normalConverter;

        public ICollectionTConverterInner(Type type, JsonSerializerOptions options)
        {
            // For performance, use the existing converter if available.
            var existing = new JsonSerializerOptions().GetConverter(type);
            
            if (existing == null)
                throw new ApplicationException($"Standard converter for {type} not found.");

            _normalConverter = (JsonConverter<T>)existing;
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Untested
            return _normalConverter.Read(ref reader, typeToConvert, options);
        }

        public override void Write(Utf8JsonWriter writer, T collection, JsonSerializerOptions options)
        {
            if (!collection.Any())
            {
                writer.WriteNullValue();
                return;
            }

            _normalConverter.Write(writer, collection, options);
        }
    }
}