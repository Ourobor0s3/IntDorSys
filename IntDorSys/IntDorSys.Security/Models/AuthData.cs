namespace IntDorSys.Security.Models
{
    /// <summary>
    ///     Contains user auth data
    /// </summary>
    public class AuthData
    {
        /// <summary>
        ///     User email
        /// </summary>
        required public string Login { get; set; }

        /// <summary>
        ///     Not hashed password
        /// </summary>
        required public string Password { get; set; }
    }
}