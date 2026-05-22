namespace SmartLibrary.Auth.User;

public class ConfigureUserDbOptions
{
    public string EncryptionSecret { get; set; } = "Secret";
    public string UserDbConnectionString { get; set; } = "DataSource=App.db";
    public TimeSpan TokenLifetime { get; set; } = TimeSpan.FromHours(1);
    public string TokenIssuer { get; set; } = "someauthority.com";
    public string TokenAudience { get; set; } = "myapi.com";
}
