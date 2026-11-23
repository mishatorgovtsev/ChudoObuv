using ChudoObuv.Models;

namespace ChudoObuv;

public static class RoleNames
{
    public const string Administrator = "Администратор";
    public const string Manager = "Менеджер";
    public const string Client = "Авторизованный пользователь";
}

public static class AppSession
{
    public static User? CurrentUser { get; private set; }

    public static bool IsAuthorized => CurrentUser != null;

    public static string? RoleName => CurrentUser?.Role?.RoleName;

    public static bool IsAdministrator => RoleName == RoleNames.Administrator;

    public static bool CanManageOrders => RoleName == RoleNames.Administrator || RoleName == RoleNames.Manager;

    public static string FullName => CurrentUser == null
        ? "Гость"
        : $"{CurrentUser.Surname} {CurrentUser.Name} {CurrentUser.Patronymic}";

    public static void SignIn(User user)
    {
        CurrentUser = user;
        OrderCart.Clear();
    }

    public static void SignOut()
    {
        CurrentUser = null;
        OrderCart.Clear();
    }
}