// -----------------------------------------------------------------------
// <copyright file="SaveFile.cs" company="Biwug">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CamlDesigner.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SaveFile
    {
        public static bool SaveContentToCamlFile(string filename, string content)
        {
            try
            {
                // create a writer and open the file 
                TextWriter tw = new StreamWriter(filename);
                // write a line of text to the file 
                tw.WriteLine(content);
                // close the stream 
                tw.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
