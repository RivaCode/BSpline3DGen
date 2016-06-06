namespace Bspline.WpfUI.Commands
{
    internal class SaveAsCommand : CommandBase
    {
        public SaveAsCommand(UiManager uiManager)
            : base( uiManager )
        {
        }

        public override bool CanExecute(object parameter)
        {
            return !this.UiManager.IsRecording;
        }

        public override void Execute(object parameter)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Picture";
            dialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            dialog.FileName = "NewImage";
            bool? result = dialog.ShowDialog();

        }
    }
}
