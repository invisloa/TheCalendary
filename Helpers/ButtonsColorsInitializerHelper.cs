using Kalendarzyk.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Graphics;
using Kalendarzyk.ViewModels.CustomControls.Buttons;

namespace Kalendarzyk.Helpers
{
    public static class ButtonsColorsInitializerHelper
    {
        private static readonly int NumberOfColumns = 10;

        public static ObservableCollection<SelectableButtonViewModel> InitializeColorButtons()
        {
            var buttonsColorsOC = new ObservableCollection<SelectableButtonViewModel>();

            // Define base color groups using ARGB
            var baseColors = new List<Color>
            {
                Color.FromArgb("#FFFF0000"), // Reds
                Color.FromArgb("#FFCD5C5C"),
                Color.FromArgb("#FF008000"), // Greens
                Color.FromArgb("#FF3CB371"),
                Color.FromArgb("#FF0000FF"), // Blues
                Color.FromArgb("#FF1E90FF"),
                Color.FromArgb("#FFFFA500"), // Oranges & Yellows
                Color.FromArgb("#FFFFD700"),
                // Add more base colors as needed
            };

            // Randomize the order of the colors to mix the palette
            var rnd = new Random();
            var randomizedColors = baseColors.OrderBy(x => rnd.Next()).ToList();

            // Generate and add shades for each base color
            foreach (var baseColor in randomizedColors)
            {
                AddColors(buttonsColorsOC, GenerateShades(baseColor));
            }

            return buttonsColorsOC;
        }

        private static void AddColors(ObservableCollection<SelectableButtonViewModel> buttonsColorsOC, IEnumerable<Color> colors)
        {
            foreach (var color in colors)
            {
                buttonsColorsOC.Add(new SelectableButtonViewModel { ButtonColor = color });
            }
        }

        private static IEnumerable<Color> GenerateShades(Color baseColor)
        {
            var shades = new List<Color>();
            double lightnessRange = 0.5; // Range for lightening (e.g., from 1.0 for base color to 1.5 for lightest color)
            double darknessRange = 0.5; // Range for darkening (e.g., from 1.0 for base color to 0.5 for darkest color)
            double step = (lightnessRange + darknessRange) / (NumberOfColumns - 1);

            for (int i = 0; i < NumberOfColumns; i++)
            {
                double factor = 1 + lightnessRange - step * i;

                double red = Math.Max(0, Math.Min(baseColor.Red * factor, 1.0));
                double green = Math.Max(0, Math.Min(baseColor.Green * factor, 1.0));
                double blue = Math.Max(0, Math.Min(baseColor.Blue * factor, 1.0));

                shades.Add(Color.FromRgba(
                    red,
                    green,
                    blue,
                    baseColor.Alpha // Use the alpha from the base color
                ));
            }

            return shades;
        }
    }
}
