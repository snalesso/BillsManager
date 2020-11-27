using System.Collections.Generic;

namespace Billy.Billing.Services
{
    public class Response<T>
    {
        public Response(
            T content,
            IEnumerable<DbValidationError<T>> errors = null)
        {
            this.Content = content;
            this.Errors = errors;
        }

        public ResponseStatus Status { get; }
        public T Content { get; }
        public IEnumerable<DbValidationError<T>> Errors { get; }
    }

    public class DbValidationError<T>
    {
        public string EntityName { get; }
        public string PropertyName { get; }
        public string Message { get; }
        public DbErrorCode Code { get; }
    }

    public enum DbErrorCode
    {
        Unique,
    }
}