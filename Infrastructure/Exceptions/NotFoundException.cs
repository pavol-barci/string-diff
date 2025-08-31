namespace StringDiff.Infrastructure.Exceptions;

/// <summary>
/// Thrown exception when attempt to get non existent Id from database should fail 
/// </summary>
/// <param name="message">Description of the error</param>
public class NotFoundException(string message) : Exception(message);