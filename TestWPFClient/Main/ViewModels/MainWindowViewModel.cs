using System;
using System.Security.Principal;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Regions;
using Prism.Interactivity.InteractionRequest;

using Main.Common;
using Main.Models;
using Main.Views;

namespace Main.ViewModels {

    public class MainWindowViewModel : AddBindableBase {

        IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        public static HttpClient _httpClient;

        public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager) {
            _container = container;
            _regionManager = regionManager;

            var cookie = new CookieContainer();
            _httpClient = new HttpClient(new HttpClientHandler { CookieContainer = cookie });
            _httpClient.BaseAddress = new Uri("http://localhost:58192/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            NavigateCommand = new DelegateCommand<string>(Navigate);

            //view discovery
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(Login));
        }


        public DelegateCommand<string> NavigateCommand { get; private set; }

        private void Navigate(string navigatePath) {
            if (navigatePath != null) _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }

        // User info
        public static string UserName { get; set; }
        public static string AuthenticationType { get; set; }

    }
}
