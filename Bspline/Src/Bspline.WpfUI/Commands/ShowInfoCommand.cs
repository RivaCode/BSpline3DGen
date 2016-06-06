namespace Bspline.WpfUI.Commands
{
    internal class ShowInfoCommand : CommandBase
    {
        public override bool CanExecute( object parameter )
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            string messageBoxText = "need to insert file in this place";
            string caption = "Info tool box";
            MessageBox.Show( messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.Information );
        }
    }
}
