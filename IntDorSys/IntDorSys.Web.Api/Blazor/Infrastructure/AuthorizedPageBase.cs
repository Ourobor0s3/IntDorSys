using IntDorSys.Web.Api.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace IntDorSys.Web.Api.Blazor.Infrastructure;

public abstract class AuthorizedPageBase : ComponentBase
{
    [Inject] protected AuthSession Auth { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    private bool _authChecked;

    protected override async Task OnInitializedAsync()
    {
        if (!_authChecked)
        {
            await Auth.InitializeAsync();
            _authChecked = true;
        }

        if (!Auth.IsLoggedIn)
        {
            Navigation.NavigateTo("/login", forceLoad: false);
        }
        else if (IsPublicPage())
        {
            Navigation.NavigateTo("/overview", forceLoad: false);
        }
    }

    private bool IsPublicPage()
    {
        var uri = Navigation.Uri;
        return uri.Contains("/login") || uri.Contains("/register") || 
               uri.EndsWith("/") || uri.EndsWith("/overview", StringComparison.OrdinalIgnoreCase);
    }

    protected override bool ShouldRender() => _authChecked;
}

public abstract class AdminPageBase : AuthorizedPageBase
{
    protected override async Task OnInitializedAsync()
    {
        await Auth.InitializeAsync();
        
        if (!Auth.IsLoggedIn)
        {
            Navigation.NavigateTo("/login", forceLoad: false);
            return;
        }
        
        if (!Auth.IsAdmin)
        {
            Navigation.NavigateTo("/overview", forceLoad: false);
            return;
        }
    }
}