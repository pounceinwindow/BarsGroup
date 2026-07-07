namespace Domain.Exceptions;

public class DomainException(string message) : Exception(message);

public class InvalidStatusTransitionException(string message)
    : DomainException(message);