using System;
using System.Collections.Generic;
using System.Globalization;
using AltBeaconOrg.BoundBeacon;
using Xamarin.Forms;

namespace AltBeaconLibrarySample
{
	public class MainPage : ContentPage
	{

		MainPageViewModel _vm = null;

		public MainPage()
		{
			_vm = new MainPageViewModel();
			this.BindingContext = _vm;


			Stepper stepper = new Stepper { };
			stepper.SetBinding(Stepper.MinimumProperty, "StepperMinValue");
			stepper.SetBinding(Stepper.MaximumProperty, "StepperMaxValue");
			stepper.SetBinding(Stepper.ValueProperty, "StepperValue");

			Label labelStepper = new Label() { VerticalTextAlignment = TextAlignment.Center};
			labelStepper.SetBinding(Label.TextProperty, new Binding("StepperValue", BindingMode.Default, stringFormat:( "Polling: {0} sec" )));
			StackLayout sl = new StackLayout {Orientation = StackOrientation.Horizontal, Children = {stepper, labelStepper } };

			Button buttonStartLogic = new Button { Text = "START WITH LOGIC"};
			buttonStartLogic.SetBinding(Button.CommandProperty, "StartRangingCommand");
			buttonStartLogic.SetBinding(Button.IsEnabledProperty, "IsEnabled");

			Button buttonStart = new Button { Text = "START NO LOGIC" };
			buttonStart.SetBinding(Button.CommandProperty, "StartRangingNoLogicCommand");
			buttonStart.SetBinding(Button.IsEnabledProperty, "IsEnabled");

			StackLayout slHeader = new StackLayout() { Children = {sl, buttonStartLogic, buttonStart }  };


			ListView lv = new ListView { HasUnevenRows = true, SeparatorColor = Color.Black, SeparatorVisibility = SeparatorVisibility.Default, Footer = slHeader };
			lv.SetBinding(ListView.ItemsSourceProperty, "ReceivedBeacons");
			lv.SetBinding(ListView.ItemTemplateProperty, "ViewCellBeaconTemplate");

			//lv.ItemsSource = _vm.ReceivedBeacons;
			//lv.ItemTemplate = new DataTemplate(typeof(ViewCellBeacon));

			Content = new StackLayout
			{
				Children = { lv }
			};
		}

		protected override void OnAppearing()
		{
			_vm.OnAppearing();
			base.OnAppearing();
		}

		protected override void OnDisappearing()
		{
			_vm.OnDisappearing();
			base.OnDisappearing();
		}

		public class ViewCellBeacon : ViewCell
		{

			public ViewCellBeacon()
			{

				Label labelId = new Label();
				labelId.SetBinding(Label.TextProperty, "Id");

				Label labelName = new Label { FontSize = 30 };
				labelName.SetBinding(Label.TextProperty, "Name");
				labelName.SetBinding(VisualElement.BackgroundColorProperty, new Binding("IsNew", BindingMode.Default, new IsNewToColorConverter()));

				Label labelDistance = new Label { FontSize = 25 };
				labelDistance.SetBinding(Label.TextProperty, new Binding("Distance", stringFormat: "{0:0.00} mt"));
				labelDistance.SetBinding(VisualElement.BackgroundColorProperty, new Binding("Counter", BindingMode.Default, new CounterToColorConverter()));

				//StackLayout slHorizontal = new StackLayout() { Orientation = StackOrientation.Horizontal, Children = {labelName, labelDistance } };

				StackLayout sl = new StackLayout { Children = { labelName, labelDistance } };

				View = sl;
			}

			class IsNewToColorConverter : IValueConverter
			{
				public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
				{
					if (value != null && value is bool)
					{
						if ((bool)value == true)
							return Color.Green;
					}
					return Color.White;
				}

				public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
				{
					throw new NotImplementedException();
				}
			}

			class CounterToColorConverter : IValueConverter
			{
				public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
				{
					if (value != null && value is int)
					{
						int counter = (int)value;
						switch (counter)
						{
							case 0:
							case 1:
								return Color.White;
							case 2:
								return Color.Yellow;
							default:
								return Color.Red;
						}
					}
					return Color.White;
				}

				public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
				{
					throw new NotImplementedException();
				}
			}
		}
	}
}

