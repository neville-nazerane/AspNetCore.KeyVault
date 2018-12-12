using Microsoft.Azure.KeyVault.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.KeyVault
{
    class PrefixedKeyVaultSecretManager : IKeyVaultSecretManager
    {
        private readonly string prefix;

        public PrefixedKeyVaultSecretManager(string prefix)
        {
            this.prefix = prefix;
        }

        public string GetKey(SecretBundle secret)
            => secret.SecretIdentifier.Name
                        .Substring(prefix.Length + 2)
                        .Replace("--", ConfigurationPath.KeyDelimiter);

        public bool Load(SecretItem secret)
            => secret.Identifier.Name.StartsWith(prefix + "--");

    }
}
