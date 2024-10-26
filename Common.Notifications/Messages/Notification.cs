using Microsoft.Extensions.Logging;

namespace Common.Notifications.Messages;

public class Notification : Message
{
    /// <summary>
    /// Id da notificação
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Data hora da notificação
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Nivel de alerta da notificação
    /// </summary>
    public LogLevel LogLevel { get; private set; }
    /// <summary>
    /// Chave
    /// </summary>
    public string? Key { get; private set; }
    /// <summary>
    /// Valor
    /// </summary>
    public string Value { get; private set; }
    /// <summary>
    /// Detalhe
    /// </summary>
    public string Detail { get; private set; }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="type">Tipo</param>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor</param>
    public Notification(LogLevel logLevel, string type, string key, string value, string detail) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = logLevel;
        Key = key;
        Value = value;
        Detail = detail;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="type">Tipo</param>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor</param>
    public Notification(string type, string key, string value, string detail) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = LogLevel.Information;
        Key = key;
        Value = value;
        Detail = detail;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="type">Tipo</param>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor</param>
    public Notification(LogLevel logLevel, string type, string key, string value) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = logLevel;
        Key = key;
        Value = value;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="type">Tipo</param>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor</param>
    public Notification(string type, string key, string value) : base(type)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        LogLevel = LogLevel.Information;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor</param>
    public Notification(LogLevel logLevel, string key, string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        LogLevel = logLevel;
        Timestamp = DateTime.Now;
        Key = key;
        Value = value;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="key">Chave</param>
    /// <param name="value">Valor</param>
    public Notification(string key, string value)
    {
        Id = Guid.NewGuid();

        AgregationId = Id;
        LogLevel = LogLevel.Information;
        Timestamp = DateTime.Now;
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="value">Valor</param>
    public Notification(LogLevel logLevel, string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        LogLevel = logLevel;
        Timestamp = DateTime.Now;
        Value = value;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="value">Valor</param>
    public Notification(string value)
    {
        Id = Guid.NewGuid();
        AgregationId = Id;
        Timestamp = DateTime.Now;
        Value = value;
    }
}

