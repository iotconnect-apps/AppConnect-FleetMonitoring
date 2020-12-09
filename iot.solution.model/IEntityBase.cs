using System;

namespace db.iot.solution
{
    public interface IEntityBase
    {
        int RecordId { get; set; }
        Guid RefId { get; set; }
    }
}