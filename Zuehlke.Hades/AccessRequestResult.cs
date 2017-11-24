namespace Zuehlke.Hades
{
    /// <summary>
    /// The result of an access request
    /// </summary>
    public enum AccessRequestResult
    {
        /// <summary>
        /// The request was granted
        /// </summary>
        Granted,
        /// <summary>
        /// The request was explicitly denied
        /// </summary>
        ExplicitlyDenied,
        /// <summary>
        /// The request was denied by default
        /// </summary>
        Denied
    }
}
