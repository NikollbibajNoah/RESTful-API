using System.ComponentModel.DataAnnotations;

namespace RESTful.Entity;

public class User
{

    public int Id { get; set; }

    [Required(ErrorMessage = "Name ist erforderlich")]
    [StringLength(50, ErrorMessage = "Name darf max. 50 Zeichen lang sein.")]
    public string Name { get; set; }

    [Range(0, 150, ErrorMessage = "Alter muss zwischen 0 und 150 liegen.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Email ist erforderlich.")]
    [EmailAddress(ErrorMessage = "Ungültige Email-Adresse.")]
    [StringLength(255)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Position ist erforderlich.")]
    [StringLength(100, ErrorMessage = "Position darf max. 100 Zeichen lang sein.")]
    public string Position { get; set; }
}