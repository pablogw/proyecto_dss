namespace Fire_Emblem_View;

public class InvalidInputRequestException : ApplicationException
{
    public InvalidInputRequestException(string message) : base(message)
    {
    }
}