using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FREENOM
{

    sealed public class Connect
    {
        private Uri _url;

        private CookieContainer _container;

        private HttpWebRequest _req;
         
        // PROP
        public Uri URL
        {
            get { return _url; }
            set
            {
                string temp = WebUtility.HtmlDecode(value.ToString());
                value = new Uri(temp);
                _url = value;
            }
        }

        public CookieContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }

        public SecurityProtocolType ProtocolType { get; set; }
        public bool IsExpect100Continue { get; set; }
        public bool IsKeepAlive { get; set; }
        public bool IsRedirect { get; set; }


        public HttpWebResponse Response { get; set; }
        public string Host { get; set; }

        public string Connection { get; set; }
        public string UserAgent { get; set; }
        public string Accept { get; set; }
        public string Refferer { get; set; }
        public string ContentType { get; set; }
        public string SetCookie { get; set; }

        public DecompressionMethods AutoDecompress { get; set; }

        public NameValueCollection NameValColl { get; set; }

        public bool IsProxy { get; set; }
        public bool IsByPass { get; set; }

        public string IpPortOfProxy { get; set; }
        public string ProxyLogin { get; set; }
        public string ProxyPass { get; set; }

        public string Token { get; set; }

        public string ErrorMessage { get; set; }


        // FIELDS OF RESPONSE
        public string Location;
        public string AjaxLocation;

        public bool IsDebbug { get; set; }
        public bool IsExtendDebbug { get; set; }

        public string CurrentError { get; set; }

        public bool Error;





        // CONSTRUCTOR
        public Connect(string url)
        {
            Uri ur;

            if (Uri.TryCreate(url, UriKind.Absolute, out ur))
            {
                _url = ur;

                _container = new CookieContainer();

                SetCookie = "Включите опцию IsDebbug.";
            }
        }

        public Connect(string url, CookieContainer cont)
        {
            Uri ur;

            if (Uri.TryCreate(url, UriKind.Absolute, out ur))
            {
                _url = ur;

                _container = cont;

                SetCookie = "Включите опцию IsDebbug.";
            }
        }

        public Connect()
        {
            _container = new CookieContainer();

            SetCookie = "Включите опцию IsDebbug.";
        }


        /// <summary>
        /// Непотокобезопасный метод
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cookie"></param>
        /// <param name="isResponse"></param>
        /// <returns></returns>
        public string RequestGet(ListBox list, string cookie = null, bool isResponse = false)
        {
            try
            {
                if (URL == null) return null;
                if (list == null) return null;
                Error = false;

                System.Net.ServicePointManager.SecurityProtocol = ProtocolType;
                System.Net.ServicePointManager.Expect100Continue = IsExpect100Continue;

                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                if (string.IsNullOrEmpty(URL.ToString())) return null;

                _req = WebRequest.CreateHttp(URL);

                _req.KeepAlive = IsKeepAlive;
                _req.AllowAutoRedirect = IsRedirect;
                _req.Method = WebRequestMethods.Http.Get;
                _req.CookieContainer = _container;
                _req.AutomaticDecompression = AutoDecompress;

                if (IsProxy)
                {
                    if (IsByPass)
                    {
                        WebProxy wproxy = new WebProxy(IpPortOfProxy);
                        wproxy.UseDefaultCredentials = true;
                        wproxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPass);

                        _req.Proxy = wproxy;
                    }
                    else
                    {
                        _req.Proxy = new WebProxy(IpPortOfProxy);
                    }
                }
                else
                {
                    _req.Proxy = null;
                }

                if (string.IsNullOrEmpty(Host))
                {
                    _req.Host = URL.Host;
                }
                else
                {
                    _req.Host = Host;
                }

                if (!string.IsNullOrEmpty(Connection)) _req.Connection = Connection;
                if (!string.IsNullOrEmpty(UserAgent)) _req.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Accept))
                {
                    _req.Accept = Accept;
                }
                else
                {
                    _req.Accept = "text/html, application/xhtml+xml, */*";
                }
                if (!string.IsNullOrEmpty(Refferer)) _req.Referer = Refferer;

                if (NameValColl != null) _req.Headers.Add(NameValColl);// добавить свой заголовок

                if (cookie != null)
                {
                    _req.Headers["Cookie"] = cookie;
                }


                #region Extend Debbug

                if (IsExtendDebbug)
                {

                    list.Items.Add("");
                    list.Items.Add("GET URL: " + URL + " " + _req.ProtocolVersion);
                    list.Items.Add("    Host::" + URL.Host);
                    foreach (string k in _req.Headers.AllKeys)
                    {
                        list.Items.Add("    " + k + "::" + _req.Headers[k]);
                    }

                    list.Items.Add("");
                }

                #endregion


                HttpWebResponse resp = (HttpWebResponse)_req.GetResponse();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Items.Add("Code :" + resp.StatusCode);

                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Items.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion

                    if (isResponse)
                    {
                        Response = resp;

                        // COOKIE
                        CookieCollection cookieFromImage = resp.Cookies;
                        _container.Add(cookieFromImage);

                        return null;
                    }

                    Stream stream = resp.GetResponseStream();

                    string ans = null;
                    using (StreamReader read = new StreamReader(stream))
                    {
                        ans = read.ReadToEnd();
                    }
                    if (stream != null) stream.Close();

                    return ans;
                }
                else
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];
                    AjaxLocation = resp.Headers["Ajax-Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Items.Add("Code :" + resp.StatusCode);
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Items.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion

                }

                // COOKIE
                CookieCollection coll = resp.Cookies;
                _container.Add(coll);
                resp.Close();

                return null;

            }
            catch (WebException wex)
            {
                HttpWebResponse resp = wex.Response as HttpWebResponse;

                if (resp != null)
                {
                    HttpStatusCode code = resp.StatusCode;

                    if ((int)code == 422)
                    {
                        list.Items.Add("ERROR Code :" + code.ToString());
                        return null;
                    }
                }

                list.Items.Add("    CONNECT: " + wex.GetBaseException().ToString());

                Error = true;

                throw wex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
            return null;
        }

        /// <summary>
        /// Потокобезопасный метод
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cookie"></param>
        /// <param name="isResponse"></param>
        /// <returns></returns>
        public string RequestGet(List<string> list, string cookie = null, bool isResponse = false)
        {
            try
            {
                if (URL == null) return null;
                if (list == null) return null;
                Error = false;

                System.Net.ServicePointManager.SecurityProtocol = ProtocolType;
                System.Net.ServicePointManager.Expect100Continue = IsExpect100Continue;

                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                if (string.IsNullOrEmpty(URL.ToString())) return null;

                _req = WebRequest.CreateHttp(URL);

                _req.KeepAlive = IsKeepAlive;
                _req.AllowAutoRedirect = IsRedirect;
                _req.Method = WebRequestMethods.Http.Get;
                _req.CookieContainer = _container;
                _req.AutomaticDecompression = AutoDecompress;

                if (IsProxy)
                {
                    if (IsByPass)
                    {
                        WebProxy wproxy = new WebProxy(IpPortOfProxy);
                        wproxy.UseDefaultCredentials = true;
                        wproxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPass);

                        _req.Proxy = wproxy;
                    }
                    else
                    {
                        _req.Proxy = new WebProxy(IpPortOfProxy);
                    }
                }
                else
                {
                    _req.Proxy = null;
                }

                if (string.IsNullOrEmpty(Host))
                {
                    _req.Host = URL.Host;
                }
                else
                {
                    _req.Host = Host;
                }

                if (!string.IsNullOrEmpty(Connection)) _req.Connection = Connection;
                if (!string.IsNullOrEmpty(UserAgent)) _req.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Accept))
                {
                    _req.Accept = Accept;
                }
                else
                {
                    _req.Accept = "text/html, application/xhtml+xml, */*";
                }
                if (!string.IsNullOrEmpty(Refferer)) _req.Referer = Refferer;

                if (NameValColl != null) _req.Headers.Add(NameValColl);// добавить свой заголовок

                if (cookie != null)
                {
                    _req.Headers["Cookie"] = cookie;
                }


                #region Extend Debbug

                if (IsExtendDebbug)
                {

                    list.Add("");
                    list.Add("GET URL: " + URL + " " + _req.ProtocolVersion);
                    list.Add("    Host::" + URL.Host);
                    foreach (string k in _req.Headers.AllKeys)
                    {
                        list.Add("    " + k + "::" + _req.Headers[k]);
                    }

                    list.Add("");
                }

                #endregion


                HttpWebResponse resp = (HttpWebResponse)_req.GetResponse();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Add("Code :" + resp.StatusCode);

                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion

                    if (isResponse)
                    {
                        Response = resp;

                        // COOKIE
                        CookieCollection cookieFromImage = resp.Cookies;
                        _container.Add(cookieFromImage);

                        return null;
                    }

                    Stream stream = resp.GetResponseStream();

                    string ans = null;
                    using (StreamReader read = new StreamReader(stream))
                    {
                        ans = read.ReadToEnd();
                    }
                    if (stream != null) stream.Close();

                    return ans;
                }
                else
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];
                    AjaxLocation = resp.Headers["Ajax-Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Add("Code :" + resp.StatusCode);
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion

                }

                // COOKIE
                CookieCollection coll = resp.Cookies;
                _container.Add(coll);
                resp.Close();

                return null;

            }
            catch (WebException wex)
            {
                HttpWebResponse resp = wex.Response as HttpWebResponse;

                if (resp != null)
                {
                    HttpStatusCode code = resp.StatusCode;

                    if ((int)code == 422)
                    {
                        list.Add("ERROR Code :" + code.ToString());
                        return null;
                    }
                }

                list.Add("    CONNECT: " + wex.Message);

                Error = true;

                throw wex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
            return null;
        }


        public string RequestPost(string post, ListBox list, string cookie = null)
        {
            try
            {
                if (URL == null) return null;
                if (list == null) return null;
                Error = false;

                System.Net.ServicePointManager.SecurityProtocol = ProtocolType;
                System.Net.ServicePointManager.Expect100Continue = IsExpect100Continue;

                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                if (string.IsNullOrEmpty(URL.ToString())) return null;

                _req = WebRequest.CreateHttp(URL);

                _req.KeepAlive = IsKeepAlive;
                _req.AllowAutoRedirect = IsRedirect;
                _req.Method = WebRequestMethods.Http.Post;
                if (string.IsNullOrEmpty(cookie)) _req.CookieContainer = _container;
                _req.AutomaticDecompression = AutoDecompress;

                if (IsProxy)
                {
                    if (IsByPass)
                    {
                        WebProxy wproxy = new WebProxy(IpPortOfProxy);
                        wproxy.UseDefaultCredentials = true;
                        wproxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPass);

                        _req.Proxy = wproxy;
                    }
                    else
                    {
                        _req.Proxy = new WebProxy(IpPortOfProxy);
                    }
                }
                else
                {
                    _req.Proxy = null;
                }

                _req.Host = URL.Host;
                if (!string.IsNullOrEmpty(UserAgent)) _req.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Accept))
                {
                    _req.Accept = Accept;
                }
                else
                {
                    _req.Accept = "text/html, application/xhtml+xml, */*";
                }
                if (!string.IsNullOrEmpty(Refferer)) _req.Referer = Refferer;
                if (!string.IsNullOrEmpty(ContentType))
                {
                    _req.ContentType = ContentType;
                }
                else
                {
                    _req.ContentType = "application/x-www-form-urlencoded";
                }

                if (NameValColl != null) _req.Headers.Add(NameValColl); // добавить свой заголовок

                if (cookie != null)
                {
                    _req.Headers["Cookie"] = cookie;
                }


                #region Extend Debbug

                if (IsExtendDebbug)
                {

                    list.Items.Add("");
                    list.Items.Add("POST URL: " + URL + " " + _req.ProtocolVersion);
                    list.Items.Add("    Host::" + URL.Host);
                    foreach (string k in _req.Headers.AllKeys)
                    {
                        list.Items.Add("    " + k + "::" + _req.Headers[k]);
                    }
                    list.Items.Add("POST: " + post);
                    list.Items.Add("");
                }

                #endregion


                byte[] bts = Encoding.UTF8.GetBytes(post);

                using (Stream strm = _req.GetRequestStream())
                {
                    strm.Write(bts, 0, bts.Length);
                }

                HttpWebResponse resp = (HttpWebResponse)_req.GetResponse();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];
                    AjaxLocation = resp.Headers["Ajax-Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Items.Add("Code :" + resp.StatusCode);
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Items.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion


                    Stream stream = resp.GetResponseStream();
                    string ans = null;
                    using (StreamReader read = new StreamReader(stream))
                    {
                        ans = read.ReadToEnd();
                    }
                    if (stream != null) stream.Close();


                    return ans;
                }
                else
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Items.Add("Code :" + resp.StatusCode);
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Items.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion


                }

                // COOKIE
                CookieCollection coll = resp.Cookies;
                _container.Add(coll);
                resp.Close();

                return null;


            }
            catch (WebException wex)
            {

                if (wex.Response == null)
                {
                    list.Items.Add("ERROR: Connect() Response == null");

                    throw wex;
                }

                HttpStatusCode code = ((HttpWebResponse)wex.Response).StatusCode;

                string ans = ReadError(wex);

                list.Items.Add("CONNECT: " + code + " :: " + ans);
                Error = true;
                ErrorMessage = ans;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
            return null;
        }

       /// <summary>
       /// Потокобезопасный метод 
       /// </summary>
       /// <param name="post"></param>
       /// <param name="list"></param>
       /// <param name="cookie"></param>
       /// <returns></returns>
        public string RequestPost(string post, List<string> list, string cookie = null)
        {
            try
            {
                if (URL == null) return null;
                if (list == null) return null;
                Error = false;

                System.Net.ServicePointManager.SecurityProtocol = ProtocolType;
                System.Net.ServicePointManager.Expect100Continue = IsExpect100Continue;

                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                if (string.IsNullOrEmpty(URL.ToString())) return null;

                _req = WebRequest.CreateHttp(URL);

                _req.KeepAlive = IsKeepAlive;
                _req.AllowAutoRedirect = IsRedirect;
                _req.Method = WebRequestMethods.Http.Post;
                if (string.IsNullOrEmpty(cookie)) _req.CookieContainer = _container;
                _req.AutomaticDecompression = AutoDecompress;

                if (IsProxy)
                {
                    if (IsByPass)
                    {
                        WebProxy wproxy = new WebProxy(IpPortOfProxy);
                        wproxy.UseDefaultCredentials = true;
                        wproxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPass);

                        _req.Proxy = wproxy;
                    }
                    else
                    {
                        _req.Proxy = new WebProxy(IpPortOfProxy);
                    }
                }
                else
                {
                    _req.Proxy = null;
                }

                _req.Host = URL.Host;
                if (!string.IsNullOrEmpty(UserAgent)) _req.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Accept))
                {
                    _req.Accept = Accept;
                }
                else
                {
                    _req.Accept = "text/html, application/xhtml+xml, */*";
                }
                if (!string.IsNullOrEmpty(Refferer)) _req.Referer = Refferer;
                if (!string.IsNullOrEmpty(ContentType))
                {
                    _req.ContentType = ContentType;
                }
                else
                {
                    _req.ContentType = "application/x-www-form-urlencoded";
                }

                if (NameValColl != null) _req.Headers.Add(NameValColl); // добавить свой заголовок

                if (cookie != null)
                {
                    _req.Headers["Cookie"] = cookie;
                }


                #region Extend Debbug

                if (IsExtendDebbug)
                {

                    list.Add("");
                    list.Add("POST URL: " + URL + " " + _req.ProtocolVersion);
                    list.Add("    Host::" + URL.Host);
                    foreach (string k in _req.Headers.AllKeys)
                    {
                        list.Add("    " + k + "::" + _req.Headers[k]);
                    }
                    list.Add("POST: " + post);
                    list.Add("");
                }

                #endregion


                byte[] bts = Encoding.UTF8.GetBytes(post);

                using (Stream strm = _req.GetRequestStream())
                {
                    strm.Write(bts, 0, bts.Length);
                }

                HttpWebResponse resp = (HttpWebResponse)_req.GetResponse();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];
                    AjaxLocation = resp.Headers["Ajax-Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Add("Code :" + resp.StatusCode);
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion


                    Stream stream = resp.GetResponseStream();
                    string ans = null;
                    using (StreamReader read = new StreamReader(stream))
                    {
                        ans = read.ReadToEnd();
                    }
                    if (stream != null) stream.Close();


                    return ans;
                }
                else
                {
                    SetCookie = resp.Headers["Set-Cookie"];
                    Location = resp.Headers["Location"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Add("Code :" + resp.StatusCode);
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion


                }

                // COOKIE
                CookieCollection coll = resp.Cookies;
                _container.Add(coll);
                resp.Close();

                return null;


            }
            catch (WebException wex)
            {

                if (wex.Response == null)
                {
                    list.Add("ERROR: Connect() Response == null");

                    throw wex;
                }

                HttpStatusCode code = ((HttpWebResponse)wex.Response).StatusCode;

                string ans = ReadError(wex);

                list.Add("CONNECT: " + code + " :: " + ans);
                Error = true;
                ErrorMessage = ans;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
            return null;
        }


        public string RequestByMethod(string method, ListBox list, string body = null)
        {
            try
            {
                if (URL == null) return null;
                if (list == null) return null;
                Error = false;

                System.Net.ServicePointManager.SecurityProtocol = ProtocolType;
                System.Net.ServicePointManager.Expect100Continue = IsExpect100Continue;

                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;

                if (string.IsNullOrEmpty(URL.ToString())) return null;

                _req = WebRequest.CreateHttp(URL);

                _req.KeepAlive = IsKeepAlive;
                _req.AllowAutoRedirect = IsRedirect;
                _req.Method = method;
                _req.CookieContainer = _container;
                _req.AutomaticDecompression = AutoDecompress;

                if (IsProxy)
                {
                    if (IsByPass)
                    {
                        WebProxy wproxy = new WebProxy(IpPortOfProxy);
                        wproxy.UseDefaultCredentials = true;
                        wproxy.Credentials = new NetworkCredential(ProxyLogin, ProxyPass);

                        _req.Proxy = wproxy;
                    }
                    else
                    {
                        _req.Proxy = new WebProxy(IpPortOfProxy);
                    }
                }
                else
                {
                    _req.Proxy = null;
                }

                _req.Host = URL.Host;
                if (!string.IsNullOrEmpty(UserAgent)) _req.UserAgent = UserAgent;
                if (!string.IsNullOrEmpty(Accept)) _req.Accept = Accept;
                if (!string.IsNullOrEmpty(Refferer)) _req.Referer = Refferer;
                if (!string.IsNullOrEmpty(ContentType)) _req.ContentType = ContentType;


                if (NameValColl != null) _req.Headers.Add(NameValColl);// добавить свой заголовок

                _req.Headers["Upgrade-Insecure-Requests"] = "1";
                _req.Headers["DNT"] = "1";

                #region Extend Debbug

                if (IsExtendDebbug)
                {

                    list.Items.Add("");
                    list.Items.Add(method + " URL: " + URL + " " + _req.ProtocolVersion);
                    list.Items.Add("Method: " + method);
                    list.Items.Add("    Host::" + URL.Host);
                    foreach (string k in _req.Headers.AllKeys)
                    {
                        list.Items.Add("    " + k + "::" + _req.Headers[k]);
                    }

                    list.Items.Add("");
                }

                #endregion

                if (!string.IsNullOrEmpty(body))
                {

                    list.Items.Add("BODY: " + body);

                    byte[] bts = Encoding.UTF8.GetBytes(body);

                    using (Stream strm = _req.GetRequestStream())
                    {
                        strm.Write(bts, 0, bts.Length);
                    }

                }


                HttpWebResponse resp = (HttpWebResponse)_req.GetResponse();

                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    SetCookie = resp.Headers["Set-Cookie"];

                    #region Debbug

                    if (IsDebbug)
                    {
                        list.Items.Add("Code :" + resp.StatusCode);

                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Items.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }

                    #endregion

                    Stream stream = resp.GetResponseStream();
                    string ans = null;
                    using (StreamReader read = new StreamReader(stream))
                    {
                        ans = read.ReadToEnd();
                    }
                    stream.Close();

                    return ans;
                }
                else
                {
                    list.Items.Add("Code :" + resp.StatusCode);

                    if (IsDebbug)
                    {
                        foreach (string k in resp.Headers.AllKeys)
                        {
                            list.Items.Add("    " + k + "::" + resp.Headers[k]);
                        }
                    }
                }

                // COOKIE
                CookieCollection coll = resp.Cookies;
                _container.Add(coll);
                resp.Close();

                return null;

            }
            catch (WebException wex)
            {
                if (wex.Response == null)
                {
                    list.Items.Add("ERROR: Connect() Response == null");

                    throw wex;
                }

                HttpStatusCode code = ((HttpWebResponse)wex.Response).StatusCode;

                if ((int)code == 400)
                {
                    CurrentError = "400 ERROR Code :" + code.ToString() + "::" + wex.Message;

                    return "400";
                }


                if ((int)code == 422)
                {

                    list.Items.Add("ERROR Code :" + code.ToString());
                    return null;
                }

                list.Items.Add("class CONNECT:" + wex.GetBaseException().ToString());

                throw wex;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
            return null;
        }


        public void ClearAllHeaders()
        {
            try
            {
                if (_req == null) return;
                this.Accept = null;
                this.Refferer = null;
                this.ContentType = null;
                this.NameValColl = null;

            }
            catch (Exception eg)
            {
                MessageBox.Show(eg.GetBaseException().ToString());
            }
        }

        private string ReadError(WebException wex)
        {
            try
            {
                HttpWebResponse res = (HttpWebResponse)wex.Response;

                Stream strm = res.GetResponseStream();

                string ans = null;
                using (StreamReader read = new StreamReader(strm))
                {
                    ans = read.ReadToEnd();
                }

                return ans;
            }
            catch (Exception ez)
            {
                throw;
            }
        }

    }



}
