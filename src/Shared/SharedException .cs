using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Shared
{
    /// <summary>
    /// エラーメッセージの規定値をクラス名とメソッド名にする<see cref="Exception"/>基底クラスを提供します。
    /// </summary>
    [Serializable()]
    public class TaskNoteException : Exception
    {

        public TaskNoteException()
        {
        }

        public TaskNoteException(string message) : base(message)
        {
        }

        public TaskNoteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public TaskNoteException(Exception innerException,
            [CallerFilePath] string filePath = "",
            [CallerMemberName] string memberName = "") :
            base($"{System.IO.Path.GetFileNameWithoutExtension(filePath)}.{memberName}", innerException)
        {
        }

        protected TaskNoteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }
}
