using System;

namespace Common.System
{
    public class Result
    {
        internal Result(Error error)
        {
            Error = error;
        }

        internal Result() { }

        public Error Error { get; }

        public bool Successful => Error is null;

        public static Result Fail(Error error) => new Result(error);

        public static Result Ok() => new Result();

        public static implicit operator Result(Error error) => Fail(error);
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        internal Result(T value)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (!Successful)
                {
                    throw new InvalidOperationException("Cannot get value of unsuccessful result");
                }

                return _value;
            }
        }

        public static Result<T> Ok(T value) => new Result<T>(value);

        public static implicit operator Result<T>(T value) => Ok(value);
    }
}
