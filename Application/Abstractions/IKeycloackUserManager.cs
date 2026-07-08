namespace Application.Abstractions
{
    public interface IKeycloackUserManager
    {
        /// <summary>
        /// Добавляет HR в Keycloack
        /// </summary>
        /// <param name="username">Имя пользователя(должно быть уникальным)</param>
        /// <param name="password">Пароль пользователя</param>
        Task CreateHR(string username, string password);

        /// <summary>
        /// Добавляет Decider в Keycloack
        /// </summary>
        /// <param name="username">Имя пользователя(должно быть уникальным)</param>
        /// <param name="password">Пароль пользователя</param>
        Task CreateDecider(string username, string password);

        /// <summary>
        /// Удаляет HR из Keycloack
        /// </summary>
        /// <param name="username">Имя пользователя(должно быть уникальным)</param>
        /// <param name="password">Пароль пользователя</param>
        Task RevokeHR(string username);

        /// <summary>
        /// Добавляет HR в Keycloack
        /// </summary>
        /// <param name="username">Имя пользователя(должно быть уникальным)</param>
        /// <param name="password">Пароль пользователя</param>
        Task RevokeDecider(string username);
    }
}
