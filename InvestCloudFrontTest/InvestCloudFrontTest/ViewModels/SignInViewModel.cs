using InvestCloudFrontTest.Models;
using InvestCloudFrontTest.Services;
using System.Windows.Input;

namespace InvestCloudFrontTest.ViewModels
{
    public class SignInViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private bool _isSignInEnabled;
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidateInputs();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidateInputs();
            }
        }

        public bool IsSignInEnabled
        {
            get => _isSignInEnabled;
            set => SetProperty(ref _isSignInEnabled, value);
        }

        public ICommand SignInCommand { get; }
        public ICommand NavigateToCreateAccountCommand { get; }

         
        public SignInViewModel(IUserService userService,IEncryptionService encryptionService)
        {
            SignInCommand = new Command(OnSignIn);
            NavigateToCreateAccountCommand = new Command(OnNavigateToCreateAccount);
            _userService = userService;
            _encryptionService = encryptionService;
        }

        private void ValidateInputs()
        {
            IsSignInEnabled = !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        private async void OnSignIn()
        {
            User user = new User { Username = _username, Password = _encryptionService.HashInput(_password) };
            var response = await _userService.ValidUserAsync(user);
            if (!response.Successfull)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Error, "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Welcome", response.Error, "OK");
            }
        }

        private async void OnNavigateToCreateAccount()
        {
            await Shell.Current.GoToAsync("///createaccount");
        }




    }

}
