using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

// Prism
using Prism.Mvvm;
using Prism.Interactivity.InteractionRequest;

namespace Main.Common {

    public abstract class AddBindableBase : BindableBase, INotifyDataErrorInfo {
        public ErrorsContainer<string> errorsContainer { get; set; }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public AddBindableBase() {
            this.errorsContainer = new ErrorsContainer<string>(OnErrorsChanged);
            NotificationRequest = new InteractionRequest<INotification>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();
        }


        /*
         * Prism : NotificationRequest, ConfirmationRequest
         */
        public InteractionRequest<INotification> NotificationRequest { get; set; }
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; set; }
        
        public string ConfirmationRequestResult { get; set; }


        /*
         * Prism : ErrorContainer
         */
        /// <summary>
        /// ValidateProperty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void ValidateProperty(object value, [CallerMemberName] string propertyName = null) {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors)) {
                var errors = validationErrors.Select(error => error.ErrorMessage);
                this.errorsContainer.SetErrors(propertyName, errors);
            } else {
                this.errorsContainer.ClearErrors(propertyName);
            }
        }

        /// <summary>
        /// HasErrors
        /// </summary>
        /// <returns>bool</returns>
        public bool HasErrors {
            get { return this.errorsContainer.HasErrors; }
        }

        /// <summary>
        /// GetErrors
        /// </summary>
        /// <param name=" propertyName"></param>
        /// <returns>IEnumerable</returns>
        public IEnumerable GetErrors(string propertyName) {
            return this.errorsContainer.GetErrors(propertyName);
        }

        /// <summary>
        /// OnErrorsChanged
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnErrorsChanged([CallerMemberName]string propertyName = null) {
            var handler = this.ErrorsChanged;
            if (handler != null) {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// ErrorsContainer
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="OnErrorsChanged"></param>
        internal void ErrorsContainer<T1>(Action<string> OnErrorsChanged) {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Exception handle common method
        /// </summary>
        /// <param name="action">Action</param>
        public void ExecuteAction(Action action) {
            try {
                action();
            } catch (Exception ex) {
                NotificationRequest.Raise(new Notification {
                    Title = "Notification", 
                    Content = "An error occurred. Please search again.\n\n" + ex.Message
                }, r => this.ConfirmationRequestResult = "Ok");
            }
        }
    }
}
