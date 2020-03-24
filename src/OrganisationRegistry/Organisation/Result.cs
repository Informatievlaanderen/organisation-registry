namespace OrganisationRegistry.Organisation
{
    using System.Linq;

    public class Result<T>
    {
        public string[] ErrorMessages { get; }
        public T Value { get; }

        public bool HasErrors => ErrorMessages.Any();

        private Result(T value)
        {
            ErrorMessages = new string[0];
            Value = value;
        }

        private Result(string[] errorMessages)
        {
            ErrorMessages = errorMessages;
        }

        public static Result<T> Fail(params string[] errorMessages)
        {
            return new Result<T>(errorMessages ?? new string[0]);
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }
    }
}
