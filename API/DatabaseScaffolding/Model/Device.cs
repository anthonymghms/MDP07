using System;
using System.Collections.Generic;

#nullable disable

namespace DatabaseScaffolding.Model
{
    public partial class Device
    {
        public Guid DeviceId { get; set; }
        public Guid DeviceUserId { get; set; }
        public string Name { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }

        public virtual User DeviceUser { get; set; }
    }
}
