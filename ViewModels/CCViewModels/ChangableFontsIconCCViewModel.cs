using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Kalendarzyk.ViewModels;

namespace Kalendarzyk.Views.CustomControls.CCViewModels
{
    public class ChangableFontsIconCCViewModel : BaseViewModel
    {
        private bool _isSelected;
        private ICommand _isSelectedCommand;
        private string _iconFontText;
        private string _selectedIconFontText;
        private string _notSelectedIconFontText;

        public ChangableFontsIconCCViewModel(bool isSelected, string selectedIconText, string notSelectedIconText)
        {
            IsSelected = isSelected;
            SelectedIconFontText = selectedIconText;
            NotSelectedIconFontText = notSelectedIconText;
            IsSelectedCommand = new RelayCommand(OnIsSelectedCommand);
            IconFontText = IsSelected ? SelectedIconFontText : NotSelectedIconFontText;
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    IconFontText = IsSelected ? SelectedIconFontText : NotSelectedIconFontText;
                }
            }
        }

        public ICommand IsSelectedCommand
        {
            get => _isSelectedCommand;
            set => SetProperty(ref _isSelectedCommand, value);
        }

        public string IconFontText
        {
            get => _iconFontText;
            set => SetProperty(ref _iconFontText, value);
        }

        public string SelectedIconFontText
        {
            get => _selectedIconFontText;
            set => SetProperty(ref _selectedIconFontText, value);
        }

        public string NotSelectedIconFontText
        {
            get => _notSelectedIconFontText;
            set => SetProperty(ref _notSelectedIconFontText, value);
        }

        private void OnIsSelectedCommand()
        {
            IsSelected = !IsSelected;
        }
    }
}
