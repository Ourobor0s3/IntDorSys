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
        await Auth.InitializeAsync();
        _authChecked = true;

        if (!Auth.IsLoggedIn)
        {
            Navigation.NavigateTo("/login", forceLoad: true);
        }
        else if (Navigation.Uri.Contains("/login") || Navigation.Uri == Navigation.BaseUri)
        {
            Navigation.NavigateTo("/overview", forceLoad: true);
        }
    }

    protected override bool ShouldRender() => _authChecked;
}