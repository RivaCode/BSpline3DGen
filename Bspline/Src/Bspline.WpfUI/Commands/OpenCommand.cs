namespace Bspline.WpfUI.Commands
{
    internal class OpenCommand : CommandBase
    {
        public OpenCommand(UiManager uiManager) 
            : base(uiManager)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return !this.UiManager.IsRecording;
        }

        public override void Execute(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load Picture";
            dialog.Filter = "XML Files (*.xml)|*.xml";
            bool? result = dialog.ShowDialog();
        }
    }
}
