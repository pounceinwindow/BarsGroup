namespace Application.Common
{
    public class DomainError : Exception
    {
        public DomainError(string message = "Error in domain logic") : base(message) { }
    }
}
