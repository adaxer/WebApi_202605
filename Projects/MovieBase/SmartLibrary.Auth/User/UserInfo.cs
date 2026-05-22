namespace SmartLibrary.Auth.User;

public class UserInfo
{
  public string? Email { get; set; }
  public string? UserName { get; set; }
  public List<string> Roles { get; set; } = new List<string>();
}

