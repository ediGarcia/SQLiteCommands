namespace SQLiteCommands.Exceptions;

internal class ColumnSpecificationNeededException : Exception
{
    public ColumnSpecificationNeededException(string message): base(message) { }
}

