using Api.Service.Configurations.Settings;
using Api.Service.Middleware;
using Common.Logs.Configurations;
using Common.Notifications.Configurations;
using Extensoes;
using Logs.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnapTrace.Configurations;
using SnapTrace.Formatters;
using Swagger.Configurations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Service.Configurations;

public static class ApiConfiguration
{
    public static IServiceCollection AddCoreApiConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment, CoreApiSettings settings)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(IServiceCollection));
        ArgumentNullException.ThrowIfNull(configuration, nameof(IConfiguration));

        services.AddHttpContextAccessor();

        configuration.SetConfiguration(environment);

        services.AddControllers(options => options.EnableEndpointRouting = false)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new GenericEnumIntegerConverter());
            });

        services.AddNotificationConfig();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerConfig(configuration);

        services.AddSnapTrace(configuration, settings.SnapTraceSettings, options =>
        {
            options.Formatter = (FormatterArgs args) =>
            {
                if (args.Exception == null)
                    return args.DefaultValue;

                string exceptionStr = new ExceptionFormatter().Format(args.Exception);
                return string.Join(Environment.NewLine, [args.DefaultValue, exceptionStr]);
            };
        });

        return services;
    }
    public static WebApplication UseCoreApiConfig(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(WebApplication));

        app.UseAuthorization();

        app.UseSnapTrace();

        app.UseMiddleware<RequestIndetityMiddleware>();

        app.UseNotificationConfig();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseSwaggerConfig(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());

        app.MapControllers();

        

        //app.UseEndpoints(endpoints =>
        //{
        //    endpoints.MapControllers();
        //});


        return app;
    }


}

public class EnumIntegerConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
{
    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && Enum.IsDefined(typeof(TEnum), reader.GetInt32()))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), reader.GetInt32());
        }

        throw new JsonException($"Invalid value {reader.GetString()} for enum {typeof(TEnum).Name}");
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(Convert.ToInt32(value));
    }
}

public class GenericEnumIntegerConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsEnum;
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(EnumIntegerConverter<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType);
    }
}