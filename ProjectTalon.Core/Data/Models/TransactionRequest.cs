using SQLite;

namespace ProjectTalon.Core.Data.Models;

public class TransactionRequest
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string? ReferenceId { get; set; }

    [Indexed]
    public string? AppId { get; set; }
    public string? Parameters { get; set; }
    public string? ExpirationSlot { get; set; }
    public int StatusId { get; set; } 
    public string? TransactionHash { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedUtc { get; set; }

    [Ignore]
    public AppConnect App { get; set; }
}