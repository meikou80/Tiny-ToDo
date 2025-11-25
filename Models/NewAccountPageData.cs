namespace TinyToDo.Models;

public class NewAccountPageData
{
    public string UserId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Expires { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}