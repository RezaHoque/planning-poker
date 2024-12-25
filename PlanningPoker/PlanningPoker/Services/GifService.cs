
using DotNetEnv.Extensions;
using log4net;

namespace PlanningPoker.Services
{
    public class GifService : IgifService
    {
        private readonly IConfiguration _configuration;
        private readonly ILog _log;
        public GifService(IConfiguration configuration, ILog log)
        {
            throw new NotImplementedException();
        }
        public Task<string> GetGif(string keyWord)
        {
            var dict = DotNetEnv.Env.NoEnvVars().Load().ToDotEnvDictionary();

            return Task.FromResult(dict[keyWord]);
        }
    }
}
