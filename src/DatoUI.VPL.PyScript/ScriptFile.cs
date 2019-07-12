using System.Collections.ObjectModel;

namespace DatoUI.VPL.PyScript
{
    public class ScriptFile
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ScriptFile" /> class.
        /// </summary>
        public ScriptFile()
        {
            ScriptContent = string.Empty;
        }

        /// <summary>
        ///     Gets or sets the content of the script.
        /// </summary>
        public string ScriptContent { get; set; }
    }
}