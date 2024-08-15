using InvestCloudFrontTest.ViewModels;

namespace InvestCloudFrontTest.Views;

public partial class CreateAccountPage : ContentPage
{
	public CreateAccountPage(CreateAccountViewModel createAccountViewModel)
	{
		InitializeComponent();
		BindingContext= createAccountViewModel;
	}
}