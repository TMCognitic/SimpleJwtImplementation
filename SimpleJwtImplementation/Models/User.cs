namespace SimpleJwtImplementation.Models
{
    public class User (int id, string nom, string prenom, string email, string role)
    {
        public int Id { get; } = id;
        public string Nom { get; } = nom;
        public string Prenom { get; } = prenom;
        public string Email { get; } = email;
        public string Role {  get; } = role;
        public string? RefreshToken { get; set; }
    }
}
