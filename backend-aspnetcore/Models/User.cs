using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAspNetCore.Models;

public class User
{
    [Column(Order = 0)]
    public Guid Id { get; set; }
    [Column(Order = 1)]
    public string Username { get; set; } = string.Empty;
    [Column(Order = 2)]
    public string Password { get; set; } = string.Empty;
    [Column(Order = 3)]
    public string Email { get; set; } = string.Empty;
    [Column(Order = 4)]
    public string Phone { get; set; } = string.Empty;

    [Column(Order = 5)]
    public bool IsActive { get; set; }
    [Column(Order = 6)]
    public string Remark { get; set; } = string.Empty;
}