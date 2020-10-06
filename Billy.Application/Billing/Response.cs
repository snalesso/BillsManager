namespace Billy.Billing.Application
{
    //public class Response
    //{
    //    public ResponseStatus Status { get; }

    //    public Response(ResponseStatus status)
    //    {
    //        this.Status = status;
    //    }
    //}

    public class Response<T> //: Response
    {
        public Response(
            ResponseStatus status,
            T content)
        //: base(status)
        {
            this.Status = status;
            this.Content = content; // ?? throw new ArgumentNullException(nameof(content));
        }

        public ResponseStatus Status { get; }
        public T Content { get; }
    }
}