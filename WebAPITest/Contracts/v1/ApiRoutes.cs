using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Contracts.v1
{
    public static class ApiRoutes
    {
        public const string root = "api";
        public const string version = "v1";
        public const string baseUrl = root + "/" + version;

        public static class Post
        {
            public const string GetAllPosts = baseUrl + "/posts";
        }
    }
}
