using System;
using System.Configuration;
using System.Runtime.Serialization;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Core
{
    [DataContract]
    public abstract class Target : ITarget
    {
        public int Id { get; set; }
        public Guid GlobalIdentifier { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        [IgnoreDataMember]
        public string TableName
        {
            get
            {
                var targetTablePrefix = ConfigurationManager.AppSettings["NDB.TargetTablePrefix"];
                var typeName = this.GetType().Name;
                return string.Format("{0}{1}", targetTablePrefix, typeName);
            }
        }

        public abstract ITargetData Data { get; set; }
    }

    public interface ITarget
    {
        int Id { get; set; }
        Guid GlobalIdentifier { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime LastModifiedDate { get; set; }
        string TableName { get; }
        ITargetData Data { get; set; }
    }
}