using InvestCloudFrontTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InvestCloudFrontTest.ViewModels
{
    internal class ConfirmationViewModel : BaseViewModel,IQueryAttributable
    {
        public ICommand NavigateToSignInCommand { get; }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public ConfirmationViewModel()
        {
            NavigateToSignInCommand = new Command(OnNavigateToSignIn);
        }

        private async void OnNavigateToSignIn()
        {
            await Shell.Current.GoToAsync("///signin");
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("message"))
            {
                Message = Uri.UnescapeDataString(query["message"].ToString());
            }
        }
    }

}
