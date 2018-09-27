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

    //[RegionAuthorityLifetime(KeepAlive = false)]
    public class AuthorityViewModel : AddBindableBase {

        IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public AuthorityViewModel(IUnityContainer container, IRegionManager regionManager) {
            _container = container;
            _regionManager = regionManager;

            if (MainWindowViewModel.AuthenticationType == "admin") this.UpdateEnabled = true;
            if (MainWindowViewModel.AuthenticationType == "admin") this.DeleteEnabled = true;

            SearchCommand = new DelegateCommand(executeSearch);
            ClearCommand = new DelegateCommand(executeClear);
            UpdateCommand = new DelegateCommand(executeUpdate, CanExecute)
                .ObservesProperty(() => this.AuthorityID)
                .ObservesProperty(() => this.AuthorityName);
            DeleteCommand = new DelegateCommand(executeDelete, CanExecute)
                .ObservesProperty(() => this.AuthorityID)
                .ObservesProperty(() => this.AuthorityName);

            ConfirmationRequestResult = "";
            this.Items = new ObservableCollection<Authority>();
            this.SelectedItem = new Authority();

            executeSearch();

            this.CanIDEnabled = true;
        }

        public bool UpdateEnabled { get; private set; }
        public bool DeleteEnabled { get; private set; }
        public bool CanIDEnabled { get; private set; }


        // Use Search
        private string _Keyword = "";
        public string Keyword {
            get { return this._Keyword; }
            set {
                this.SetProperty(ref this._Keyword, value);
            }
        }


        // Use DataGrid
        public ObservableCollection<Authority> Items { get; private set; }


        // Use DataGrid Selected Item
        private Authority _SelectedItem;
        public Authority SelectedItem {
            get { return _SelectedItem; }
            set {
                this.SetViewModelProperties(value);
                this.SetProperty(ref this._SelectedItem, value);

                this.CanIDEnabled = (this.AuthorityName == "(New)") ? true : false;
                OnPropertyChanged(PropertyGet.GetPropertyName(() => CanIDEnabled));
            }
        }


        private string _AuthorityID;

        [StringLength(20, ErrorMessage = "Please input {1} characters or less.")]
        [RegularExpression("[0-9a-z]+", ErrorMessage = "Please input Numeric or Lowercase characters.")]
        public string AuthorityID {
            get { return this._AuthorityID; }
            set {
                this.SetProperty(ref this._AuthorityID, value);
                this.ValidateProperty(value);
            }
        }


        private string _AuthorityName;

        [StringLength(20, ErrorMessage = "Please input {1} characters or less.")]
        public string AuthorityName {
            get { return this._AuthorityName; }
            set {
                this.SetProperty(ref this._AuthorityName, value);
                this.ValidateProperty(value);
            }
        }


        private string _Order;

        [Range(0, 999999, ErrorMessage = "Please input {1} ～ {2} numeric degit")]
        public string Order {
            get { return this._Order; }
            set {
                this.SetProperty(ref this._Order, value);
                this.ValidateProperty(value);
            }
        }


        public bool _Flag;

        public bool Flag {
            get { return this._Flag; }
            set {
                this.SetProperty(ref this._Flag, value);
                //this.ValidateProperty(value);
            }
        }


        public string Update { get; private set; }

        public byte[] RowVersion { get; private set; }


        /// <summary>
        /// ViewModel each properties　→　ViewModel Authority SelectedItem
        /// </summary>
        /// <param name="source"></param>
        public void SetItem() {
            if (this.AuthorityID == "") return;
            this.SelectedItem.AuthorityID = this.AuthorityID;
            this.SelectedItem.AuthorityName = this.AuthorityName;
            this.SelectedItem.Order = int.Parse(this.Order);
            this.SelectedItem.Flag = this.Flag;
            this.SelectedItem.Update = DateTime.Now;
            this.SelectedItem.RowVersion = this.RowVersion;
        }


        /// <summary>
        /// Member source　→　ViewModel each properties
        /// </summary>
        /// <param name="source">Authority</param>
        public void SetViewModelProperties(Authority source) {
            if (source == null) return;
            this.AuthorityID = source.AuthorityID;
            this.AuthorityName = source.AuthorityName;
            this.Order = source.Order.ToString();
            this.Flag = source.Flag;
            this.Update = source.Update.ToString();
            this.RowVersion = source.RowVersion;
        }


        /// <summary>
        /// executeSearch(DelegateCommand)
        /// </summary>
        public void executeSearch() {
            Action a = () => {
                HttpResponseMessage response = MainWindowViewModel._httpClient.GetAsync("api/Authorities").Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();

                var q = response.Content.ReadAsAsync<IEnumerable<Authority>>().Result
                    .Where(p => p.AuthorityName.Contains(Keyword))
                    .OrderBy(p => p.Order);

                this.Items = null;
                this.Items = new ObservableCollection<Authority>(q);
                var temp = new Authority { AuthorityName = "(New)", Order = 0, Flag = true, Update = DateTime.UtcNow };
                this.Items.Insert(0, temp);
                this.SelectedItem = temp;
                ItemPropertyChanged();
            };

            ExecuteAction(a);
        }


        /// <summary>
        /// executeClear(DelegateCommand)
        /// </summary>
        public void executeClear() {
            Keyword = "";
            executeSearch();
        }


        /// <summary>
        /// executeUpdate(DelegateCommand)
        /// </summary>
        private void executeUpdate() {
            Action add = () => {
                SetItem();
                HttpResponseMessage response = MainWindowViewModel._httpClient.PostAsJsonAsync("api/Authorities/", this.SelectedItem).Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();
                var m = response.Content.ReadAsAsync<Authority>().Result;
                
                this.Items.Remove(this.SelectedItem);
                this.Items.Insert(0, m);
                this.SelectedItem = m;
                var temp = new Authority { AuthorityName = "(New)", Order = 0, Flag = true, Update = DateTime.UtcNow };
                this.Items.Insert(0, temp);
                ItemPropertyChanged();
            };

            Action update = () => {
                SetItem();
                HttpResponseMessage response = MainWindowViewModel._httpClient.PutAsJsonAsync("api/Authorities/" + SelectedItem.AuthorityID, this.SelectedItem).Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();
                var m = response.Content.ReadAsAsync<Authority>().Result;

                int i = Items.IndexOf(this.SelectedItem);
                this.Items.Remove(this.SelectedItem);
                this.Items.Insert(i, m);
                this.SelectedItem = m;
                ItemPropertyChanged();
            };

            Action a = (SelectedItem.AuthorityID == null || SelectedItem.AuthorityID == "") ? add : update;
            ExecuteAction(a);
        }


        /// <summary>
        /// executeDelete(DelegateCommand)
        /// </summary>
        private void executeDelete() {
            ConfirmationRequest.Raise(new Confirmation {
                Title = "Confirmation", Content = "Are you delete the select item?"
            }, r => ConfirmationRequestResult = r.Confirmed ? "Ok" : "Cancel");

            if (ConfirmationRequestResult != "Ok") return;

            Action a = () => {
                HttpResponseMessage response = MainWindowViewModel._httpClient.DeleteAsync("api/Authorities/" + SelectedItem.AuthorityID).Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();
                var m = response.Content.ReadAsAsync<Authority>().Result;

                this.Items.Remove(this.SelectedItem);
                this.SelectedItem = this.Items[0];
                ItemPropertyChanged();
            };

            ExecuteAction(a);
        }


        /// <summary>
        /// ItemPropertyChanged
        /// </summary>
        public void ItemPropertyChanged() {
            OnPropertyChanged(PropertyGet.GetPropertyName(() => Items));
            OnPropertyChanged(PropertyGet.GetPropertyName(() => SelectedItem));
        }


        /*
         * Prism : DelegateCommand
         */
        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }


        /// <summary>
        /// CanExecute
        /// </summary>
        /// <returns>bool</returns>
        private bool CanExecute() {
            bool b = !this.HasErrors
                && !string.IsNullOrWhiteSpace(this.AuthorityID)
                && !string.IsNullOrWhiteSpace(this.AuthorityName);

            return b;
        }
    }
}