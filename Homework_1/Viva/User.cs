public class User
{
    public int Id { get; }
    public string FullName { get; }
    public string Login { get; }

    // Принимает готовый хэш; вычисление хэша выполняется вне модели.
    public string PasswordHash { get; }

    public User(int id, string fullName, string login, string passwordHash)
    {
        if (id < 0)
        {
            throw new System.ArgumentOutOfRangeException(nameof(id));
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new System.ArgumentException("ФИО обязательно.", nameof(fullName));
        }

        if (string.IsNullOrWhiteSpace(login))
        {
            throw new System.ArgumentException("Логин обязателен.", nameof(login));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new System.ArgumentException("Хэш пароля обязателен.", nameof(passwordHash));
        }

        Id = id;
        FullName = fullName.Trim();
        Login = login.Trim();
        PasswordHash = passwordHash;
    }

    // Хэш пароля не включается в краткие данные пользователя.
    public void Deconstruct(out int id, out string fullName, out string login)
    {
        id = Id;
        fullName = FullName;
        login = Login;
    }
}
