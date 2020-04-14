using System.IO;
using FluentAssertions;
using Xunit;
using static System.Environment;

namespace Trakx.Tests.Tools
{
    internal static class Secrets
    {
        static Secrets()
        {
            var srcPath = new DirectoryInfo(CurrentDirectory).Parent?.Parent?.Parent?.Parent;
            DotNetEnv.Env.Load(Path.Combine(srcPath?.FullName, ".env"));
        }

        public static string CryptoCompareApiKey => GetEnvironmentVariable("CRYPTOCOMPARE_API_KEY");
        public static string InfuraApiKey => GetEnvironmentVariable("INFURA_API_KEY");
        public static string EthereumWalletSecret => GetEnvironmentVariable("ETHERWALLET");
    }

    public class ToBeDeleted
    {
        [Fact(Skip = "Should only work if you have created a .env file at in the 'src' folder.")]
        //[Fact]
        public void Secrets_should_find_env_file_in_src_and_import_environment_variables()
        {
            Secrets.CryptoCompareApiKey.Should().NotBeNullOrWhiteSpace();
        }
    }
}
