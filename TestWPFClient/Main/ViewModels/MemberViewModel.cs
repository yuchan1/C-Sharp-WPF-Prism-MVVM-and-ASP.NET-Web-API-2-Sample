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
    public class MemberViewModel : AddBindableBase {

        IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public MemberViewModel(IUnityContainer container, IRegionManager regionManager) {
            _container = container;
            _regionManager = regionManager;

            if (MainWindowViewModel.AuthenticationType == "admin") this.UpdateEnabled = true;
            if (MainWindowViewModel.AuthenticationType == "admin") this.DeleteEnabled = true;

            SearchCommand = new DelegateCommand(executeSearch);
            ClearCommand = new DelegateCommand(executeClear);
            UpdateCommand = new DelegateCommand(executeUpdate, CanExecute)
                .ObservesProperty(() => this.MemberID)
                .ObservesProperty(() => this.MemberName);
            DeleteCommand = new DelegateCommand(executeDelete, CanExecute)
                .ObservesProperty(() => this.MemberID)
                .ObservesProperty(() => this.MemberName);

            ConfirmationRequestResult = "";
            getComboBoxLists();
            this.Items = new ObservableCollection<Member>();
            this.SelectedItem = new Member();
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
        public ObservableCollection<Member> Items { get; private set; }


        // Use DataGrid Selected Item
        private Member _SelectedItem;
        public Member SelectedItem {
            get { return _SelectedItem; }
            set {
                this.SetViewModelProperties(value);
                this.SetProperty(ref this._SelectedItem, value);

                this.CanIDEnabled = (this.MemberName == "(New)") ? true : false;
                OnPropertyChanged(PropertyGet.GetPropertyName(() => CanIDEnabled));
            }
        }


        private string _MemberID = "";

        [StringLength(20, ErrorMessage = "Please input {1} characters or less.")]
        [RegularExpression("[0-9a-z]+", ErrorMessage = "Please input Numeric or Lowercase characters.")]
        public string MemberID {
            get { return this._MemberID; }
            set {
                this.SetProperty(ref this._MemberID, value);
                this.ValidateProperty(value);
            }
        }


        private string _MemberName;

        [StringLength(100, ErrorMessage = "Please input {1} characters or less.")]
        public string MemberName {
            get { return this._MemberName; }
            set {
                this.SetProperty(ref this._MemberName, value);
                this.ValidateProperty(value);
            }
        }


        private string _LoginPassword;

        [StringLength(20, ErrorMessage = "Please input {1} characters or less.")]
        [RegularExpression("[0-9a-z]+", ErrorMessage = "Please input Numeric or Lowercase characters.")]
        public string LoginPassword {
            get { return this._LoginPassword; }
            set {
                this.SetProperty(ref this._LoginPassword, value);
                this.ValidateProperty(value);
            }
        }


        private string _AuthorityID;
        public string AuthorityID {
            get { return this._AuthorityID; }
            set {
                this.SetProperty(ref this._AuthorityID, value);
            }
        }


        private string _Email;

        [StringLength(100, ErrorMessage = "Please input {1} characters or less.")]
        [RegularExpression("[0-9a-z@.]+", ErrorMessage = "Please input mail address format")]
        public string Email {
            get { return this._Email; }
            set {
                this.SetProperty(ref this._Email, value);
                this.ValidateProperty(value);
            }
        }


        private string _Remarks;

        [StringLength(255, ErrorMessage = "Please input {1} characters or less.")]
        public string Remarks {
            get { return this._Remarks; }
            set {
                this.SetProperty(ref this._Remarks, value);
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


        private bool _Flag;
        public bool Flag {
            get { return this._Flag; }
            set {
                this.SetProperty(ref this._Flag, value);
            }
        }


        public string Update { get; private set; }

        public byte[] RowVersion { get; private set; }


        /// <summary>
        /// ViewModel each properties　→　ViewModel Member SelectedItem
        /// </summary>
        /// <param name="source"></param>
        public void SetItem() {
            if (this.MemberID == "") return;
            this.SelectedItem.MemberID = this.MemberID;
            this.SelectedItem.MemberName = this.MemberName;
            this.SelectedItem.LoginPassword = this.LoginPassword;
            this.SelectedItem.AuthorityID = this.AuthorityID;
            this.SelectedItem.Email = this.Email;
            this.SelectedItem.Remarks = this.Remarks;
            this.SelectedItem.Order = int.Parse(this.Order);
            this.SelectedItem.Flag = this.Flag;
            this.SelectedItem.Update = DateTime.Now;
            this.SelectedItem.RowVersion = this.RowVersion;
        }


        /// <summary>
        /// Member source　→　ViewModel each properties
        /// </summary>
        /// <param name="source">Member</param>
        public void SetViewModelProperties(Member source) {
            if (source == null) return;
            this.MemberID = source.MemberID;
            this.MemberName = source.MemberName;
            this.LoginPassword = source.LoginPassword;
            this.AuthorityID = source.AuthorityID;
            this.Email = source.Email;
            this.Remarks = source.Remarks;
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
                HttpResponseMessage response = MainWindowViewModel._httpClient.GetAsync("api/Members").Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();

                var q = response.Content.ReadAsAsync<IEnumerable<Member>>().Result
                    .Where(p => p.MemberName.Contains(Keyword))
                    .OrderBy(p => p.Order);

                this.Items = null;
                this.Items = new ObservableCollection<Member>(q);
                var temp = new Member { MemberName = "(New)", Order = 0, Flag = true, Update = DateTime.Now };
                this.Items.Insert(0, temp);
                this.SelectedItem = temp;
                ItemPropertyChanged();
            };

            ExecuteAction(a);

            getComboBoxLists();
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
                HttpResponseMessage response = MainWindowViewModel._httpClient.PostAsJsonAsync("api/Members/", this.SelectedItem).Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();
                var m = response.Content.ReadAsAsync<Member>().Result;

                this.Items.Remove(this.SelectedItem);
                this.Items.Insert(0, m);
                this.SelectedItem = m;
                var temp = new Member { MemberName = "(New)", Order = 0, Flag = true, Update = DateTime.Now };
                this.Items.Insert(0, temp);               
                ItemPropertyChanged();
            };

            Action update = () => {
                SetItem();
                HttpResponseMessage response = MainWindowViewModel._httpClient.PutAsJsonAsync("api/Members/" + SelectedItem.MemberID, this.SelectedItem).Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();
                var m = response.Content.ReadAsAsync<Member>().Result;

                int i = Items.IndexOf(this.SelectedItem);
                this.Items.Remove(this.SelectedItem);
                this.Items.Insert(i, m);
                this.SelectedItem = m;
                ItemPropertyChanged();
            };

            Action a = (SelectedItem.MemberID == null || SelectedItem.MemberID == "") ? add : update;
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
                HttpResponseMessage response = MainWindowViewModel._httpClient.DeleteAsync("api/Members/" + SelectedItem.MemberID).Result;
                if (!response.IsSuccessStatusCode) throw new HttpRequestException();
                var m = response.Content.ReadAsAsync<Member>().Result;

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
                && !string.IsNullOrWhiteSpace(this.MemberID)
                && !string.IsNullOrWhiteSpace(this.MemberName);

            return b;
        }


        /*
         * Collection use DataGridComboBoxColumn / ComboBox 
         */
        public List<Authority> AuthorityLists { get; set; }

        public void getComboBoxLists() {
            this.AuthorityLists = null;

            Action a = () => {
                this.AuthorityLists = new List<Authority>(Models.AuthorityLists.Get());
            };

            ExecuteAction(a);
        }
    }
}