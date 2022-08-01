// -----------------------------------------------------------------------
// <copyright file="MyWebClient.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CamlDesigner.Helpers
{
    using System;
    using System.Net;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MyWebClient : WebClient
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (this.HeadOnly && req.Method == "GET")
            {
                req.Method = "HEAD";
            }
            return req;

        }

    }
}
