using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Shared.Http;

[Serializable()]
public class BadRequestException : TaskNoteException
{
    public BadRequestException(BadResponse badResponse)
    {
        BadResponse = badResponse ?? throw new ArgumentNullException(nameof(badResponse));
    }

    public BadRequestException(string message, BadResponse badResponse) : base(message)
    {
        BadResponse = badResponse ?? throw new ArgumentNullException(nameof(badResponse));
    }

    public BadRequestException(string message, BadResponse badResponse, Exception innerException) : base(message, innerException)
    {
        BadResponse = badResponse ?? throw new ArgumentNullException(nameof(badResponse));
    }

    public BadRequestException(Exception innerException, BadResponse badResponse, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") : base(innerException, filePath, memberName)
    {
        BadResponse = badResponse ?? throw new ArgumentNullException(nameof(badResponse));
    }

    protected BadRequestException(SerializationInfo info, BadResponse badResponse, StreamingContext context) : base(info, context)
    {
        BadResponse = badResponse ?? throw new ArgumentNullException(nameof(badResponse));
    }

    public BadResponse BadResponse { get; }
}
