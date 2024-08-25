using System.Collections;

namespace Kalendarzyk.Views.CustomControls.Buttons
{
	public partial class SelectableButtonsFlexLayoutCC : FlexLayout
	{
		private static Color _mainButtonBackgroundColor;

		public SelectableButtonsFlexLayoutCC()
		{
			_mainButtonBackgroundColor = (Color)Application.Current.Resources["MainButtonBackgroundColor"];
			InitializeComponent();
		}

		public static readonly BindableProperty ItemsSourceProperty =
			BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SelectableButtonsFlexLayoutCC), null, propertyChanged: OnItemsSourceChanged);


		public static readonly BindableProperty ButtonBackgroundProperty =
			BindableProperty.Create(nameof(ButtonBackground), typeof(Color), typeof(SelectableButtonsFlexLayoutCC), defaultValue: Colors.Black);

		public Color ButtonBackground
		{
			get => (Color)GetValue(ButtonBackgroundProperty);
			set => SetValue(ButtonBackgroundProperty, value);
		}

		public IEnumerable ItemsSource
		{
			get => (IEnumerable)GetValue(ItemsSourceProperty);
			set => SetValue(ItemsSourceProperty, value);
		}

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is SelectableButtonsFlexLayoutCC control && newValue is IEnumerable items)
			{
				BindableLayout.SetItemsSource(control, items);
			}
		}
	}
}
