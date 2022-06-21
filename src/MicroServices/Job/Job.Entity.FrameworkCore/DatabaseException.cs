using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Job.Entity.FrameworkCore;

public class DatabaseException : TaskNoteException
{
    public DatabaseException()
    {
    }

    public DatabaseException(string message) : base(message)
    {
    }

    public DatabaseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public DatabaseException(Exception innerException, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") : base(innerException, filePath, memberName)
    {
    }

    protected DatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
