using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skytap.Cloud.Vm.ChangeState
{
    public class CommonRestApiCall
    {
        public static Dictionary<string, JToken> GetToken(string url, string userName, string password, string methodType)
        {
            Dictionary<string, JToken> responseCollection = new Dictionary<string, JToken>();
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri) as HttpWebRequest;
            string auth = "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(userName + ":" + password));
            request.Headers["Authorization"] = auth;
            request.Method = methodType;
            request.Accept = "application/json";
            request.ContentType = "application/json";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
           | SecurityProtocolType.Tls11
           | SecurityProtocolType.Tls12
           | SecurityProtocolType.Ssl3;

            int retry = 0;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            try
            {
                while ((response.StatusCode.ToString() == "Ok") || retry < 4)
                {


                    try
                    {
                        WebResponse webResponse = request.GetResponse();
                        Stream webStream = webResponse.GetResponseStream();
                        if (webStream != null)
                        {
                            StreamReader responseReader = new StreamReader(webStream);
                            var response1 = responseReader.ReadToEnd();
                            JObject data = JObject.Parse(response1);
                            foreach (JProperty property in data.Properties())
                            {
                                responseCollection.Add(property.Name, property.Value);
                            }
                            //Console.Out.WriteLine(response1);
                            responseReader.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        retry++;
                        continue;
                        // eatup exception and try again 
                    }
                    break;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return responseCollection;
        }

    }
    }
