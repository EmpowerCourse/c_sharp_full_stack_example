using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public interface ISettingsService
    {
        string GetStringValue(string key);
        int GetIntValue(string key);
        bool GetBoolValue(string key);
        object GetValue(string key);
    }
}
