using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationDemo.Models;

public class SignerData
{
    [Required(ErrorMessage = "Este campo es obligatorio")]
    [DataType(DataType.EmailAddress, ErrorMessage = "email es o")]
    [Display(Name = "Correo", Prompt = "test@mail.com")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio")]
    [MinLength(5, ErrorMessage = "Nombre debe contener mínimo 5 caracteres")]
    [Display(Name = "Nombre Completo", Prompt = "Juan Perez")]
    public string Name { get; set; }

    public Position Position { get; set; }
}

public enum Position
{
    Arriba,
    Medio,
    Abajo
}