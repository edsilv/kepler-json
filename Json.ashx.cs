using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Kepler
{
    /// <summary>
    /// Summary description for kepler
    /// </summary>
    public class Json : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var request = WebRequest.Create("http://exoplanetarchive.ipac.caltech.edu/cgi-bin/nstedAPI/nph-nstedAPI?table=exoplanets");
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var csvString = reader.ReadToEnd();

            if (!String.IsNullOrEmpty(context.Request.QueryString["csv"]))
            {
                context.Response.ContentType = "text/plain";
                
                context.Response.Write(csvString);
            }
            else
            {
                context.Response.ContentType = "application/json";

                var lines = csvString.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                var cols = lines[0].Split(',');
                var csv = lines.Skip(1)
                               .Select(l => l.Split(',')
                                             .Select((s, i) => new { s, i })
                                             .ToDictionary(x => cols[x.i], x => x.s));

                //var json = DateTime.Now.ToLongTimeString();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var json = serializer.Serialize(csv);

                context.Response.Write(json);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}