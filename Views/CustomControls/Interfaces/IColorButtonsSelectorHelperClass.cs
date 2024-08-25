using Kalendarzyk.ViewModels;
using Kalendarzyk.ViewModels.CustomControls.Buttons;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kalendarzyk.ViewModels.CustomControls
{
    public interface IColorButtonsSelectorHelperClass
	{
		ObservableCollection<SelectableButtonViewModel> ColorButtons { get; set; }
		ICommand SelectedButtonCommand { get; }
		Color SelectedColor { get; set; }
	}
}