using ChudoObuv.Models;

namespace ChudoObuv;

public static class AppSession
{
    public static User? CurrentUser { get; private set; }

    public static bool IsAuthorized => CurrentUser != null;

    public static string FullName => CurrentUser == null
        ? "Гость"
        : $"{CurrentUser.Surname} {CurrentUser.Name} {CurrentUser.Patronymic}";

    public static void SignIn(User user) => CurrentUser = user;

    public static void SignOut() => CurrentUser = null;
}