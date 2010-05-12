using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MoCS.Client.Controls.RatingControlSample
{
    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        CultureInfo _cultureInfo = new CultureInfo("en-US");

        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof (double), typeof (RatingControl),
                                        new FrameworkPropertyMetadata(0.0,
                                                                      FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
                                                                      RatingValueChanged));


        private double _maxValue = 10;

        public RatingControl()
        {
            InitializeComponent();
        }

        public double RatingValue
        {
            get { return (double) GetValue(RatingValueProperty); }
            set
            {
                if (value < 0)
                {
                    SetValue(RatingValueProperty, 0);
                }
                else if (value > _maxValue)
                {
                    SetValue(RatingValueProperty, _maxValue);
                }
                else
                {
                    SetValue(RatingValueProperty, value);
                }
            }
        }

        private static void RatingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RatingControl parent = sender as RatingControl;
            double ratingValue = (double)e.NewValue;

            int numberOfButtonsToHighlight = (int)(ratingValue);

            UIElementCollection children = ((StackPanel) (parent.Content)).Children;
            ToggleButton button = null;

            //turn the right buttons on
            for (int i = 0; i < numberOfButtonsToHighlight; i++)
            {
                button = children[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = true;
            }

            //turn the rest of them off
            for (int i = numberOfButtonsToHighlight; i < children.Count; i++)
            {
                button = children[i] as ToggleButton;
                if (button != null)
                    button.IsChecked = false;
            }
        }

        private void RatingButtonClickEventHandler(Object sender, RoutedEventArgs e)
        {
            //ToggleButton button = sender as ToggleButton;

            //double newRating = double.Parse((String)button.Tag, _cultureInfo.NumberFormat);
            //if(RatingValue==newRating && newRating==0.5)
            //{
            //    RatingValue = 0.0;
            //}
            //else
            //{
            //    RatingValue = newRating;
            //}
            //e.Handled = true;
        }

        private void RatingButtonMouseEnterEventHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //ToggleButton button = sender as ToggleButton;
            //double hoverRating = double.Parse((String)button.Tag, _cultureInfo.NumberFormat);
            //int numberOfButtonsToHighlight = (int)(2*hoverRating);

            //UIElementCollection children = RatingContentPanel.Children;
            
            //ToggleButton hlbutton = null;

            //for (int i = 0; i < numberOfButtonsToHighlight; i++)
            //{
            //    hlbutton = children[i] as ToggleButton;
            //    if (hlbutton != null)
            //        hlbutton.IsChecked = true;
            //}

            //for (int i = numberOfButtonsToHighlight; i < children.Count; i++)
            //{
            //    hlbutton = children[i] as ToggleButton;
            //    if (hlbutton != null)
            //        hlbutton.IsChecked = false;
            //}
        }

        private void RatingButtonMouseLeaveEventHandler(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //double ratingValue = RatingValue;
            //int numberOfButtonsToHighlight = (int)(2 * ratingValue);

            //UIElementCollection children = RatingContentPanel.Children;
            //ToggleButton button = null;

            //for (int i = 0; i < numberOfButtonsToHighlight; i++)
            //{
            //    button = children[i] as ToggleButton;
            //    if (button != null)
            //        button.IsChecked = true;
            //}

            //for (int i = numberOfButtonsToHighlight; i < children.Count; i++)
            //{
            //    button = children[i] as ToggleButton;
            //    if (button != null)
            //        button.IsChecked = false;
            //}
        }
    }
}