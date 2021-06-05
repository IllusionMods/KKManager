namespace KKManager.Windows.Content
{
    public interface IContentWindow
    {
        void RefreshList();
        void CancelRefreshing();
        void DeserializeContent(string contentString);
    }
}