using IntDorSys.Web.Api.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace IntDorSys.Web.Api.Blazor.Infrastructure;

public abstract class AuthorizedPageBase : ComponentBase
{
    [Inject] protected AuthSession Auth { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    protected bool IsAuthorized { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        await Auth.InitializeAsync();
        IsAuthorized = Auth.IsLoggedIn;

        if (!IsAuthorized)
        {
            Navigation.NavigateTo("/login", forceLoad: false);
        }
    }
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