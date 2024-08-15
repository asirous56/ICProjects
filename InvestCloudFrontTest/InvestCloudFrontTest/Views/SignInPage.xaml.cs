using InvestCloudFrontTest.ViewModels;

namespace InvestCloudFrontTest.Views;

public partial class SignInPage : ContentPage
{
    public SignInPage(SignInViewModel signInViewModel)
    {
        InitializeComponent();
        BindingContext = signInViewModel;
    }
}