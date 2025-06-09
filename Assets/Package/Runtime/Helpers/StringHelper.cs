using System;

namespace VARLab.Velcro
{
    /// <summary>
    /// A collections of useful string functions.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Formats time in seconds into string form. 
        /// The available formats can be found on <see href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings#the-constant-c-format-specifier">Microsoft's documentation</see>.<br/>
        /// Example formats:<br/>
        /// (H: Hours, m: Minutes, s: Seconds)<br/>
        /// HH:mm<br/>
        /// HH:mm:ss
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatTime(double seconds, string format = "HH:mm:ss")
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return DateTime.Today.Add(time).ToString(format);
        }
    }
}
