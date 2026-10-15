using System.Text;
using System.Text.RegularExpressions;

namespace Lab9v7;

class AuthException : Exception
{
    public AuthException(string message) : base(message) { }
}

class InvalidCredentialsFormatException : AuthException
{
    public InvalidCredentialsFormatException(string message) : base(message) { }
}

class AuthenticationFailedException : AuthException
{
    public AuthenticationFailedException(string message) : base(message) { }
}

abstract class AuthProvider
{
    public string Name { get; }

    protected AuthProvider(string name)
    {
        Name = name;
    }

    public abstract bool Authenticate(string credentials);

    protected static void RequireNotEmpty(string? credentials)
    {
        if (string.IsNullOrWhiteSpace(credentials))
            throw new InvalidCredentialsFormatException("Credentials не можуть бути порожніми.");
    }
}

class PasswordAuth : AuthProvider
{
    private static readonly Regex Format = new(@"^[A-Za-z0-9_]{3,20}:.{8,64}$");

    public Dictionary<string, string> Users { get; } = new();

    public PasswordAuth() : base("Password") { }

    public override bool Authenticate(string credentials)
    {
        RequireNotEmpty(credentials);

        if (!Format.IsMatch(credentials))
            throw new InvalidCredentialsFormatException(
                "Очікується формат 'login:password' (логін 3-20 символів, пароль від 8 символів).");

        int sep = credentials.IndexOf(':');
        string login = credentials[..sep];
        string password = credentials[(sep + 1)..];

        if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
            throw new InvalidCredentialsFormatException("Пароль має містити літери та цифри.");

        if (!Users.TryGetValue(login, out var stored))
            throw new AuthenticationFailedException($"Користувача '{login}' не знайдено.");

        return stored == password;
    }
}

class OAuthAuth : AuthProvider
{
    private static readonly Regex Format = new(@"^Bearer [A-Za-z0-9\-_\.]{16,}$");

    public string ClientId { get; }
    public bool ServerAvailable { get; set; } = true;
    public HashSet<string> ValidTokens { get; } = new();

    public OAuthAuth(string clientId) : base("OAuth")
    {
        ClientId = clientId;
    }

    public override bool Authenticate(string credentials)
    {
        RequireNotEmpty(credentials);

        if (!Format.IsMatch(credentials))
            throw new InvalidCredentialsFormatException(
                "Очікується формат 'Bearer <token>' (токен від 16 символів: літери, цифри, - _ .).");

        if (!ServerAvailable)
            throw new AuthenticationFailedException("OAuth-сервер недоступний.");

        string token = credentials["Bearer ".Length..];
        return ValidTokens.Contains(token);
    }
}

class BiometricAuth : AuthProvider
{
    private static readonly Regex Format =
        new(@"^(fingerprint|face):[0-9a-fA-F]{16}$", RegexOptions.IgnoreCase);

    public bool DeviceReady { get; set; } = true;
    public HashSet<string> EnrolledTemplates { get; } = new(StringComparer.OrdinalIgnoreCase);

    public BiometricAuth() : base("Biometric") { }

    public override bool Authenticate(string credentials)
    {
        RequireNotEmpty(credentials);

        if (!Format.IsMatch(credentials))
            throw new InvalidCredentialsFormatException(
                "Очікується формат 'fingerprint:<16 hex>' або 'face:<16 hex>'.");

        if (!DeviceReady)
            throw new AuthenticationFailedException("Біометричний сканер не готовий.");

        return EnrolledTemplates.Contains(credentials);
    }
}

record AuthRequest(AuthProvider Provider, string Credentials);

class AuthService
{
    public void AuthenticateAll(List<AuthRequest> requests)
    {
        int ok = 0, denied = 0, errors = 0;

        foreach (var req in requests)
        {
            try
            {

                bool result = req.Provider.Authenticate(req.Credentials);

                if (result)
                {
                    ok++;
                    Console.WriteLine($"[OK]      {req.Provider.Name}: автентифікацію пройдено.");
                }
                else
                {
                    denied++;
                    Console.WriteLine($"[DENIED]  {req.Provider.Name}: невірні облікові дані.");
                }
            }
            catch (InvalidCredentialsFormatException ex)
            {
                errors++;
                Console.WriteLine($"[FORMAT]  {req.Provider.Name}: {ex.Message}");
            }
            catch (AuthenticationFailedException ex)
            {
                errors++;
                Console.WriteLine($"[FAILED]  {req.Provider.Name}: {ex.Message}");
            }
            catch (Exception ex)
            {
                errors++;
                Console.WriteLine($"[ERROR]   {req.Provider.Name}: неочікувана помилка - {ex.Message}");
            }
        }

        Console.WriteLine($"\nПідсумок: успішно - {ok}, відхилено - {denied}, помилок - {errors}");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var password = new PasswordAuth();
        password.Users["ivan"] = "Secret123";

        var oauth = new OAuthAuth("client-42");
        oauth.ValidTokens.Add("abc123XYZ789token_01");

        var bio = new BiometricAuth();
        bio.EnrolledTemplates.Add("fingerprint:A1B2C3D4E5F60718");

        var unavailableOAuth = new OAuthAuth("client-43") { ServerAvailable = false };

        var requests = new List<AuthRequest>
        {
            new(password, "ivan:Secret123"),
            new(password, "ivan:WrongPass1"),
            new(password, "ivan:short"),
            new(password, "petro:Password99"),
            new(oauth, "Bearer abc123XYZ789token_01"),
            new(oauth, "abc123XYZ789token_01"),
            new(unavailableOAuth, "Bearer abc123XYZ789token_01"),
            new(bio, "fingerprint:A1B2C3D4E5F60718"),
            new(bio, "face:0000000000000000"),
            new(bio, "retina:123")
        };

        var service = new AuthService();
        service.AuthenticateAll(requests);
    }
}