namespace SQLiteCommands.Exceptions;

internal class InvalidAttributeCombinationException : Exception
{
    public InvalidAttributeCombinationException(string message) : base(message) { }
}