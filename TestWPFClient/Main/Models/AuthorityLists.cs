using System;
using System.Collections.Generic;
using System.Linq;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

using Main.Common;
using Main.ViewModels;

namespace Main.Models {
    internal static class AuthorityLists {
        public static List<Authority> Get() { 
            List<Authority> x = new List<Authority>();

            HttpResponseMessage response = MainWindowViewModel._httpClient.GetAsync("api/Authorities").Result;
            if (response.IsSuccessStatusCode) {
                var q = response.Content.ReadAsAsync<IEnumerable<Authority>>().Result
                    .OrderBy(p => p.Order);

                x = new List<Authority>(q);
            }

            return x;  
        }
    }
}
