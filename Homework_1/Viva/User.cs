public class User
{
    public int Id { get; }
    public string FullName { get; private set; }
    public string Login { get; private set; }
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

    public void ChangeFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new System.ArgumentException("ФИО обязательно.", nameof(fullName));
        }

        FullName = fullName.Trim();
    }

    public void ChangeLogin(string login)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            throw new System.ArgumentException("Логин обязателен.", nameof(login));
        }

        Login = login.Trim();
    }

    public override bool Equals(object? obj)
    {
        if (object.ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is User other && Id > 0 && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id > 0 ? Id.GetHashCode() : base.GetHashCode();
    }

    public void Deconstruct(out int id, out string fullName, out string login)
    {
        id = Id;
        fullName = FullName;
        login = Login;
    }
}
