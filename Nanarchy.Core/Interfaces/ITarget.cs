using System;

namespace Nanarchy.Core.Interfaces
{
    public interface ITarget
    {
        int Id { get; set; }
        Guid GlobalIdentifier { get; set; }
    }
}