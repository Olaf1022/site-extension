﻿using LetsEncrypt.Azure.Core.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace LetsEncrypt.Azure.Core.Services
{
    public class LocalFileSystemAuthorizationChallengeProvider : BaseHttpAuthorizationChallengeProvider
    {       
        private readonly IAzureWebAppEnvironment azureEnvironment;
        private readonly IAuthorizationChallengeProviderConfig config;
        private readonly PathProvider pathProvider;

        public LocalFileSystemAuthorizationChallengeProvider(IAzureWebAppEnvironment azureEnvironment, IAuthorizationChallengeProviderConfig config)
        {
            this.azureEnvironment = azureEnvironment;
            this.config = config;
            this.pathProvider = new PathProvider(azureEnvironment);
        }




        public override async Task EnsureDirectory()
        {
            var dir = await this.pathProvider.ChallengeDirectory(false);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public override async Task EnsureWebConfig()
        {
            var dir = await this.pathProvider.ChallengeDirectory(false);
            var webConfigPath = Path.Combine(dir, "web.config");
            if (config.DisableWebConfigUpdate)
            {
                Trace.TraceInformation($"Disabled updating web.config at {webConfigPath}");
            }
            else
            {
                if ((!File.Exists(webConfigPath) || File.ReadAllText(webConfigPath) != webConfig))
                {
                    Trace.TraceInformation($"Writing web.config to {webConfigPath}");
                    File.WriteAllText(webConfigPath, webConfig);
                }
            }
        }

        public override async Task PersistsChallengeFile(string filePath, string fileContent)
        {
            string answerPath = await GetAnswerPath(filePath);

            Console.WriteLine($" Writing challenge answer to {answerPath}");
            Trace.TraceInformation("Writing challenge answer to {0}", answerPath);

            File.WriteAllText(answerPath, fileContent);
        }

        private async Task<string> GetAnswerPath(string filePath)
        {
            var rootDir = await this.pathProvider.WebRootPath(false);
            // We need to strip off any leading '/' in the path
            if (filePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                filePath = filePath.Substring(1);
            var answerPath = Environment.ExpandEnvironmentVariables(Path.Combine(rootDir, filePath));
            return answerPath;
        }

        public override async Task CleanupChallengeFile(string filePath)
        {
            File.Delete(await GetAnswerPath(filePath));
        }
    }
}
