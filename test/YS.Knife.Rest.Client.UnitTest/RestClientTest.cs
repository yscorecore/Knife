﻿using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;

namespace YS.Knife.Rest.Client.UnitTest
{
    [TestClass]
    public class RestClientTest : KnifeHost
    {
        #region RelativePath

        [TestCategory("RelativePath")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenBaseAddressNotEndWithBackslashAndPathStartWithBackslash()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl + "/api");
            var all = await client.Get<string[]>("/testget");
            all.Should().BeEquivalentTo(new string[] { "value1", "value2" });
        }
        [TestCategory("RelativePath")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenBaseAddressEndWithBackslashAndPathStartWithBackslash()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl + "/api/");
            var all = await client.Get<string[]>("/testget");
            all.Should().BeEquivalentTo(new string[] { "value1", "value2" });
        }
        [TestCategory("RelativePath")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenBaseAddressNotEndWithBackslashAndPathNotStartWithBackslash()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl + "/api");
            var all = await client.Get<string[]>("testget");
            all.Should().BeEquivalentTo(new string[] { "value1", "value2" });
        }
        [TestCategory("RelativePath")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenBaseAddressEndWithBackslashAndPathNotStartWithBackslash()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl + "/api/");
            var all = await client.Get<string[]>("testget");
            all.Should().BeEquivalentTo(new string[] { "value1", "value2" });
        }
        #endregion

        #region GET
        [TestCategory("GET")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenNoArgs()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var all = await client.Get<string[]>("api/testget");
            all.Should().BeEquivalentTo(new string[] { "value1", "value2" });
        }

        [TestCategory("GET")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenAddHeaderDictionary()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var all = await client.Get<string[]>("/api/testget/abc",
                null,
                new Dictionary<string, string>
                {
                    ["arg2"] = "22"
                });
            all.Should().BeEquivalentTo(new string[] { "abc", "22" });
        }

        [TestCategory("GET")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenAddQueryDictionary()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var all = await client.Get<string[]>("/api/testget/abc",
                new Dictionary<string, string>
                {
                    ["Arg1"] = "11"
                });
            all.Should().BeEquivalentTo(new string[] { "abc", "11" });
        }
        [TestCategory("GET")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenAddQueryObject()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var all = await client.Get<string[]>("api/testget/abc",
                new
                {
                    Arg1 = "11"
                });
            all.Should().BeEquivalentTo(new string[] { "abc", "11" });
        }
        [TestCategory("GET")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenAddHeaderAndQueryObject()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var all = await client.Get<string[]>("/api/testget/abc",
                new
                {
                    Arg1 = "11"
                },
                new Dictionary<string, string>
                {
                    ["Arg2"] = "22"
                });
            all.Should().BeEquivalentTo(new string[] { "abc", "11", "22" });
        }

        [TestCategory("GET")]
        [TestMethod]
        public async Task ShouldGetAllValueWhenUseSendHttpAndAddHeaderAndQueryObject()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var all = await client.SendGet<string[]>(
                "/api/testget/{id}",
                new { Id = "abc" },
                new { Arg1 = 11 },
                new { Arg2 = 22 });
            all.Should().BeEquivalentTo(new string[] { "abc", "11", "22" });
        }

        #endregion

        #region POST
        [TestCategory("POST")]
        [TestMethod]
        public async Task ShouldPostJsonSuccess()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var result = await client.PostJson<Result>("api/testpost/body", new
            {
                Id = 1,
                Name = "zhangsan"
            });
            result.Should().BeEquivalentTo(new Result { Success = true, Message = "1-zhangsan" });
        }
        [TestCategory("POST")]
        [TestMethod]
        public async Task ShouldPostUrlEncodeFormSuccess()
        {
            var client = NewRestClient(TestEnvironment.TestServerUrl);
            var result = await client.PostUrlEncodeForm<Result>("api/testpost/form", new
            {
                Id = 1,
                Name = "zhangsan",
                tags = new[] { "a", null, "c" }
            });
            result.Should().BeEquivalentTo(new Result { Success = true, Message = "1-zhangsan-a--c" });
        }
        #endregion

        private RestClient NewRestClient(string baseAddress)
        {
            var restInfo = new RestInfo(baseAddress);
            var factory = this.GetService<IHttpClientFactory>();
            return new RestClient(restInfo, factory.CreateClient());
        }
    }
    public class Result
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
