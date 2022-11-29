namespace LoginAPI.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDTO UserDetail{ get; set; }
    }   
}
