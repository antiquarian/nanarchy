using System.Collections.Generic;
using System.Runtime.Serialization;
using Nanarchy.Core;
using Nanarchy.Core.Interfaces;

namespace Nanarchy.Sample.Targets
{
    [DataContract]
    public class Setting
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
    [DataContract]
    public class SettingsCollection : ITargetData
    {
        [DataMember]
        public IEnumerable<Setting> Settings { get; set; }
    }

    [DataContract]
    public class SettingsTarget : Target
    {
        [DataMember]
        public override ITargetData Data { get; set; }
    }
}