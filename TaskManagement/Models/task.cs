namespace TaskManagement.Models;

public partial class task
{
    public int id { get; set; }

    public int? userid { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public string status { get; set; } = null!;

    public DateTime duedate { get; set; }

    public DateTime? createdat { get; set; }= DateTime.UtcNow;

    public DateTime? updatedat { get; set; }= DateTime.UtcNow;

    public virtual user? user { get; set; }
}
