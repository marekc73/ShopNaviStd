using System.Threading.Tasks;
using Xamarin.Forms;
public interface INavigationService : INavigation
{
    Task<bool> DisplayAlert(string title, string message, string accept, string cancel = null);
}

public class NavigationService : INavigationService
{
    public INavigation Navi { get; internal set; }
    public Page myPage { get; set; }

    public Task<Page> PopAsync()
    {
        return Navi.PopAsync();
    }

    public Task<Page> PopModalAsync()
    {
        return Navi.PopModalAsync();
    }

    public Task PopToRootAsync()
    {
        return Navi.PopToRootAsync();
    }

    public Task PushAsync(Page page)
    {
        return Navi.PushAsync(page);
    }

    public async Task PushModalAsync(Page page)
    {
        await Navi.PushModalAsync(page);
    }

    public Task<bool> DisplayAlert(string title, string message, string accept, string cancel = null)
    {
        return myPage.DisplayAlert(title, message, accept, cancel);
    }

    public void InsertPageBefore(Page page, Page before)
    {
        throw new System.NotImplementedException();
    }

    public System.Collections.Generic.IReadOnlyList<Page> ModalStack
    {
        get { throw new System.NotImplementedException(); }
    }

    public System.Collections.Generic.IReadOnlyList<Page> NavigationStack
    {
        get { throw new System.NotImplementedException(); }
    }

    public Task<Page> PopAsync(bool animated)
    {
        throw new System.NotImplementedException();
    }

    public Task<Page> PopModalAsync(bool animated)
    {
        throw new System.NotImplementedException();
    }

    public Task PopToRootAsync(bool animated)
    {
        throw new System.NotImplementedException();
    }

    public Task PushAsync(Page page, bool animated)
    {
        throw new System.NotImplementedException();
    }

    public Task PushModalAsync(Page page, bool animated)
    {
        throw new System.NotImplementedException();
    }

    public void RemovePage(Page page)
    {
        throw new System.NotImplementedException();
    }
}
