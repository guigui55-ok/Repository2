namespace ImageViewer.Values
{
    public class ViewImageSettings : IViewImageSettings
    {
        public bool _isViewAlwaysCenter = true;

        bool IViewImageSettings.IsViewAlwaysCenter { get { return _isViewAlwaysCenter; } set { _isViewAlwaysCenter = value; } }
    }
}
