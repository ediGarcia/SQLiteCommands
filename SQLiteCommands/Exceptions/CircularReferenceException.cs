namespace SQLiteCommands.Exceptions;

internal class CircularReferenceException : Exception
{
    public CircularReferenceException(string message) : base(message) { }
}