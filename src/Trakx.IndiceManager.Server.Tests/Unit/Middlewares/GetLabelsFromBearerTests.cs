using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Trakx.Common.Models;
using Trakx.Common.Utils;
using Trakx.IndiceManager.Server.Middlewares;
using Xunit;

namespace Trakx.IndiceManager.Server.Tests.Unit.Middlewares
{
    public class GetLabelsFromBearerTests
    {
        private const string Uid = "123456789";
        private readonly IMemoryCache _cache;
        private readonly ServiceProvider _serviceProvider;


        public GetLabelsFromBearerTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddAuthentication("Bearer").AddJwtBearer();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task GetLabelsFromBearer_should_put_labels_in_cache()
        {
            var middleware = new GetLabelsFromBearer(innerHttpContext => Task.CompletedTask, _cache);

            var context = new DefaultHttpContext { RequestServices = _serviceProvider };

            context.Request.Headers.Add("Authorization", "Bearer " + MockJwtTokens.GenerateJwtToken(new List<Claim> { new Claim("uid", Uid) }).Invoke().Result);
            await middleware.Invoke(context);

            _cache.TryGetValue("bearer_labels_" + Uid, out List<Label> labels);
            labels.Count.Should().Be(MockJwtTokens.GenerateLabelsList().Count);
            labels[0].Value.Should().Be(MockJwtTokens.GenerateLabelsList()[0].Value);
            labels[0].Key.Should().Be(MockJwtTokens.GenerateLabelsList()[0].Key);
        }

        [Fact]
        public async Task GetLabelsFromBearer_should_not_put_labels_in_cache_if_there_is_no_bearer()
        {
            var middleware = new GetLabelsFromBearer(innerHttpContext => Task.CompletedTask, _cache);
            var context = new DefaultHttpContext { RequestServices = _serviceProvider };
            await middleware.Invoke(context);
            _cache.TryGetValue("bearer_labels_" + Uid, out _).Should().BeFalse();
        }
    }
}
