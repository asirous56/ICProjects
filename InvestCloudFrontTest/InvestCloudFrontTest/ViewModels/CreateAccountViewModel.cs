using InvestCloudFrontTest.Models;
using InvestCloudFrontTest.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace InvestCloudFrontTest.ViewModels
{
    public class CreateAccountViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private string _phone;
        private string _password;
        private string _errorFN;
        private string _errorLN;
        private string _errorUN;
        private string _errorPN;
        private string _errorPD;
        private string _errorSD;
        private string _username;
        private bool _isCreateAccountEnabled;
        private DateTime _serviceStartDate = DateTime.Now;
        private IUserService _userService;
        private readonly IEncryptionService _encryptionService;

        public string FirstName
        {
            get => _firstName;
            set
            {
                SetProperty(ref _firstName, value);
                ValidateInputs();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                SetProperty(ref _lastName, value);
                ValidateInputs();
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                SetProperty(ref _phone, value);
                ValidateInputs();
            }
        }

        public string UserName
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidateInputs();
            }
        }

        public DateTime ServiceStartDate
        {
            get => _serviceStartDate;
            set
            {
                SetProperty(ref _serviceStartDate, value);
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


        public string ErrorFN
        {
            get => _errorFN;
            set
            {
                SetProperty(ref _errorFN, value);
            }
        }

        public string ErrorLN
        {
            get => _errorLN;
            set
            {
                SetProperty(ref _errorLN, value);
            }
        }
        public string ErrorPN
        {
            get => _errorPN;
            set
            {
                SetProperty(ref _errorPN, value);
            }
        }
        public string ErrorUN
        {
            get => _errorUN;
            set
            {
                SetProperty(ref _errorUN, value);
            }
        }
        public string ErrorPD
        {
            get => _errorPD;
            set
            {
                SetProperty(ref _errorPD, value);
            }
        }
        public string ErrorSD
        {
            get => _errorSD;
            set
            {
                SetProperty(ref _errorSD, value);
            }
        }

        public bool IsCreateAccountEnabled
        {
            get => _isCreateAccountEnabled;
            set => SetProperty(ref _isCreateAccountEnabled, value);
        }
        public ICommand CreateAccountCommand { get; }


        public CreateAccountViewModel(IUserService userService, IEncryptionService encryptionService)
        {
            CreateAccountCommand = new Command(OnCreateAccount);
            _userService = userService;
            _encryptionService = encryptionService;
        }

        private void ValidateInputs()
        {
            IsCreateAccountEnabled =
                             ValidateFirstName(FirstName) &&
                             ValidateLastName(LastName) &&
                             ValidatePhoneNumber(Phone) &&
                             ValidateUserName(UserName) &&
                             ValidatePassword(Password) &&
                             ValidateServiceStartDate(ServiceStartDate);
        }

        private bool ValidateFirstName(string firstName)
        {
            ErrorFN = "";
            string error;
            bool isValid = ValidateName(firstName, out error);
            if (!isValid)
                ErrorFN = $"FirstName {error}";
            return isValid;
        }
        private bool ValidateLastName(string lastName)
        {
            ErrorLN = "";
            string error;
            bool isValid = ValidateName(lastName, out error);
            if (!isValid)
                ErrorLN = $"LastName {error}";
            return isValid;
        }

        private bool ValidateUserName(string userName)
        {
            ErrorUN = "";
            string error;
            bool isValid = ValidateName(userName, out error);
            if (!isValid)
                ErrorUN = $"UserName {error}";
            return isValid;
        }

        private bool ValidateName(string name, out string message)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                message = "Can not be empty or blank";
                return false;
            }
            bool isValid = !Regex.IsMatch(name, @"[!@#$%^&*]");
            message = $"should not contain any of !@#$%^&* ";
            return isValid;
        }
        private bool ValidatePassword(string password)
        {
            ErrorPD = "";
            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorPD = "Password Can not be empty or blank";
                return false;
            }
            if (password.Length < 8 || password.Length > 15)
            {
                ErrorPD = "password.Length < 8 || password.Length > 15";
                return false;
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                ErrorPD = "At least one lowercase";
                return false;
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                ErrorPD = "At least one uppercase";
                return false;
            }

            if (Regex.IsMatch(password, @"(\w+)\1"))
            {
                ErrorPD = "No repetitive sequences";
                return false;
            }

            return true;
        }
        private bool ValidateServiceStartDate(DateTime date)
        {
            ErrorSD = "";
            var today = DateTime.Today;
            if (date < today)
            {
                ErrorSD = "past date is not allowed";
                return false; // Past date not allowed
            }

            if ((date - today).TotalDays > 30)
            {
                ErrorSD = "too early to create an account";
                return false; // More than 30 days into the future not allowed
            }

            return true;
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            ErrorPN = "";
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                ErrorPN = "Phone number Can not be empty or blank";
                return false;
            }
            bool isValid = Regex.IsMatch(phoneNumber, @"^\(\d{3}\)-\d{3}-\d{4}$");
            if (!isValid)
                ErrorPN = "Phone number format should be (123)-456-7890";
            return isValid;
        }

        private async void OnCreateAccount()
        {
            User user = new User()
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                PhoneNumber = this.Phone,
                ServiceStartDate = this.ServiceStartDate,
                Username = this.UserName
            ,
                Password = _encryptionService.HashInput(this.Password)
            };
            var result = await _userService.CreateAccountAsync(user);
            if (result.Successfull)
            {
                await Shell.Current.GoToAsync($"///confirmation?message={Uri.EscapeDataString(result.Error)}");
            }
            else if (user.Password != Password)
            {
                await Application.Current.MainPage.DisplayAlert("Error", result.Error, "OK");
            }

        }


    }

}
