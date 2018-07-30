using Armoire.Common;
using Microsoft.Extensions.Configuration;
using System;

namespace Armoire.Services
{
    public class SettingsService : ISettingsService
    {
        private IConfiguration _configuration;

        public SettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int GetIntValue(string key)
        {
            return Convert.ToInt32(_configuration[key]);
        }

        public bool GetBoolValue(string key)
        {
            return Convert.ToBoolean(_configuration[key]);
        }

        public string GetStringValue(string key)
        {
            var rawValue = GetValue(key);
            return $"{rawValue}";
        }

        public object GetValue(string key)
        {
            return _configuration[key];
        }
    }
}
