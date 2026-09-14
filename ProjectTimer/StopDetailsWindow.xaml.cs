using System.Windows;

namespace ProjectTimer
{
    public partial class StopDetailsWindow : Window
    {
        public string Reason { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsComplete { get; private set; }
        public bool IsIncomplete { get; private set; }

        public StopDetailsWindow()
        {
            InitializeComponent();
        }

        private void CompleteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IncompleteCheckBox.IsChecked = false;
        }

        private void IncompleteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CompleteCheckBox.IsChecked = false;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!CompleteCheckBox.IsChecked.GetValueOrDefault() && !IncompleteCheckBox.IsChecked.GetValueOrDefault())
            {
                MessageBox.Show("Please mark this entry as Complete or Incomplete.", "Missing Status",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Reason = ReasonTextBox.Text.Trim();
            Description = DescriptionTextBox.Text.Trim();
            IsComplete = CompleteCheckBox.IsChecked.GetValueOrDefault();
            IsIncomplete = IncompleteCheckBox.IsChecked.GetValueOrDefault();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
