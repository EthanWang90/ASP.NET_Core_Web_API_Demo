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
            public const string Get = baseUrl + "/post/{Id}";
            public const string Create = baseUrl + "/posts";
            public const string Update = baseUrl + "/post/{Id}";
            public const string Delete = baseUrl + "/post/{Id}";
        }

        public static class Identity
        {
            public const string Register = baseUrl + "/register";
            public const string Login = baseUrl + "/login";
            public const string RefreshToken = baseUrl + "/RefreshToken";
        }
    }
}
