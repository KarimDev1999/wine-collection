namespace Wine.Domain.Exceptions;

public class EntityNotFoundException(string message) : Exception(message);