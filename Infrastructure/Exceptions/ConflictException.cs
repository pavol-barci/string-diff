namespace StringDiff.Infrastructure.Exceptions;

/// <summary>
/// Thrown exception when attempt to create item in database with same Id was made
/// </summary>
/// <param name="message">Description of the error</param>
public class ConflictException(string message) : Exception(message);