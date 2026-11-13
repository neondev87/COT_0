// Switch HOST | ADMIN del login: qué modo está elegido y el formulario del modo ADMIN
using COT_0.Host.Services.Usuarios;
using COT_0.Host.Services.Ventana;
using CommunityToolkit.Mvvm.ComponentModel;

namespace COT_0.Host.Views.ModelosDeVista;

/// <summary>Por defecto HOST. Al elegir ADMIN revisa si ya hay admins; al entrar un admin, lo deja en ActiveUser y abre el dashboard.</summary>
public partial class LoginModeViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsHostSelected), nameof(IsAdminSelected), nameof(IsHostMode))]
    private bool _isAdminMode;

    public LoginModeViewModel(AdminLoginViewModel admin)
    {
        Admin = admin;
        admin.SignedIn += OnAdminSignedIn;
    }

    public AdminLoginViewModel Admin { get; }

    public bool IsHostMode => !IsAdminMode;

    // El switch son dos RadioButton enlazados en dos sentidos
    public bool IsHostSelected
    {
        get => !IsAdminMode;
        set { if (value) IsAdminMode = false; }
    }

    public bool IsAdminSelected
    {
        get => IsAdminMode;
        set { if (value) IsAdminMode = true; }
    }

    partial void OnIsAdminModeChanged(bool value)
    {
        if (value) _ = Admin.StartAsync();
        else Admin.LeaveQuestionStep();
    }

    private static void OnAdminSignedIn(string adminName)
    {
        ActiveUser.SignInAdmin(adminName);
        Navigator.ShowDashboard();
    }
}
