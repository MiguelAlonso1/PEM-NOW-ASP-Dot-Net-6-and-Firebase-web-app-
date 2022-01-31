using System.ComponentModel.DataAnnotations;

namespace TestingFirebase.Models;//Miguel upgraded this to scoped namespaces -a C# 10 new feature
public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }