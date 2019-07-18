using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace Instagramwebapi.Helper
{
    abstract class BaseRequest
    {
        public static HttpClient client = new HttpClient();
    }
}
