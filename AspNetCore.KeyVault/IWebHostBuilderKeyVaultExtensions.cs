using AspNetCore.KeyVault;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Hosting
{
    public static class IWebHostBuilderKeyVaultExtensions
    {

        public static IWebHostBuilder UseKeyVaultConfiguration(
                                                this IWebHostBuilder builder,
                                                string vaultEndPoint,
                                                bool optional = false
                                            )
            => builder.UseKeyVaultConfiguration(vaultEndPoint, null, optional);

        public static IWebHostBuilder UseKeyVaultConfiguration(
                                        this IWebHostBuilder builder,
                                        string vaultEndPoint,
                                        string prefix,
                                        bool optional = false
                                    )
        {
            var config = KeyVaultConfiguration(vaultEndPoint, prefix);
            if (!optional || config != null)
                builder.UseConfiguration(config);
            return builder;
        }

        public static IWebHostBuilder UseKeyVaultConfiguration(
                                        this IWebHostBuilder builder,
                                        string vaultEndPoint,
                                        IConfiguration alternateConfiguration
                                    )
            => builder.UseKeyVaultConfiguration(vaultEndPoint, null, alternateConfiguration);

        public static IWebHostBuilder UseKeyVaultConfiguration(
                                this IWebHostBuilder builder,
                                string vaultEndPoint,
                                string prefix,
                                IConfiguration alternateConfiguration
                            )
        {
            var config = KeyVaultConfiguration(vaultEndPoint, prefix);
            if (config == null)
                builder.UseConfiguration(alternateConfiguration);
            else builder.UseConfiguration(config);
            return builder;
        }

        static IConfiguration KeyVaultConfiguration(string vaultEndPoint, string prefix = null)
        {
            if (vaultEndPoint == null) return null;
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            azureServiceTokenProvider.KeyVaultTokenCallback));
            IKeyVaultSecretManager manager = null;
            if (prefix == null) manager = new DefaultKeyVaultSecretManager();
            else manager = new PrefixedKeyVaultSecretManager(prefix);
            return new ConfigurationBuilder()
                    .AddAzureKeyVault(vaultEndPoint, keyVaultClient, manager)
                    .Build();
        }

    }
}
