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
    
    public class LoginParameter {
        public string UserID { get; set; }
        public string Password { get; set; }
    }

    public class LoginResult {
        public string UserName { get; set; }
        public string AuthorityID { get; set; }
    }

    //[RegionMemberLifetime(KeepAlive = false)]
    public class LoginViewModel : AddBindableBase {

        IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public LoginViewModel(IUnityContainer container, IRegionManager regionManager) {
            _container = container;
            _regionManager = regionManager;

            LoginCommand = new DelegateCommand(executeLogin, CanExecute)
                .ObservesProperty(() => this.UserID)
                .ObservesProperty(() => this.Password);
        }


        private string _UserID;

        [StringLength(20, ErrorMessage = "Please input {1} characters or less.")]
        [RegularExpression("[0-9a-z]+", ErrorMessage = "Please input Numeric or Lowercase characters.")]
        public string UserID {
            get { return _UserID; }
            set { SetProperty(ref _UserID, value); }
        }

        
        private string _Password;

        [StringLength(20, ErrorMessage = "Please input {1} characters or less.")]
        [RegularExpression("[0-9a-z]+", ErrorMessage = "Please input Numeric or Lowercase characters.")]
        public string Password {
            get { return _Password; }
            set { SetProperty(ref _Password, value); }
        }

        private string _ErrorMessage;
        public string ErrorMessage {
            get { return _ErrorMessage; }
            set { SetProperty(ref _ErrorMessage, value); }
        }


        /// <summary>
        /// executeLogin(DelegateCommand)  ※This way is Bad, Change required
        /// </summary>
        public void executeLogin() {
            Action a = () => {
                LoginParameter param = new LoginParameter { UserID = this.UserID, Password = this.Password };
                HttpResponseMessage response = MainWindowViewModel._httpClient.PostAsJsonAsync("api/Auth/", param).Result;

                if (response.IsSuccessStatusCode) {
                    LoginResult q = response.Content.ReadAsAsync<LoginResult>().Result;
                    MainWindowViewModel.UserName = q.UserName;
                    MainWindowViewModel.AuthenticationType = q.AuthorityID;

                    _regionManager.RequestNavigate("MenuRegion", "Menu");
                    _regionManager.RequestNavigate("ContentRegion", "Main");

                } else {
                    ErrorMessage = "UserID and password do not match.";
                }
            };

            ExecuteAction(a);
        }


        /// <summary>
        /// Exception handle common method
        /// </summary>
        /// <param name="action">Action</param>
        private void ExecuteAction(Action action) {
            try {
                action();
            } catch (Exception ex) {
                NotificationRequest.Raise(new Notification {
                    Content = "An error occurred. Please search again.\n\n" + ex.Message,
                    Title = "Notification"
                }, r => this.ConfirmationRequestResult = "Ok");
            }
        }
        
        
        /*
         * Prism : DelegateCommand
         */
        public DelegateCommand LoginCommand { get; private set; }
        public DelegateCommand DebugLoginCommand { get; private set; }


        /// <summary>
        /// CanExecute
        /// </summary>
        /// <returns>bool</returns>
        private bool CanExecute() {
            bool b = !string.IsNullOrWhiteSpace(this.UserID)
                && !string.IsNullOrWhiteSpace(this.Password);

            return b;
        }
    }
}
