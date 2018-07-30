using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Armoire.Common
{
    public class CookieUtility : ICookieUtility
    {
        private readonly int _defaultExpirationDays = 10;
        private readonly string _appKey;
        private readonly string _appSalt;
        private readonly ICipherService _cipherService;

        public CookieUtility() { }

        public CookieUtility(int defaultExpirationDays, string appKey, string appSalt, ICipherService cipherService)
        {
            _defaultExpirationDays = defaultExpirationDays;
            _appKey = appKey;
            _appSalt = appSalt;
            _cipherService = cipherService;
        }

        public void SetCookie(string key, string value)
        {
            SetCookie(key, value, DateTime.Now.AddDays(_defaultExpirationDays));
        }

        public void SerializeAndSetCookie(string key, object value)
        {
            var stringValue = JsonConvert.SerializeObject(value);
            SetCookie(key, stringValue);
        }

        public T DeserializeAndGetCookie<T>(string key)
        {
            var stringValue = GetCookie(key);
            if (String.IsNullOrWhiteSpace(stringValue))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(stringValue);
        }

        public void SetCookie(string key, string value, DateTime expiration)
        {
            var cookie = new HttpCookie(key)
            {
                Value = _cipherService.Encrypt<DESCryptoServiceProvider>(value, _appKey, _appSalt),
                Expires = expiration
            };
            HttpContext.Current.Request.Cookies.Add(cookie);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public string GetCookie(string key)
        {
            if (HttpContext.Current.Request.Cookies[key] != null
                && !String.IsNullOrEmpty(HttpContext.Current.Request.Cookies[key].Value))
            {
                try
                {
                    return _cipherService.Decrypt<DESCryptoServiceProvider>(
                        HttpContext.Current.Request.Cookies[key].Value, _appKey, _appSalt);
                }
                catch (FormatException)
                {
                    return null;
                }
            }
            return null;
        }

        public void RemoveCookie(string key)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(key)
            {
                Expires = DateTime.Now.AddDays(-1)
            });
        }
    }
}
