namespace JokeCompany.ExternalFeeds.RandomName
{
    /// <summary>
    /// Provides random names.
    /// </summary>
    public interface IGetRandomName
    {
        /// <summary>
        /// Generates a random full name.
        /// </summary>
        /// <returns>Random full name.</returns>
        string GetRandomFullName();
    }
}