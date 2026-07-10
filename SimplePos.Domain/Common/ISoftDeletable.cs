using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Domain.Common;

public interface ISoftDeletable
{
    bool SoftDeleted { get; }
    DateTime? DateTimeSoftDeleted { get; }
    Result SoftDelete();
}
