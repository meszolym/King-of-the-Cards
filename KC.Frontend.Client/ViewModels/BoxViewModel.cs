using ReactiveUI;

namespace KC.Frontend.Client.ViewModels;

public partial class BoxViewModel : ReactiveObject
{
    public HandViewModel LeftHandViewModel;
    public HandViewModel RightHandViewModel;
    
    public BoxViewModel()
    {
        LeftHandViewModel = new HandViewModel();
        RightHandViewModel = new HandViewModel();
    }
}