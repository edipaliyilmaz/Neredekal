using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Helpers
{
    using System.Net.Http;
    using System.Text;
    using Newtonsoft.Json;

    namespace Tests.Helpers
    {
        public static class TestHelper
        {
            /// <summary>
            /// Converts an object to JSON content for HTTP requests.
            /// </summary>
            public static StringContent GetJsonContent(object obj)
            {
                var json = JsonConvert.SerializeObject(obj);
                return new StringContent(json, Encoding.UTF8, "application/json");
            }
        }
    }

}
