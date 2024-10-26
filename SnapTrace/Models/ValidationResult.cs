using Common.Notifications.Messages;

namespace SnapTrace.Models;

public class ValidationResult
{
    public int Status { get; set; }
    public IEnumerable<Validation> Validations { get; set; }
    public ICollection<Notification> Notifications { get; set; }

}
