using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public static class SequentialGuidGenerator
    {
        public static Guid NewSequentialGuid()
        {
            long timestamp = DateTime.UtcNow.Ticks;
            byte[] guidArray = new byte[16];

            // Time-based part of GUID
            BitConverter.GetBytes(timestamp).CopyTo(guidArray, 0);

            // Random or machine-specific part of GUID
            new Random().NextBytes(guidArray.Skip(8).ToArray());

            return new Guid(guidArray);
        }
    }
}
