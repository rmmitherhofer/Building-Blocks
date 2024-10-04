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
    public LogLevel Level { get; private set; }
    /// <summary>
    /// Chave
    /// </summary>
    public string? Chave { get; private set; }
    /// <summary>
    /// Valor
    /// </summary>
    public string Valor { get; private set; }
    /// <summary>
    /// Detalhe
    /// </summary>
    public string Detalhe { get; private set; }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="tipo">Tipo</param>
    /// <param name="chave">Chave</param>
    /// <param name="valor">Valor</param>
    public Notification(LogLevel nivel, string tipo, string chave, string valor, string detalhe) : base(tipo)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Timestamp = DateTime.Now;
        Level = nivel;
        Chave = chave;
        Valor = valor;
        Detalhe = detalhe;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="tipo">Tipo</param>
    /// <param name="chave">Chave</param>
    /// <param name="valor">Valor</param>
    public Notification(string tipo, string chave, string valor, string detalhe) : base(tipo)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Timestamp = DateTime.Now;
        Level = LogLevel.Information;
        Chave = chave;
        Valor = valor;
        Detalhe = detalhe;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="tipo">Tipo</param>
    /// <param name="chave">Chave</param>
    /// <param name="valor">Valor</param>
    public Notification(LogLevel nivel, string tipo, string chave, string valor) : base(tipo)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Timestamp = DateTime.Now;
        Level = nivel;
        Chave = chave;
        Valor = valor;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="tipo">Tipo</param>
    /// <param name="chave">Chave</param>
    /// <param name="valor">Valor</param>
    public Notification(string tipo, string chave, string valor) : base(tipo)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Timestamp = DateTime.Now;
        Level = LogLevel.Information;
        Chave = chave;
        Valor = valor;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="chave">Chave</param>
    /// <param name="valor">Valor</param>
    public Notification(LogLevel nivel, string chave, string valor)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Level = nivel;
        Timestamp = DateTime.Now;
        Chave = chave;
        Valor = valor;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="chave">Chave</param>
    /// <param name="valor">Valor</param>
    public Notification(string chave, string valor)
    {
        Id = Guid.NewGuid();

        IdAgregacao = Id;
        Level = LogLevel.Information;
        Timestamp = DateTime.Now;
        Chave = chave;
        Valor = valor;
    }

    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="valor">Valor</param>
    public Notification(LogLevel nivel, string valor)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Level = nivel;
        Timestamp = DateTime.Now;
        Valor = valor;
    }
    /// <summary>
    /// Construtor
    /// </summary>
    /// <param name="valor">Valor</param>
    public Notification(string valor)
    {
        Id = Guid.NewGuid();
        IdAgregacao = Id;
        Timestamp = DateTime.Now;
        Valor = valor;
    }
}

