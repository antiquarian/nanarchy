using System;

namespace Nanarchy
{
    public interface IHierarchy
    {
        int Id { get; set; }
        Guid GlobalIdentifier { get; set; }
    }
}