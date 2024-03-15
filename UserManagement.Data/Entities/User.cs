using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
namespace UserManagement.Models;

public class User
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Forename { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }

    // we try to define date type and then assign the property
    [Column(TypeName="Date")]
    public DateTime DateOfBirth { get; set; }

}
