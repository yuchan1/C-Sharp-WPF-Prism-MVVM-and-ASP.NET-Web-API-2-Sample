using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Regions;
using Prism.Interactivity.InteractionRequest;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;

using Main.Common;
using Main.Models;

namespace Main.ViewModels {

    //[RegionMemberLifetime(KeepAlive = false)]
    public class MenuViewModel : AddBindableBase {

        IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public MenuViewModel(IUnityContainer container, IRegionManager regionManager) {
            _container = container;
            _regionManager = regionManager;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            UserName = " ｢Login : " + MainWindowViewModel.UserName + "｣";
            ConfirmationRequestResult = "";
        }


        public string UserName { get; private set; }


        /*
         * Prism : DelegateCommand
         */
        public DelegateCommand AboutCommand { get; private set; }
        public DelegateCommand HelpCommand { get; private set; }



        /*
         * Prism : NavigationCommand
         */
        public DelegateCommand<string> NavigateCommand { get; private set; }

        private void Navigate(string navigatePath) {
            if (navigatePath != null) _regionManager.RequestNavigate("ContentRegion", navigatePath);
        }
    }
}
