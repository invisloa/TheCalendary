using Kalendarzyk.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Kalendarzyk.ViewModels.CustomControls.Buttons
{
    public class ColorButtonsSelectorViewModel : BaseViewModel, IColorButtonsSelectorHelperClass
    {
        private static readonly int NumberOfShades = 5; // Fixed number of shades to generate

        // Base colors from which shades will be generated
        private static readonly List<Color> BaseColors = new()
        {
            Color.FromArgb("#FFFF0000"),   // Red
            Color.FromArgb("#FFCD5C5C"),   // Indian Red
            Color.FromArgb("#FF008000"),   // Green
            Color.FromArgb("#FF3CB371"),   // Medium Sea Green
            Color.FromArgb("#FF0000FF"),   // Blue
            Color.FromArgb("#FF1E90FF"),   // Dodger Blue
            Color.FromArgb("#FFFFA500"),   // Orange
            Color.FromArgb("#FFFFD700"),   // Gold
            Color.FromArgb("#FFD3D3D3"),   // Light Gray
            Color.FromArgb("#FFFFFFFF")    // White
        };

        private Color _selectedColor;
        private ObservableCollection<SelectableButtonViewModel> _colorButtons;

        public ObservableCollection<SelectableButtonViewModel> ColorButtons
        {
            get => _colorButtons;
            set => SetProperty(ref _colorButtons, value);
        }

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (SetProperty(ref _selectedColor, value))
                {
                    UpdateButtonSelection();
                }
            }
        }

        public ICommand SelectedButtonCommand { get; }

        // Primary constructor
        public ColorButtonsSelectorViewModel(
            ObservableCollection<SelectableButtonViewModel> colorButtons = null,
            ICommand selectedButtonCommand = null,
            Color? startingColor = null)
        {
            ColorButtons = colorButtons ?? GenerateColorPaletteButtons();
            SelectedButtonCommand = selectedButtonCommand ?? new Command<SelectableButtonViewModel>(OnColorSelectionCommand);
            SelectedColor = startingColor ?? Colors.Blue;
        }

        private void OnColorSelectionCommand(SelectableButtonViewModel clickedButton)
        {
            SelectedColor = clickedButton.ButtonColor;
        }

        private void UpdateButtonSelection()
        {
            foreach (var button in ColorButtons)
            {
                button.IsSelected = button.ButtonColor == SelectedColor;
            }
        }

        #region Static Methods for Color Generation

        public static ObservableCollection<SelectableButtonViewModel> GenerateColorPaletteButtons()
        {
            return GenerateColorPaletteButtons(null);
        }

        public static ObservableCollection<SelectableButtonViewModel> GenerateColorPaletteButtons(ICommand selectButtonCommand)
        {
            var buttonsColorsOC = new ObservableCollection<SelectableButtonViewModel>();
            var allColorShades = GenerateColorShades(BaseColors, NumberOfShades);

            foreach (var color in allColorShades)
            {
                buttonsColorsOC.Add(new SelectableButtonViewModel
                {
                    ButtonColor = color,
                    ButtonCommand = selectButtonCommand
                });
            }

            return buttonsColorsOC;
        }

        private static List<Color> GenerateColorShades(List<Color> baseColors, int numberOfShades)
        {
            var allColorShades = new List<Color>();

            foreach (var baseColor in baseColors)
            {
                allColorShades.AddRange(GenerateShadesForBaseColor(baseColor, numberOfShades));
            }

            return allColorShades;
        }

        private static IEnumerable<Color> GenerateShadesForBaseColor(Color baseColor, int numberOfShades)
        {
            var shades = new List<Color>();
            double step = 1.0 / (numberOfShades + 1);

            for (int i = 1; i <= numberOfShades; i++)
            {
                double factor = 1.0 - step * i;

                double red = Math.Clamp(baseColor.Red * factor, 0, 1.0);
                double green = Math.Clamp(baseColor.Green * factor, 0, 1.0);
                double blue = Math.Clamp(baseColor.Blue * factor, 0, 1.0);

                shades.Add(Color.FromRgba(red, green, blue, 1));
            }

            return shades;
        }

        #endregion
    }
}
