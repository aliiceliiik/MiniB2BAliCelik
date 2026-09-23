namespace MiniB2B.Business.Common;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }
}