using System.Text.Json.Serialization;

namespace InAppUpdates.iOSAndroid.Maui.Common
{
    /// <summary>
    /// Model class for storing app update check cache information
    /// </summary>
    internal class AppStoreUpdateCheckCache
    {
        /// <summary>
        /// Gets or sets the date of the last update check
        /// </summary>
        [JsonPropertyName("lastCheckDate")]
        public DateTime LastCheckDate { get; set; }
        
        /// <summary>
        /// Gets or sets the last checked version
        /// </summary>
        [JsonPropertyName("lastCheckedVersion")]
        public string? LastCheckedVersion { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the current session started
        /// </summary>
        [JsonPropertyName("currentSessionDate")]
        public DateTime CurrentSessionDate { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the check has been 
        /// performed for the current session
        /// </summary>
        [JsonPropertyName("checkedInCurrentSession")]
        public bool CheckedInCurrentSession { get; set; }
    }
}