using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Contracts.Response
{
    public class AuthFailResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
