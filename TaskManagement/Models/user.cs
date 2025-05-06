namespace TaskManagement.Models;

public partial class user
{
    public int id { get; set; }

    public string username { get; set; } = null!;

    public string passwordhash { get; set; } = null!;

    public string email { get; set; } = null!;

    public DateTime? createdat { get; set; }
    public string role { get; set; } = "Regular";
    public virtual ICollection<task> tasks { get; set; } = new List<task>();
}
