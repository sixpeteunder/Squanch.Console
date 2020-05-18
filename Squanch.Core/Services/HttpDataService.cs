﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Flurl;
using Flurl.Http;

namespace Squanch.Core.Services
{
    class HttpDataService
    {
        private readonly Dictionary<string, object> responseCache;
        private readonly string baseAddress = "";

        public HttpDataService(string defaultBaseUrl = "")
        {
            if (!string.IsNullOrEmpty(defaultBaseUrl))
                baseAddress = $"{defaultBaseUrl}/";

            responseCache = new Dictionary<string, object>();
        }

        public async Task<T> GetAsync<T>(string uri, Dictionary<string, string> parameters = null, bool forceRefresh = false)
        {
            T result;
            uri = $"{baseAddress}{uri}";
            if (parameters != null) uri.SetQueryParams(parameters);
            
            if (forceRefresh || !responseCache.ContainsKey(uri))
            {
                result = await uri.GetJsonAsync<T>();

                if (responseCache.ContainsKey(uri)) 
                    responseCache[uri] = result;
                else responseCache.Add(uri, result);
            }
            else result = (T)responseCache[uri];

            return result;
        }

        public async Task<T> GetFromFullUriAsync<T>(string uri, Dictionary<string, string> parameters = null, bool forceRefresh = false)
            => await GetAsync<T>(uri.Substring(baseAddress.Length), parameters, forceRefresh);
    }
}