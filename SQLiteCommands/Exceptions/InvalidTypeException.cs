﻿namespace SQLiteCommands.Exceptions;

internal class InvalidTypeException : Exception
{
    public InvalidTypeException(string message) : base(message) { }
}