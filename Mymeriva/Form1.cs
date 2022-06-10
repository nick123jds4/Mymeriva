using AForge.Imaging.Filters;
using Bogus;
using CaptchaSolverLib;
using EAGetMail;
using FREENOM;
using HtmlAgilityPack;
using Mymeriva.Agility;
using Mymeriva.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Mymeriva
{
    public partial class Form1 : Form
    {

        private CancellationTokenSource _cancel;
        private string _currentMail;
        private string _mailPass;
        private delegate bool _delegats();
        private const string _dataFile = "DATA";
        private const string _mailFile = _dataFile + "/mails.txt";
        private const string _proxyFile = "PROXY\\proxy.txt";
        private const string _pathToSave = "ACCOUNTS/accounts.txt";
        private const string _pathToSaveSomee = "ACCOUNTS/someeAccounts.txt";
        private const string _domainsPath = @"DOMAINS\domains.txt";
        private const string _domainsPathSomee = @"DOMAINS\domainsSomee.txt";
        private const string _pathToCheckSomee = @"DOMAINS\CHECK\checkSomee.txt";
        private const string _nod = "Random Domain Name/nameOfDomane.txt";
        private const string _urlSomee = "https://somee.com/FreeAspNetHosting.aspx";
        private string _currentProxy;
        private string _proxyLogin;
        private string _proxyPass;
        private string _typeProxy;
        private string _anonymity;
        private string _ua;
        private string _cookies;
        private string _pass;
        private Color _color;
        private int _errorCount;
        private int _emailError;
        private int _mailCount;
        private string _linkTo;
        private WebBrowserDocumentCompletedEventHandler _handler;
        private string _token;
        private string _sitekey;
        private string _idCaptch;
        private bool _isCaptchaOk;
        private string _resultCaptch;
        private string[] _tab2ArrayOfProxy;
        private CancellationTokenSource _cancelEmail;
        private IPAddress _ip;
        private string __VIEWSTATE;
        private string __EVENTVALIDATION;
        private string __VIEWSTATEGENERATOR;
        private string _someeAspVersion;
        private string _someeOsVersion;
        private string _userName;
        XElement _variableBranch = new XElement("Variable");
        private bool _isFirstRunChrome = true;
        private ChromeDriver _driver;
        private BackgroundWorker _worker;

        public string Key { get; set; }
        public string Key_anti_captcha { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {

                richTextBox1.Multiline = true;
                richTextBox1.WordWrap = false;


                #region SETTING.XML

                if (File.Exists("setting.xml"))
                {
                    XDocument docs = XDocument.Load("setting.xml");

                    DeclareAllControls(this.Controls, docs);


                    foreach (XElement doc in docs.Descendants("Variable").Elements())
                    {

                        var info = this.GetType().GetField(doc.Name.ToString(), BindingFlags.Instance | BindingFlags.NonPublic);

                        if (info != null)
                        {
                            info.SetValue(this, doc.Value);

                            // listBox1.Items.Add(doc.Name + " = " + doc.Value);
                        }

                    }// for


                }




                // это событие происходит перед закрытием формы
                this.FormClosing += new FormClosingEventHandler((o, f) =>
                {
                    SaveAll();
                });

                #endregion


                string[] mass = File.ReadAllLines(_dataFile + "/UserAgetn.txt");
                _ua = mass[new Random().Next(mass.Length)].Trim();

                while (string.IsNullOrEmpty(_ua)) // если пустая строчка
                {
                    _ua = mass[new Random().Next(mass.Length)].Trim();
                }

                listBox1.Items.Add(_ua);

                Key_anti_captcha = "f993fc2ce77fd405f4f6a0ec248d72ce";
                Key = "e99e09e77bfab670d2b192c1a6db8c4a".Trim();
                _sitekey = "6LcxaxEaAAAAAEdk56_9qVDd47kuvxGGvUvmVGcU";

                textBox2.Text = "GetMyEmailOnet M1: " + _currentMail + " :: " + _mailPass + " :: " + _pass;


                // если в comboBox что то есть - обновить данные
                if (comboBoxIpServer.SelectedIndex != -1)
                {
                    string[] arr = comboBoxIpServer.SelectedItem.ToString()
                        .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    if (arr.Length != 2) return;

                    _ip = IPAddress.Parse(arr[0]);

                }

                if (comboBoxASPNetVersion.SelectedIndex == -1) { _someeAspVersion = "4.0.30319"; }
                else
                {
                    var res = comboBoxASPNetVersion.SelectedItem.ToString();

                    res = res.Split(';')[1];

                    _someeAspVersion = res;
                }
                if (comboBoxOperatingSystem.SelectedIndex == -1) { _someeOsVersion = "6000"; }
                else
                {
                    var res = comboBoxOperatingSystem.SelectedItem.ToString();
                    res = res.Split(';')[1];
                    _someeOsVersion = res;
                }

                comboBoxASPNetVersion.SelectedIndexChanged += new EventHandler((obj, erg) =>
                {
                    _someeAspVersion = comboBoxASPNetVersion.SelectedItem.ToString();
                });
                comboBoxOperatingSystem.SelectedIndexChanged += new EventHandler((obj, erg) =>
                {
                    _someeOsVersion = comboBoxOperatingSystem.SelectedItem.ToString();
                });

                listBox1.Items.Add("value " + _someeOsVersion + " " + _someeAspVersion);

                if (radioButtonSomee.Checked) labelSomee.Text += " somee.com";

            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }

        #region Save data

        private void SaveAll()
        {
            try
            {
                XDocument document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));

                XElement elemA = new XElement("A");

                elemA.Add(_variableBranch);

                if (!string.IsNullOrEmpty(_linkTo))
                {

                    SaveLocalVariable("_linkTo", _linkTo);
                }


                SaveLocalVariable("_currentMail", _currentMail);
                SaveLocalVariable("_mailPass", _mailPass);


                if (!string.IsNullOrEmpty(_pass))
                {
                    SaveLocalVariable("_pass", _pass);
                }


                SaveLocalVariable("_currentProxy", _currentProxy);

                if (checkBoxProxyPass.Checked)
                {
                    SaveLocalVariable("_proxyLogin", _proxyLogin);
                    SaveLocalVariable("_proxyPass", _proxyPass);

                }

                SaveLocalVariable("_userName", _userName);

                SaveAllControls(this.Controls, ref elemA);


                document.Add(elemA);

                // сохранить данные если они отсутствуют в вируальном дереве xml
                if (File.Exists("setting.xml"))
                {
                    XDocument docs = XDocument.Load("setting.xml");

                    string[] except = { "_linkTo", "_currentMail", "_mailPass", "_currentProxy", "_proxyLogin", "_proxyPass", "_userName" };

                    foreach (XElement doc in docs.Descendants("Variable").Elements())
                    {
                        // получить все переменные  
                        var info = this.GetType()
                            .GetField(doc.Name.ToString(), BindingFlags.Instance | BindingFlags.NonPublic);

                        if (info == null) continue;
                        if (except.Contains(doc.Name.ToString())) continue;

                        if (document.XPathSelectElement("//" + doc.Name) == null)
                        {
                            SaveLocalVariable(doc.Name.ToString(), doc.Value);
                        }

                    } // for
                }


                document.Save("setting.xml");


            }
            catch (Exception ez)
            {
                MessageBox.Show("SaveAll " + ez.GetBaseException().ToString());
            }

        }

        public string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }


        private void SaveLocalVariable(string name, string value)
        {
            XElement elem = new XElement(name, value);

            _variableBranch.Add(elem);
        }


        private void SaveAllControls(Control.ControlCollection controls, ref XElement elemA)
        {
            try
            {

                foreach (Control control in controls)
                {

                    #region filters

                    var button = control as Button;
                    if (button != null) continue;

                    var label = control as Label;
                    if (label != null) continue;

                    var menu = control as MenuStrip;
                    if (menu != null) continue;

                    #endregion

                    #region получить значения конкретных свойств

                    var type = control.GetType();

                    string[] props = { "Checked", "Value", "SelectedItem" };

                    bool isprop = false;
                    foreach (string name in props)
                    {
                        var prop = type.GetProperty(name);

                        if (prop != null)
                        {
                            //listBox1.Items.Add("\tContain : " + prop.Name);

                            XElement B = new XElement(control.Name, prop.GetValue(control));

                            elemA.Add(B);

                            isprop = true;
                            break;
                        }
                    }

                    if (isprop) continue;

                    #endregion

                    if (!string.IsNullOrEmpty(control.Name) && !string.IsNullOrEmpty(control.Text))
                    {
                        XElement B = new XElement(control.Name, control.Text);

                        elemA.Add(B);

                    }

                    if (control.Controls.Count != 0)// есть ли вложенные контролы
                    {
                        SaveAllControls(control.Controls, ref elemA);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }

        private void DeclareAllControls(Control.ControlCollection controls, XDocument doc)
        {
            try
            {
                foreach (Control control in controls)
                {
                    #region filters

                    var button = control as Button;
                    if (button != null) continue;

                    var label = control as Label;
                    if (label != null) continue;

                    var menu = control as MenuStrip;
                    if (menu != null) continue;

                    #endregion

                    // listBox1.Items.Add(control.GetType()+" :: "+control.Name);

                    foreach (XElement elem in doc.Descendants("A").Elements())
                    {

                        if (elem.Name.ToString().Equals(control.Name))
                        {

                            #region получить значения конкретных свойств

                            var type = control.GetType();

                            string[] props = { "Checked", "Value", "SelectedItem" };

                            bool isprop = false;
                            foreach (string name in props)
                            {
                                var prop = type.GetProperty(name);

                                if (prop != null)
                                {
                                    // listBox1.Items.Add("\t" +control.Name+") "+ prop.Name + " " + elem.Value + " " +
                                    //                  prop.PropertyType);

                                    if (prop.PropertyType == typeof(bool))
                                    {
                                        prop.SetValue(control, Convert.ToBoolean(elem.Value));
                                    }

                                    if (prop.PropertyType == typeof(decimal))
                                    {
                                        prop.SetValue(control, Convert.ToDecimal(elem.Value));
                                    }

                                    if (prop.PropertyType == typeof(object))
                                    {
                                        prop.SetValue(control, elem.Value.ToString());
                                    }

                                    isprop = true;
                                    break;
                                }
                            }

                            if (isprop) break;

                            #endregion

                            if (!string.IsNullOrEmpty(elem.Name.ToString()) && !string.IsNullOrEmpty(elem.Value))
                            {
                                control.Text = elem.Value;
                            }

                        }

                    }

                    if (control.Controls.Count != 0) // есть ли вложенные контролы
                    {
                        DeclareAllControls(control.Controls, doc);
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }

        #endregion

        private class MyList<T> : List<T>
        {
            public List<T> List { get; set; }

            public MyList(T arg)
            {
                this.Add(arg);
            }

            public MyList(List<T> arg)
            {
                this.AddRange(arg);
            }

            public static MyList<T> Insert(T log)
            {

                return new MyList<T>(log);
            }
            public static MyList<T> Insert(IEnumerable<T> log)
            {

                return new MyList<T>(log.ToList());
            }
        }



        private void buttonSTART_Click(object sender, EventArgs e)
        {
            try
            {



                #region backgroundworker 

                var pr = new ProgressChangedEventHandler((obj, arg) =>
                {
                    if (_worker.CancellationPending) return;

                    listBox1.Items.Add($"Поток: {Thread.CurrentThread.ManagedThreadId}");

                    List<string> log = arg.UserState as List<string>;

                    List<ICloneable> newList = new List<ICloneable>(log.Count);

                    log.ForEach((item) =>
                    {
                        newList.Add((ICloneable)item.Clone());
                    });

                    if (log != null & log.Count != 0)
                    {
                        for (int i = 0; i < log.Count; i++)
                        {
                            listBox1.Items.Add($"{log[i]}");
                        }

                    }
                });

                if (_worker != null) _worker.ProgressChanged -= pr;
                _worker = new BackgroundWorker();
                _worker.WorkerSupportsCancellation = true;
                _worker.WorkerReportsProgress = true;
                _worker.ProgressChanged += pr;



                #endregion


                listBox1.Items.Clear();
                ClearErrorsDirectory();

                if (_cancelEmail != null) _cancelEmail.Cancel();
                if (_cancel != null) _cancel.Cancel();


                listBox1.Items.Add("TimerN count: " + _timers.Count);
                StopTimersN();
                if (_timers.Count != 0) return;

                _cancel = new CancellationTokenSource();
                CancellationToken token = _cancel.Token;

                // подавлять куки
                WinInetHelper.SupressCookiePersist();


                if (checkBxPROXY.Checked)
                {
                    #region proxy

                    bool isConnect = false;
                    HttpWebRequest req = WebRequest.CreateHttp("https://www.google.com/");

                    if (string.IsNullOrEmpty(_currentProxy))
                    {
                        bool isPrx = SetProxy();//обновить прокси
                        if (!isPrx) return;
                    }
                    else
                    {
                        listBox1.Items.Add("[2]HTTP:" + _currentProxy + " :" + _proxyLogin + ":" + _proxyPass);
                    }

                    if (checkBoxCheckProxy.Checked)
                    {
                        #region Check Proxy

                        listBox1.Items.Add("    id:" + Thread.CurrentThread.ManagedThreadId);


                        ServicePointManager.DnsRefreshTimeout = 10000;

                        System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                                          SecurityProtocolType.Tls11 |
                                                                          SecurityProtocolType.Tls12;

                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        ServicePointManager.DnsRefreshTimeout = 10000;

                        req.KeepAlive = true;
                        req.AllowAutoRedirect = false;
                        req.Method = WebRequestMethods.Http.Get;
                        req.Timeout = 10000;

                        if (checkBoxProxyPass.Checked)
                        {
                            WebProxy proxy = new WebProxy(_currentProxy);
                            proxy.UseDefaultCredentials = true;
                            proxy.Credentials = new NetworkCredential(_proxyLogin, _proxyPass);

                            req.Proxy = proxy;
                        }
                        else
                        {
                            req.Proxy = new WebProxy(_currentProxy);
                        }


                        req.UserAgent =
                            "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:62.0) Gecko/20100101 Firefox/62.0";
                        req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                        req.Referer = "https://www.google.com/";

                        req.BeginGetResponse((async) =>
                        {
                            try
                            {
                                WebRequest re = (WebRequest)async.AsyncState;
                                HttpWebResponse resp = (HttpWebResponse)re.EndGetResponse(async);

                                if (resp.StatusCode == HttpStatusCode.OK)
                                {
                                    isConnect = true;
                                }

                                resp.Close();

                            }
                            catch (Exception err)
                            {
                            }

                        }, req);


                        #endregion
                    }
                    else
                    {
                        isConnect = true;
                    }

                    int z = 0;
                    TimerN(() =>
                    {
                        if (isConnect)
                        {
                            listBox1.Items.Add("Connect is success.");

                            _worker.DoWork += new DoWorkEventHandler((obj, arg) =>
                            {
                                if (string.IsNullOrEmpty(_linkTo))
                                {
                                    if (!Email()) return;

                                    if (radioButtonMymeriva.Checked) RUN(token);
                                    if (radioButtonSomee.Checked) SomeeRUN(token);
                                }
                                else
                                {
                                    if (radioButtonMymeriva.Checked) RUN2(token, true);
                                    if (radioButtonSomee.Checked) SomeeRUN2(token, true);
                                }


                            });
                            _worker.RunWorkerAsync();
                            return true;
                        }

                        z++;
                        label3.Text = "tm: " + z;
                        if (z >= 10)
                        {
                            req.Abort();
                            listBox1.Items.Add("Timer is stopped");
                            this.Invoke(new MethodInvoker(() => buttonDeleteProxy.PerformClick()));
                            this.Invoke(new MethodInvoker(() => buttonSTART.PerformClick()));
                            return true;
                        }

                        return false;
                    }, 500);

                    #endregion
                }
                else
                {
                    #region WITHOUT PROXY

                    _worker.DoWork += new DoWorkEventHandler((obj, arg) =>
                    {
                        if (string.IsNullOrEmpty(_linkTo))
                        {
                            if (!Email()) return;

                            if (radioButtonMymeriva.Checked) RUN(token, false);
                            if (radioButtonSomee.Checked) SomeeRUN(token, false);
                        }
                        else
                        {
                            if (radioButtonMymeriva.Checked) RUN2(token, false);
                            if (radioButtonSomee.Checked) SomeeRUN2(token, false);
                        }
                    });
                    _worker.RunWorkerAsync();
                    #endregion
                }


            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }


        #region Mymeriva

        private async void RUN(CancellationToken tokenCanc, bool isProxy = true)
        {
            try
            {

                listBox1.Items.Add("------              ------");
                listBox1.Items.Add("-------B E G I N-------");
                if (isProxy) listBox1.Items.Add("@@@@@@@ Proxy is " + isProxy);
                if (tokenCanc.IsCancellationRequested) return;
                this.Invoke(new MethodInvoker(() => buttonSTART.Visible = false));

                #region E-mail
                ///*
                _cancelEmail = new CancellationTokenSource();
                CancellationTokenSource cs = new CancellationTokenSource();

                object combo = comboBoxEmail.SelectedItem;
                if (combo != null)
                {

                    if (string.IsNullOrEmpty(_currentMail))
                    {
                        listBox1.Items.Add("E-mail is empty.");
                        return;
                    }


                    Task.Factory.StartNew(() => EmailPop3(combo.ToString(), _currentMail, _mailPass, cs));
                }
                else
                {
                    listBox1.Items.Add("Вы не указали сервер для почты.");
                    return;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), cs.Token);
                    return;
                }
                catch (TaskCanceledException)
                {
                }

                //*/
                #endregion


                Connect obj = new Connect("http://mymeriva.com/reg.aspx");

                obj.IsProxy = isProxy;
                obj.IpPortOfProxy = _currentProxy;
                if (checkBoxProxyPass.Checked)
                {
                    obj.IsByPass = true;
                    obj.ProxyLogin = _proxyLogin;
                    obj.ProxyPass = _proxyPass;
                }

                obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                obj.IsKeepAlive = true;
                obj.IsRedirect = true;

                obj.IsDebbug = true;
                obj.IsExtendDebbug = true;

                #region Get main page

                obj.UserAgent = _ua;

                string ans = obj.RequestGet(listBox1);
                if (string.IsNullOrEmpty(ans))
                {
                    listBox1.Items.Add("Пустой ответ от сервера.");
                    return;
                }

                File.AppendAllText("ERRORS/main.html", ans);

                #endregion



                #region define data

                Faker fake = new Faker();
                string name = fake.Name.FirstName();
                string lastName = fake.Name.LastName();
                Random rand = new Random();

                string userName = fake.Internet.UserName().ToLower();
                while (true)
                {
                    if (userName.Length > 8)
                    {
                        break;
                    }

                    userName = fake.Internet.UserName().ToLower();
                }
                userName = userName.Replace("_", string.Empty).Replace(".", string.Empty) + rand.Next(1, 100);


                string word = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                char letter = (char)new Random().Next(97, 122);
                char bigLetter = (char)new Random().Next(65, 90);
                int position = new Random().Next(word.Length);
                _pass = word.Insert(position, letter.ToString() + bigLetter.ToString() + new Random().Next(100));

                label1.Text = "Pass: " + _pass;

                if (string.IsNullOrEmpty(_pass)) return;
                if (string.IsNullOrEmpty(userName)) return;
                if (string.IsNullOrEmpty(name)) return;
                if (string.IsNullOrEmpty(lastName)) return;

                #endregion



                // captcha

                #region Captcha

                CancellationTokenSource source = new CancellationTokenSource();

                this.Invoke(new MethodInvoker(() =>
                {

                    label2.Text = "CAPTCHA START";
                    label2.BackColor = Color.Chartreuse;


                    Captcha("http://mymeriva.com/reg.aspx", 1);

                    TimerN(() =>
                    {
                        if (_isCaptchaOk)
                        {
                            source.Cancel();
                            return true;
                        }

                        return false;
                    }, 2000);

                }));

                #endregion

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(15), source.Token);
                    return;
                }
                catch (TaskCanceledException)
                {
                }




                // 6-8 доменных имен за одну регестрацию
                // на 1 ip - 2000 доменов

                bool res = await WebRepeatIfError(() =>
                {
                    #region Post request

                    obj.URL = new Uri("http://mymeriva.com/regaction.aspx");
                    obj.NameValColl = new NameValueCollection();
                    obj.NameValColl.Add("Cache-Control", "no-cache");

                    string fullName = fake.Name.FullName().ToLower();

                    listBox1.Items.Add("");
                    listBox1.Items.Add(fullName);
                    listBox1.Items.Add(userName);

                    string to =
                        string.Format(
                            "fname={0}&uid={1}&email={2}&pass={3}&passconf={3}&g-recaptcha-response={4}",
                            WebUtility.UrlEncode(fullName),
                            userName,
                            _currentMail,
                            _pass,
                            _resultCaptch
                            );

                    ans = obj.RequestPost(to, listBox1);

                    File.AppendAllText("ERRORS/main2.html", ans);

                    // filter
                    if (!ans.Contains("<form action=\"confirmaction.aspx\""))
                    {
                        label2.BackColor = Color.Red;
                        listBox1.Items.Add("Something wrong !!!");
                        return false;
                    }

                    #endregion

                    return true;

                });

                if (!res) return;

                label2.Text = "Waiting e-mail";

                // save
                File.AppendAllText(_pathToSave, userName + ";" + _pass + ";" + _currentMail + Environment.NewLine);

                #region Waiting confirmaction from e-mail

                this.Invoke(new MethodInvoker(() => TimerN(() =>
                {
                    if (!string.IsNullOrEmpty(_linkTo))
                    {
                        label2.BackColor = Color.Yellow;

                        listBox1.Items.Add("   2 PAGE " + _linkTo);

                        Task.Factory.StartNew(() => RUN2(tokenCanc, isProxy, obj), _cancel.Token);

                        return true;
                    }
                    return false;
                }, 2000)));

                #endregion


            }
            catch (WebException web)
            {
                _errorCount++;

                listBox1.Items.Add(_errorCount + ") RUN(): " + web.GetBaseException().ToString());

                if (_errorCount >= 3)
                {
                    _errorCount = 0;
                    buttonDeleteProxy.PerformClick();
                    this.Invoke(new MethodInvoker(buttonSTART.PerformClick));
                    return;
                }

                RUN(tokenCanc, isProxy);
            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }



        private async void RUN2(CancellationToken tokenCanc, bool isProxy, Connect obj = null)
        {
            try
            {

                listBox1.Items.Add("-------R U N 2-------");
                if (tokenCanc.IsCancellationRequested) return;
                if (isProxy) listBox1.Items.Add("@@@@@@@ Proxy is " + isProxy);

                if (obj == null)
                {
                    #region if null

                    obj = new Connect();

                    obj.IsProxy = isProxy;
                    obj.IpPortOfProxy = _currentProxy;
                    if (checkBoxProxyPass.Checked)
                    {
                        listBox1.Items.Add("@@@@Используем прокси по паролю. isProxy = " + isProxy);

                        obj.IsByPass = true;
                        obj.ProxyLogin = _proxyLogin;
                        obj.ProxyPass = _proxyPass;
                    }

                    obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
                    obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    obj.IsKeepAlive = true;
                    obj.IsRedirect = true;

                    obj.IsDebbug = true;
                    obj.IsExtendDebbug = true;

                    #endregion
                }

                obj.ClearAllHeaders();
                obj.UserAgent = _ua;
                obj.NameValColl = new NameValueCollection();
                obj.NameValColl.Add("Cache-Control", "no-cache");

                CancellationTokenSource source = null;
                string ans = null;
                string to = null;

                if (!string.IsNullOrEmpty(_linkTo))
                {

                    #region confirmation

                    // captcha 2

                    #region Captcha

                    this.Invoke(new MethodInvoker(() =>
                    {
                        source = new CancellationTokenSource();

                        label2.Text = "CAPTCHA START 2";
                        label2.BackColor = Color.Chartreuse;

                        Captcha("http://mymeriva.com/confirm.aspx", 2);

                        TimerN(() =>
                        {
                            if (_isCaptchaOk)
                            {
                                source.Cancel();
                                return true;
                            }

                            return false;
                        }, 2000);

                    }));

                    #endregion

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(15), source.Token);
                        return;
                    }
                    catch (TaskCanceledException)
                    {
                    }

                    bool res = await WebRepeatIfError(() =>
                    {

                        obj.URL = new Uri("http://mymeriva.com/confirmaction.aspx");
                        obj.UserAgent = _ua;
                        obj.NameValColl = new NameValueCollection();
                        obj.NameValColl.Add("Cache-Control", "no-cache");

                        to = string.Format("email={0}&key={1}&g-recaptcha-response={2}", _currentMail, _linkTo, _resultCaptch);

                        ans = obj.RequestPost(to, listBox1);

                        File.AppendAllText("ERRORS/pageOfCreateDomains.html", ans);

                        if (!ans.Contains("stepaction.aspx"))
                        {
                            label2.BackColor = Color.Red;
                            listBox1.Items.Add("Что то не так. Мы не смогли получить доступ к странице по созданию доменных имен.");
                            return false;
                        }

                        return true;

                    });

                    if (!res) return;

                    #endregion


                    listBox1.Items.Add("Код подтверждения из письма - активирован.");
                }
                else
                {
                    #region Login In

                    if (!File.Exists(_pathToSave)) return;
                    string[] accs = File.ReadAllLines(_pathToSave);

                    var res = accs.LastOrDefault();

                    if (string.IsNullOrEmpty(res)) return;

                    string[] spl = res.Split(':', ';');

                    if (spl.Length < 2) return;

                    string login = spl[0];
                    string password = spl[1];

                    listBox1.Items.Add("Account: " + login + ";" + password);

                    obj.URL = new Uri("http://mymeriva.com/login.aspx");

                    ans = obj.RequestGet(listBox1);

                    if (!ans.Contains("loginaction.aspx"))
                    {
                        label2.BackColor = Color.Red;
                        listBox1.Items.Add("Что то не так. На странице отсутствует форма авторизации.");
                        return;
                    }


                    // captcha 2

                    #region Captcha

                    this.Invoke(new MethodInvoker(() =>
                    {
                        source = new CancellationTokenSource();

                        label2.Text = "CAPTCHA START 2";
                        label2.BackColor = Color.Chartreuse;

                        Captcha("http://mymeriva.com/login.aspx", 2);

                        TimerN(() =>
                        {
                            if (_isCaptchaOk)
                            {
                                source.Cancel();
                                return true;
                            }

                            return false;
                        }, 2000);

                    }));

                    #endregion

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(15), source.Token);
                        return;
                    }
                    catch (TaskCanceledException)
                    {
                    }

                    obj.URL = new Uri("http://mymeriva.com/loginaction.aspx");

                    to = string.Format("uid={0}&pass={1}&g-recaptcha-response={2}", login, password, _resultCaptch);

                    ans = obj.RequestPost(to, listBox1);

                    File.AppendAllText("ERRORS/pageOfLoginIn.html", ans);

                    if (!ans.Contains("stepaction.aspx"))
                    {
                        label2.BackColor = Color.Red;
                        listBox1.Items.Add("Что то не так. Мы не смогли получить доступ к странице по созданию доменных имен.");
                        return;
                    }

                    #endregion

                    listBox1.Items.Add("Login In Мы в личном кабинете.");
                }



                // L O O P


                int z = 1;
                do
                {
                    var mass = File.ReadAllLines(_nod);
                    string domain = mass.OrderByDescending(s => Guid.NewGuid().ToString()).ToArray()[0].Trim();


                    listBox1.Items.Add("-------" + z + " == " + numericUpDown1.Value);

                    // captcha 3

                    #region Captcha

                    this.Invoke(new MethodInvoker(() =>
                    {
                        source = new CancellationTokenSource();

                        label2.Text = "CAPTCHA START 2";
                        label2.BackColor = Color.Chartreuse;

                        Captcha("http://mymeriva.com/devices.aspx", z);

                        TimerN(() =>
                        {
                            if (_isCaptchaOk)
                            {
                                source.Cancel();
                                return true;
                            }

                            return false;
                        }, 2000);

                    }));

                    #endregion

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(15), source.Token);
                        return;
                    }
                    catch (TaskCanceledException)
                    {
                    }


                    bool res = await WebRepeatIfError(() =>
                    {

                        #region create domain name

                        obj.URL = new Uri("http://mymeriva.com/stepaction.aspx");

                        to =
                            string.Format(
                                "hst={0}&dom=mymeriva.com&ip={1}&g-recaptcha-response={2}", domain, _ip, _resultCaptch);

                        ans = obj.RequestPost(to, listBox1);

                        File.AppendAllText("ERRORS/CreateDomains_" + z + ".html", ans);

                        if (!ans.Contains("Your domain was created"))
                        {
                            if (ans.Contains("Google reCAPTCHA error"))
                            {
                                label2.BackColor = Color.Red;
                                listBox1.Items.Add("Что то не так. Google reCAPTCHA error.");

                                return true;
                            }

                            if (ans.Contains("Domain already in use"))
                            {
                                label2.BackColor = Color.Red;
                                listBox1.Items.Add("Что то не так. Domain already in use.");
                                RemSingleName(domain, _nod);
                                return true;
                            }

                            label2.BackColor = Color.Red;
                            listBox1.Items.Add("Что то не так. Мы не смогли создать доменное имя.");
                            return false;
                        }

                        #endregion

                        return true;

                    });

                    if (!res) return;

                    // SAVE
                    label2.BackColor = Color.Magenta;
                    label2.Text = "F I N I S H";

                    File.AppendAllText(_domainsPath, domain + ".mymeriva.com;" + _pass + ";" + _currentMail + ";" + _ip + Environment.NewLine);


                    RemSingleName(domain, _nod, z);



                    z++;
                } while (z <= numericUpDown1.Value);

                listBox1.Items.Add("F I N I S H");

                #region clear all data

                _linkTo = null;
                RemoveMail(_currentMail);
                _currentMail = null;

                if (isProxy)
                {
                    buttonDeleteProxy.PerformClick();
                }



                #endregion


            }
            catch (WebException web)
            {
                listBox1.Items.Add("RUN2(): " + web.GetBaseException().ToString());

                _errorCount++;

                if (_errorCount > 2)
                {
                    this.Invoke(new MethodInvoker(() => buttonDeleteProxy.PerformClick()));
                    this.Invoke(new MethodInvoker(() => buttonSTART.PerformClick()));

                    return;
                }

                // RUN2(tokenCanc, isProxy, obj);
            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }


        #endregion


        #region Somee

        private void SomeeRUN(CancellationToken tokenCanc, bool isProxy = true)
        {
            try
            {
                List<string> log = new List<string>();
                Crowler crw = new Crowler();

                if (_worker.CancellationPending) return;
                this.Invoke(new MethodInvoker(() => buttonSTART.Visible = false));

                _cancelEmail = new CancellationTokenSource();
                CancellationTokenSource cs = new CancellationTokenSource();

                if (checkBoxTempMail.Checked)
                {

                    #region temp e-mail

                    this.Invoke(new MethodInvoker(() =>
                    {
                        Connect mail = new Connect();
                        mail.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                        mail.IsKeepAlive = true;
                        mail.IsRedirect = true;
                        mail.UserAgent = _ua;

                        #region Temp e-mail

                        TimerN(() =>
                        {

                            if (_cancelEmail.IsCancellationRequested)
                            {
                                return true;
                            }

                            label3.Text = $"Mails: {_box}";
                            _box++;

                            if (_box >= 130)
                            {
                                File.AppendAllText("ERRORS/temp/tempMailErr.txt", _currentMail.Split('@').LastOrDefault() + Environment.NewLine);

                                // удалить домен
                                var m = _currentMail.Split('@')[1];
                                RemSingleName(m, @"ERRORS\temp\tempMailDomains.txt");
                                return true;
                            }

                            if (!cs.IsCancellationRequested)
                            {
                                #region check if valid e-mail

                                mail.URL = new Uri("https://generator.email/check_adres_validation3.php");
                                mail.NameValColl = new NameValueCollection();
                                mail.NameValColl.Add("X-Requested-With", "XMLHttpRequest");
                                var arr = _currentMail.Split('@');
                                string to = string.Format("usr={0}&dmn={1}", arr[0], arr[1]);
                                string ansA = mail.RequestPost(to, listBox1);
                                mail.NameValColl = null;

                                dynamic d = JsonConvert.DeserializeObject<dynamic>(ansA);

                                if (d.status.ToString().Equals("bad"))
                                {
                                    return true;
                                }

                                #endregion
                            }

                            cs.Cancel();

                            mail.URL = new Uri("https://generator.email/" + _currentMail);
                            string ansM = null;
                            try
                            {
                                ansM = mail.RequestGet(listBox1);
                            }
                            catch (WebException wex)
                            {
                                return false;
                            }

                            if (ansM.Contains("Email verification"))
                            {
                                _box = 0;

                                List<string> urls = GetUrlsFromPage(ansM, "a", "Email verification", true);
                                if (urls == null) return true;
                                foreach (string r in urls)
                                {

                                    string tempUrl = "https://generator.email" + r;
                                    mail.URL = new Uri(tempUrl);
                                    ansM = mail.RequestGet(listBox1);

                                    var m = Regex.Match(ansM, "(?<=following\\ validation\\ code:)[\\w\\W]*?(?=Regards,)");
                                    if (m.Success)
                                    {
                                        _linkTo += m.Value.Trim() + ";";
                                        listBox1.Items.Add("monkey: " + _linkTo);

                                    }

                                }

                                return true;
                            }

                            return false;
                        }, 5000);

                        #endregion
                         
                    }));
                    #endregion
                }
                else
                {

                    #region E-mail

                    if (checkBoxDeleteEmail.Checked) checkBoxDeleteEmail.Checked = false;

                    object combo = comboBoxEmail.SelectedItem;
                    if (combo != null)
                    {

                        if (string.IsNullOrEmpty(_currentMail))
                        {
                            return;
                        }


                        Task.Factory.StartNew(() => EmailPop3(combo.ToString(), _currentMail, _mailPass, cs));
                    }
                    else
                    {
                        this.Invoke(new MethodInvoker(() => listBox1.Items.Add("Вы не указали сервер для почты.")));
                        return;
                    }

                    #endregion

                }

                try
                {
                    var t = Task.Delay(TimeSpan.FromMinutes(2), cs.Token);
                    t.Wait();
                    return;
                }
                catch (Exception ex)
                {

                }
                log.Add("---we are after email"); 

                Connect obj = new Connect(_urlSomee);

                obj.IsProxy = isProxy;
                obj.IpPortOfProxy = _currentProxy;
                if (checkBoxProxyPass.Checked)
                {
                    obj.IsByPass = true;
                    obj.ProxyLogin = _proxyLogin;
                    obj.ProxyPass = _proxyPass;
                }

                obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                obj.IsKeepAlive = true;
                obj.IsRedirect = true;

                obj.IsDebbug = true;
                obj.IsExtendDebbug = true;

                #region Get main page

                obj.UserAgent = _ua; 
                string ans = obj.RequestGet(log); 
                string orderNowLink = Regex.Match(ans, "<a href=\"(.*?)\">Order now</a>").Groups[1].Value;

                if (string.IsNullOrEmpty(orderNowLink))
                {
                    return;
                }

                // перейти на страницу формы

                obj.URL = new Uri(orderNowLink); 
                ans = obj.RequestGet(log); 
                File.AppendAllText("ERRORS/main.html", ans);

                #endregion

                _worker.ReportProgress(0, log);
               
                #region define data

                Faker fake = new Faker();
                string name = fake.Name.FirstName();
                string lastName = fake.Name.LastName();

                string userName = _currentMail.Split('@').FirstOrDefault();
                if (string.IsNullOrEmpty(userName)) return;
                _userName = userName;

                string word = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
                char letter = (char)new Random().Next(97, 122);
                char bigLetter = (char)new Random().Next(65, 90);
                int position = new Random().Next(word.Length);
                _pass = word.Insert(position, letter.ToString() + bigLetter.ToString() + new Random().Next(100));

                log.Add("Pass: " + _pass);

                if (string.IsNullOrEmpty(_pass)) return;
                if (string.IsNullOrEmpty(userName)) return;
                if (string.IsNullOrEmpty(name)) return;
                if (string.IsNullOrEmpty(lastName)) return;

                #endregion

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(ans);
                var root = doc.DocumentNode;
                var form = root.SelectSingleNode("//form");
                if (form == null) return;
                var inputs = form.Descendants("input");

                var param = new StringBuilder();

                #region Get url of image

                string formUrl = Regex.Match(ans, "<form method=\"post\" action=\"(.*?)\"").Groups[1].Value;

                formUrl = (formUrl.FirstOrDefault() == '.' ? formUrl.Substring(1) : formUrl);

                Uri com = new Uri(orderNowLink);

                formUrl = com.GetLeftPart(UriPartial.Authority) +
                          Path.GetDirectoryName(com.LocalPath).Replace("\\", "/") + formUrl;



                param.AppendFormat("{0}={1}&", Uri.EscapeDataString("ctl00$ScriptManager1"), Uri.EscapeDataString("ctl00$CPH1$DORegistration1$DOValidationID1$UpdatePanel1|ctl00$CPH1$DORegistration1$DOValidationID1$Timer1"));
                param.AppendFormat("{0}={1}&", Uri.EscapeDataString("__EVENTTARGET"), Uri.EscapeDataString("ctl00$CPH1$DORegistration1$DOValidationID1$Timer1"));



                foreach (HtmlNode input in inputs)
                {
                    string nameA = input.GetAttributeValue("name", null);
                    string valueB = input.GetAttributeValue("value", null);

                    if (string.IsNullOrEmpty(valueB)) valueB = string.Empty;

                    if (nameA.Contains("CheckBoxAgreeToTerms")) continue;
                    if (nameA.Contains("ButtonRegister")) continue;
                    if (nameA.Contains("__EVENTTARGET")) continue;

                    if (nameA.Contains("__VIEWSTATE"))
                    {
                        valueB = Uri.EscapeDataString(valueB);
                    }
                    if (nameA.Contains("__EVENTVALIDATION"))
                    {
                        valueB = Uri.EscapeDataString(valueB);
                    }

                    param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), valueB);

                }


                param.Append("__ASYNCPOST=true");

                File.AppendAllText("ERRORS/params.txt", param.ToString());

                obj.URL = new Uri("https://somee.com/DOKA/DOC/DOLoginOrRegister.aspx?docode=false&sl=false");
                obj.Accept = "*/*";
                //obj.Refferer = orderNowLink;
                obj.NameValColl = new NameValueCollection();
                obj.NameValColl.Add("X-Requested-With", "XMLHttpRequest");
                obj.NameValColl.Add("X-MicrosoftAjax", "Delta=true");
                obj.NameValColl.Add("Cache-Control", "no-cache");
                obj.NameValColl.Add("Origin", "https://somee.com");

                ans = obj.RequestPost(param.ToString(), log);

                File.AppendAllText("ERRORS/getUrlImage.html", ans);

                var img =
                    Regex.Match(ans,
                        "<img id=\"ctl00_CPH1_DORegistration1_DOValidationID1_RepeaterValidim_ctl00_Im1\" src=\"(.*?)\"");


                if (!img.Success) return;

                string urlImage = "https://somee.com/DOKA" + img.Groups[1].Value.Remove(0, 2);

                #endregion


                #region http: save image.jpg

                if (File.Exists("ERRORS/captcha.jpg")) File.Delete("ERRORS/captcha.jpg");



                obj.ClearAllHeaders();
                obj.URL = new Uri(urlImage);
                obj.Accept = "image/png, image/svg+xml, image/*;q=0.8, */*;q=0.5";

                obj.RequestGet(log, null, true);

                Stream stream = obj.Response.GetResponseStream();

                if (stream == null)
                {
                    log.Add("stream of image is null");

                    return;
                }
                byte[] InBuffer;
                using (BinaryReader bread = new BinaryReader(stream))
                {
                    // считывать поток кусочками указанных байтов
                    InBuffer = bread.ReadBytes(1024);

                    using (MemoryStream mem = new MemoryStream())
                    {
                        while (InBuffer.Length > 0)
                        {
                            // записать в память потока - данные
                            mem.Write(InBuffer, 0, InBuffer.Length);

                            InBuffer = bread.ReadBytes(1024);

                            log.Add("Lenght: " + InBuffer.Length);
                        }

                        log.Add("В выделенной памяти потока: " + mem.Length + " байт.");
                        log.Add("В сохранненом массиве: " + InBuffer.Length + " байт.");

                        byte[] InFile = new byte[mem.Length];
                        mem.Position = 0;
                        mem.Read(InFile, 0, InFile.Length);

                        using (FileStream fs = new FileStream("ERRORS/captcha.jpg", FileMode.Create))
                        {
                            fs.Write(InFile, 0, InFile.Length);
                        }

                        stream.Close();
                        obj.Response.Close();
                    }
                }


                if (!File.Exists("ERRORS/captcha.jpg")) return;

                #endregion

                
                /////////////////////////CAPTCHA///////////////////////// 
                string myCaptcha = null;

                #region Captcha
                /*
                label2.Text = "CAPTCHA START";
                label2.BackColor = Color.Chartreuse;
                CaptchaResult res = null;

                if (radioButtnAntiCaptchaImageText.Checked)
                {
                    res = await AntiCaptcha("ERRORS/captcha.jpg", 5);
                }
                else if (radioButtn2CaptchImageText.Checked)
                {
                    res = await UploadCaptchaFile("ERRORS/captcha.jpg", 5);
                }
                else
                {
                    return;
                }

                if (res.Success)
                {
                    myCaptcha = res.Response.Trim().ToLower();
                    listBox1.Items.Add("   Success: " + myCaptcha);
                }
                else
                {
                    if (radioButtnAntiCaptchaImageText.Checked) return;

                    this.Invoke(new MethodInvoker(() =>
                    {
                        label2.BackColor = Color.Red;
                        label2.Text = "Captcha unsolvable.";
                        _cancelEmail.Cancel();

                        if (res.Response.Trim() == "ERROR_CAPTCHA_UNSOLVABLE")
                        {
                            // за этот ответ денег не берут
                            buttonSTART.Visible = true;
                            buttonSTART.PerformClick();
                        }
                        else
                        {
                            string result = StatusCaptcha(CaptchaInfo.reportbad);
                            listBox1.Items.Add(result.Trim() + " == OK_REPORT_RECORDED");
                            if (result.Trim() == "OK_REPORT_RECORDED")
                            {
                                listBox1.Items.Add("yes == " + result.Trim());

                                buttonSTART.Visible = true;
                                buttonSTART.PerformClick();

                            }
                        }
                    }));
                    return;
                }

                */
                #endregion

                this.Invoke(new MethodInvoker(() =>
                {
                    CaptchaSolver solver = new CaptchaSolver();
                    myCaptcha = solver.Resolver(pictureBox1, labelAnsCaptcha);
                }));

                if (myCaptcha is null) return;

                Task<bool> repeat = WebRepeatIfError(() =>
                {
                    #region Post request

                    obj.URL = new Uri("https://somee.com/DOKA/DOC/DOLoginOrRegister.aspx?docode=false&sl=false");
                    obj.NameValColl = new NameValueCollection();
                    obj.NameValColl.Add("Cache-Control", "no-cache");


                    param.Clear();

                    foreach (HtmlNode input in inputs)
                    {
                        string nameA = input.GetAttributeValue("name", null);
                        string valueB = input.GetAttributeValue("value", null);

                        if (nameA.Contains("TextBoxFirstName")) valueB = name;
                        if (nameA.Contains("TextBoxLastName")) valueB = lastName;
                        if (nameA.Contains("TextBoxUserID")) valueB = userName;
                        if (nameA.Contains("TextBoxPassword")) valueB = _pass;
                        if (nameA.Contains("TextBoxPasswordConf")) valueB = _pass;
                        if (nameA.Contains("TextBoxEmail")) valueB = _currentMail;
                        if (nameA.Contains("TextBoxValidationNumber")) valueB = myCaptcha;
                        if (nameA.Contains("CheckBoxAgreeToTerms")) valueB = "on";
                        if (nameA.Contains("ButtonRegister")) valueB = WebUtility.UrlEncode(valueB);

                        if (nameA.Contains("__VIEWSTATE"))
                        {
                            valueB = Uri.EscapeDataString(valueB);
                        }
                        if (nameA.Contains("__EVENTVALIDATION"))
                        {
                            valueB = Uri.EscapeDataString(valueB);
                        }

                        param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), valueB);

                    }

                    param.Length -= 1;

                    File.AppendAllText("ERRORS/params2.txt", param.ToString());

                    ans = obj.RequestPost(param.ToString(), log);

                    File.AppendAllText("ERRORS/validationCode.html", ans);

                    // filter
                    if (!ans.Contains("DOEmailVerification.aspx"))
                    {
                        var err = GetErrorFromPage(ans, "span", "class", "validatorCtl");

                        if (err.Contains("Incorrect validation ID!"))
                        {
                            //listBox1.Items.Add("Wrong captcha: Incorrect validation ID!");
                            _cancelEmail.Cancel();

                            return false;//use new captcha

                            if (radioButtnAntiCaptchaImageText.Checked) return false;

                            string result = StatusCaptcha(CaptchaInfo.reportbad);
                            if (result.Contains("OK_REPORT_RECORDED"))
                            {
                            }
                            return false;
                        }



                        return false;
                    }


                    // save data of page
                    doc.LoadHtml(ans);
                    root = doc.DocumentNode;
                    form = root.SelectSingleNode("//form");
                    if (form != null)
                    {
                        inputs = form.Descendants("input");
                        foreach (HtmlNode input in inputs)
                        {
                            string nameA = input.GetAttributeValue("name", null);
                            string valueB = input.GetAttributeValue("value", null);

                            if (nameA.Equals("__VIEWSTATE"))
                            {
                                __VIEWSTATE = Uri.EscapeDataString(valueB);
                            }

                            if (nameA.Equals("__EVENTVALIDATION"))
                            {
                                __EVENTVALIDATION = Uri.EscapeDataString(valueB);
                            }

                            if (nameA.Equals("__VIEWSTATEGENERATOR"))
                            {
                                __VIEWSTATEGENERATOR = Uri.EscapeDataString(valueB);
                            }

                        }

                    }

                    #endregion

                    return true;

                });
                repeat.Wait();

               

                if (!repeat.Result)
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        buttonDeleteProxy.PerformClick();
                        buttonSTART.Visible = true;
                        buttonSTART.PerformClick();
                    }));
                    return;

                }


                SaveAllCookie(obj.Container, log);

                #region Waiting confirmaction from e-mail

                var ce = new CancellationTokenSource();
                this.Invoke(new MethodInvoker(() =>
                {
                    TimerN(() =>
                    {
                        if (!string.IsNullOrEmpty(_linkTo))
                        {
                            listBox1.Items.Add($"timer end {_linkTo}");
                            ce.Cancel();
                            return true;
                        }
                        return false;
                    }, 1000);
                }));
                try
                {
                    var t = Task.Delay(TimeSpan.FromMinutes(2), ce.Token);
                    t.Wait();
                    return;
                }
                catch (Exception ex)
                {

                }
         
                SomeeRUN2(tokenCanc, isProxy, obj);


                #endregion


            }
            catch (WebException web)
            {
                #region

                _errorCount++;

                //listBox1.Items.Add(_errorCount + ") SomeeRUN(): " + web.GetBaseException().ToString());

                if (_errorCount >= 3)
                {
                    _errorCount = 0;

                    this.Invoke(new MethodInvoker(() =>
                    {
                        buttonDeleteProxy.PerformClick();
                        buttonSTART.Visible = true;
                        buttonSTART.PerformClick();
                    }));
                    return;
                }

                SomeeRUN(tokenCanc, isProxy);

                #endregion

            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }


        private async void SomeeRUN2(CancellationToken tokenCanc, bool isProxy, Connect obj = null)
        {
            try
            {
                List<string> log = new List<string>();
                log.Add("-------Somee R U N 2-------");
                if (tokenCanc.IsCancellationRequested) return;
                if (isProxy) log.Add("@@@@@@@ Proxy is " + isProxy);

                _worker.ReportProgress(0, log);

                var doc = new HtmlAgilityPack.HtmlDocument();
                HtmlNode root, form;
                var param = new StringBuilder();
                string ans = null, to = null;
                string domain = null, ip = null;
                IEnumerable<HtmlNode> inputs = null;

                if (obj == null)
                {
                    #region if null

                    obj = new Connect(_urlSomee);

                    obj.IsProxy = isProxy;
                    obj.IpPortOfProxy = _currentProxy;
                    if (checkBoxProxyPass.Checked)
                    {
                        log.Add("@@@@Используем прокси по паролю. isProxy = " + isProxy);

                        obj.IsByPass = true;
                        obj.ProxyLogin = _proxyLogin;
                        obj.ProxyPass = _proxyPass;
                    }

                    obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
                    obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    obj.IsKeepAlive = true;
                    obj.IsRedirect = true;

                    obj.IsDebbug = true;
                    obj.IsExtendDebbug = true;

                    #region Get main page

                    obj.UserAgent = _ua;

                    ans = obj.RequestGet(listBox1);

                    string orderNowLink = Regex.Match(ans, "<a href=\"(.*?)\" id=\"ctl00_ctl00_A3\">Login</a>").Groups[1].Value;

                    if (string.IsNullOrEmpty(orderNowLink))
                    {
                        log.Add("Login the link is empty.");
                        return;
                    }

                    // перейти на страницу формы

                    obj.URL = new Uri(orderNowLink);

                    ans = obj.RequestGet(listBox1);

                    File.AppendAllText("ERRORS/login.html", ans);

                    #endregion


                    #region Log In

                    doc.LoadHtml(ans);
                    root = doc.DocumentNode;
                    form = root.SelectSingleNode("//form");
                    if (form == null) return;
                    inputs = form.Descendants("input");

                    foreach (HtmlNode input in inputs)
                    {
                        string nameA = input.GetAttributeValue("name", null);
                        string valueB = input.GetAttributeValue("value", null);

                        if (string.IsNullOrEmpty(valueB)) valueB = string.Empty;

                        if (nameA.Contains("CheckBoxAgreeToTerms")) continue;
                        if (nameA.Contains("ButtonRegister")) continue;
                        if (nameA.Contains("RememberMe")) continue;

                        if (nameA.Contains("UserName"))
                        {
                            valueB = _currentMail;
                        }

                        if (nameA.Contains("Password"))
                        {
                            valueB = _pass;
                        }

                        param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), Uri.EscapeDataString(valueB));

                    }

                    param.Length -= 1;

                    File.AppendAllText("ERRORS/params.txt", param.ToString());

                    obj.URL = new Uri("https://somee.com/DOKA/DOC/DOLoginOrRegister.aspx");
                    obj.NameValColl = new NameValueCollection();
                    obj.NameValColl.Add("Cache-Control", "no-cache");

                    ans = obj.RequestPost(param.ToString(), log);

                    log.Add("Ans: " + ans);

                    File.AppendAllText("ERRORS/LogIn_2.html", ans);


                    #endregion


                    #endregion
                }

                if (ans == null)
                {
                    if (!string.IsNullOrEmpty(_linkTo))
                    {
                        #region Autentifacation

                        var arr = _linkTo.Split(';');
                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(arr[i])) continue;

                            log.Add(arr[i]);

                            obj.URL = new Uri("https://somee.com/DOKA/DOC/DOEmailVerification.aspx");

                            obj.ClearAllHeaders();
                            obj.NameValColl = new NameValueCollection();
                            obj.NameValColl.Add("Cache-Control", "no-cache");

                            to =
                                string.Format(
                                    "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={0}&__VIEWSTATEGENERATOR={1}&__EVENTVALIDATION={2}&ctl00%24CPH1%24TBCode={3}&ctl00%24CPH1%24BConfirm=Confirm",
                                    __VIEWSTATE, __VIEWSTATEGENERATOR, __EVENTVALIDATION, arr[i]);

                            ans = obj.RequestPost(to, log);

                            File.AppendAllText("ERRORS/confirm.html", ans);
                        }

                        #endregion
                    }
                }

                if (ans.Contains("We just sent you an email with validation code"))
                {
                    #region Autentifacation

                    var arr = _linkTo.Split(';');
                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (string.IsNullOrEmpty(arr[i])) continue;

                        log.Add(arr[i]);

                        obj.URL = new Uri("https://somee.com/DOKA/DOC/DOEmailVerification.aspx");

                        obj.ClearAllHeaders();
                        obj.NameValColl = new NameValueCollection();
                        obj.NameValColl.Add("Cache-Control", "no-cache");

                        to =
                            string.Format(
                                "__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE={0}&__VIEWSTATEGENERATOR={1}&__EVENTVALIDATION={2}&ctl00%24CPH1%24TBCode={3}&ctl00%24CPH1%24BConfirm=Confirm",
                                __VIEWSTATE, __VIEWSTATEGENERATOR, __EVENTVALIDATION, arr[i]);

                        ans = obj.RequestPost(to, log);

                        File.AppendAllText("ERRORS/confirm.html", ans);
                    }

                    #endregion
                }

                if (ans.Contains("DOStoreCheckout.aspx"))
                {
                    log.Add("Код подтверждения из письма - активирован.");
                }
                else if (ans.Contains("DOUserDefault.aspx"))
                {
                    log.Add("-----------------");

                    #region Store

                    obj.ClearAllHeaders();
                    obj.URL = new Uri("https://somee.com/DOKA/DOC/DOCommonStore.aspx"); 
                    ans = obj.RequestGet(listBox1);

                    log.Add("Ans: " + ans);

                    File.AppendAllText("ERRORS/Store.html", ans);


                    #endregion

                    log.Add("-----------------");

                    #region Free hosting select

                    doc.LoadHtml(ans);
                    root = doc.DocumentNode;
                    form = root.SelectSingleNode("//form");
                    if (form == null) return;
                    var inpts = form.Descendants("input");
                    param.Clear();


                    foreach (HtmlNode input in inpts)
                    {
                        string nameA = input.GetAttributeValue("name", null);
                        string valueB = input.GetAttributeValue("value", null);

                        if (string.IsNullOrEmpty(valueB)) valueB = string.Empty;
                        if (nameA == null) continue;

                        param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), Uri.EscapeDataString(valueB));

                    }

                    // указать что это бесплатный хостинг
                    param.AppendFormat("{0}={1}", Uri.EscapeDataString("ctl00$ctl00$CPH1$CPH1$DOProductStore1$ListViewProducts$ctrl9$ButtonOrder"), Uri.EscapeDataString("ctl00$ctl00$CPH1$CPH1$DOProductStore1$ListViewProducts$ctrl9$ButtonOrder"));

                    File.AppendAllText("ERRORS/params2.txt", param.ToString());

                    obj.URL = new Uri("https://somee.com/DOKA/DOC/DOCommonStore.aspx");
                    obj.NameValColl = new NameValueCollection();
                    obj.NameValColl.Add("Cache-Control", "no-cache");

                    ans = obj.RequestPost(param.ToString(), log);

                    log.Add("Ans: " + ans);

                    File.AppendAllText("ERRORS/FreeHosting.html", ans);

                    if (!ans.Contains("DOStoreCheckout.aspx")) return;

                    #endregion

                    log.Add("-----------------");
                }
                else
                {
                    return;
                }

                _worker.ReportProgress(0, log);

                #region Checkout

                doc.LoadHtml(ans);
                root = doc.DocumentNode;
                form = root.SelectSingleNode("//form");
                if (form == null) return;
                inputs = form.Descendants("input");
                param.Clear();


                foreach (HtmlNode input in inputs)
                {
                    string nameA = input.GetAttributeValue("name", null);
                    string valueB = input.GetAttributeValue("value", null);

                    if (string.IsNullOrEmpty(valueB)) valueB = string.Empty;
                    if (nameA == null) continue;

                    if (nameA.Contains("ImageButtonRemoveProduct")) continue;
                    if (nameA.Contains("ImageButtonCheckOut")) continue;

                    if (nameA.Equals("__EVENTTARGET")) valueB = "ctl00$CPH1$LinkButtonCheckOut";

                    param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), Uri.EscapeDataString(valueB));

                }

                param.Length -= 1;

                var match = Regex.Match(ans, "<form method=\"post\" action=\"(.*?)\"");
                if (!match.Success) return;
                string url = match.Groups[1].Value.Substring(1);
                url = "https://somee.com/DOKA/DOU" + url;

                File.AppendAllText("ERRORS/params3.txt", param.ToString());

                obj.URL = new Uri(url);
                obj.NameValColl = new NameValueCollection();
                obj.NameValColl.Add("Cache-Control", "no-cache"); 
                ans = obj.RequestPost(param.ToString(), log);

                log.Add("Ans: " + ans);

                File.AppendAllText("ERRORS/Checkout.html", ans);

                if (ans.Contains("Free hosting package is limited in amount of  1 per account"))
                {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        label2.BackColor = Color.Red;
                        label2.Text = "waiting... Free hosting package is limited in amount of  1 per account";
                    }));

                    #region выйти в главное меню и зайти в раздел Websites

                    await Task.Delay(TimeSpan.FromSeconds(15));

                    obj.URL = new Uri("https://somee.com/DOKA/DOU/DOUserDefault.aspx");
                    ans = obj.RequestGet(log);

                    File.AppendAllText("ERRORS/dashBoard.html", ans);

                    await Task.Delay(TimeSpan.FromSeconds(10));

                    match = Regex.Match(ans,
                        "<a href=\"(DOMP/DOWebSites/DOWebSitesDefault.aspx.*?)\"");
                    if (!match.Success) return;
                    url = match.Groups[1].Value;
                    url = "https://somee.com/DOKA/DOU/" + url;


                    obj.URL = new Uri(url);
                    ans = obj.RequestGet(listBox1);
                    File.AppendAllText("ERRORS/websitesPage.html", ans);

                    // страница с формой для создания сайта
                    match = Regex.Match(ans,
                        "href=\"(DOCreateWebSite.aspx.*?)\">");

                    if (!match.Success)
                    {
                        // сайт по каким то причинам уже создан
                        match = Regex.Match(ans,
                    "href=\"(DOSiteDefault.aspx.*?)\"");
                        if (!match.Success) return;
                        domain = GetInnerText(ans, "span", "id", "ctl00_ctl00_CPH1_CPH1_ListViewWebsites_ctrl0_LabelSiteName");
                        if (!domain.Contains(".somee.com")) return;
                        url = "https://somee.com/DOKA/DOU/DOMP/DOWebSites/" + match.Groups[1].Value;

                        // website
                        obj.URL = new Uri(url);
                        ans = obj.RequestGet(listBox1);
                        File.AppendAllText("ERRORS/createSitePage2.html", ans);
                        ip = Regex.Match(ans, "href=\"ftp://(\\d{2,3}.\\d{2,3}.\\d{2,3}.\\d{2,3})").Groups[1].Value;

                        #region save all data and restart

                        File.AppendAllText(_domainsPathSomee, domain + ".somee.com;" + ip + ";" + _userName + ";" + _pass + ";" + _currentMail + Environment.NewLine);


                        RemSingleName(domain, _nod);

                        log.Add("F I N I S H");

                        #region clear all data

                        _userName = null;
                        _linkTo = null;
                        RemoveMail(_currentMail);
                        _currentMail = null;

                        this.Invoke(new MethodInvoker(() => buttonSTART.Visible = true));

                        if (isProxy)
                        {
                            if (checkBoxProxyPass.Checked)
                            {
                                _currentProxy = null;
                            }
                            else
                            {
                                _currentProxy = null;
                                //buttonDeleteProxy.PerformClick();
                            }
                        }



                        #endregion


                        await Task.Delay(TimeSpan.FromSeconds(5));

                        this.Invoke(new MethodInvoker(() =>
                        {
                            buttonSTART.Visible = true;
                            buttonSTART.PerformClick();
                        }));

                        #endregion

                        return;
                    }

                    url = match.Groups[1].Value;
                    url = "https://somee.com/DOKA/DOU/DOMP/DOWebSites/" + url;
                    obj.URL = new Uri(url);
                    ans = obj.RequestGet(log);
                    File.AppendAllText("ERRORS/createSitePage.html", ans);

                    #endregion

                }

                if (ans.Contains("DOCreateWebSite.aspx"))
                {
                    this.Invoke(new MethodInvoker(() => label2.BackColor = Color.Magenta));
                }

                #endregion

                log.Add("-----------------");
                _worker.ReportProgress(0, log);
                do
                {

                    var mass = File.ReadAllLines(_nod);
                    if (mass.Length == 0)
                    {
                        this.Invoke(new MethodInvoker(() => MessageBox.Show("you have not domain name.")));
                        return;
                    }
                    domain = mass.OrderByDescending(s => Guid.NewGuid().ToString()).ToArray()[0].Trim();
                    if (domain.Contains(';'))domain = domain.Split(';').FirstOrDefault();
                    

                    #region Create web site

                    doc.LoadHtml(ans);
                    root = doc.DocumentNode;
                    form = root.SelectSingleNode("//form");
                    if (form == null) return;
                    inputs = form.Descendants("input");
                    param.Clear();


                    foreach (HtmlNode input in inputs)
                    {
                        string nameA = input.GetAttributeValue("name", null);
                        string valueB = input.GetAttributeValue("value", null);

                        if (string.IsNullOrEmpty(valueB)) valueB = string.Empty;
                        if (nameA == null) continue;

                        // имя для сайта
                        if (nameA.Contains("TextBoxSiteName")) valueB = domain;

                        param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), Uri.EscapeDataString(valueB));

                    }

                    param.AppendFormat("{0}={1}&", "DropDownListHostingZone", "somee.com");
                    param.AppendFormat("{0}={1}&", "DropDownListOpSystem", Uri.EscapeDataString(_someeOsVersion));
                    param.AppendFormat("{0}={1}&", "DropDownListNetVersion", Uri.EscapeDataString(_someeAspVersion));
                    param.AppendFormat("{0}={1}", "TextBoxSiteDescription", "");

                    match = Regex.Match(ans, "<form method=\"post\" action=\"(.*?)\"");
                    if (!match.Success) return;
                    url = match.Groups[1].Value.Substring(1);
                    url = "https://somee.com/DOKA/DOU/DOMP/DOWebSites" + url;

                    File.AppendAllText("ERRORS/params4.txt", param.ToString());

                    obj.URL = new Uri(url);
                    obj.Refferer = url;
                    obj.NameValColl = new NameValueCollection();
                    obj.NameValColl.Add("Cache-Control", "no-cache");

                    ans = obj.RequestPost(param.ToString(), log);

                    log.Add("Ans: " + ans);

                    File.AppendAllText("ERRORS/CreateWebSite.html", ans);

                    if (!ans.Contains("You website was created successfully"))
                    {
                        if (ans.Contains("Site name already registred"))
                        {
                            this.Invoke(new MethodInvoker(() =>
                            {
                                label2.Text = "Site name already registred";
                                RemSingleName(domain, _nod);
                            }));
                            Task t12 = Task.Delay(TimeSpan.FromSeconds(5));
                            t12.Wait();
                            continue;
                        }
                        else
                        {
                            return;
                        }
                    }
                    this.Invoke(new MethodInvoker(() =>
                    {
                        label2.BackColor = Color.Yellow;
                        label2.Text = "Finish.";
                    }));
                    break;
                    #endregion

                } while (true);

                _worker.ReportProgress(0, log);

                ip = GetIpToFtp(obj);


                #region save all data and restart

                this.Invoke(new MethodInvoker(() =>
                {
                    File.AppendAllText(_domainsPathSomee, domain + ".somee.com;" + ip + ";" + _userName + ";" + _pass + ";" + _currentMail + Environment.NewLine);


                    RemSingleName(domain, _nod);

                    listBox1.Items.Add("F I N I S H");

                    #region clear all data

                    _userName = null;
                    _linkTo = null;
                    RemoveMail(_currentMail);
                    _currentMail = null;

                    buttonSTART.Visible = true;

                    if (isProxy)
                    {
                        if (checkBoxProxyPass.Checked)
                        {
                            _currentProxy = null;
                        }
                        else
                        {
                            _currentProxy = null;
                            //buttonDeleteProxy.PerformClick();
                        }
                    }



                    #endregion


                    buttonSTART.Visible = true;
                    buttonSTART.PerformClick();
                }));

                #endregion



            }
            catch (WebException web)
            {
                listBox1.Items.Add("SomeeRUN2(): " + web.GetBaseException().ToString());

                _errorCount++;

                if (_errorCount > 2)
                {
                    _errorCount = 0;
                    this.Invoke(new MethodInvoker(() =>
                    {
                        buttonDeleteProxy.PerformClick();
                        buttonSTART.Visible = true;
                        buttonSTART.PerformClick();
                    }));

                    return;
                }

                SomeeRUN2(tokenCanc, isProxy, null);
            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }

        private int _errCountIp;
        private string GetIpToFtp(Connect obj)
        {
            string ip = null;
            try
            {
                listBox1.Items.Add("----------begin ip------------");

                #region Get IP addres to ftp
                // dashboard
                obj.URL = new Uri("https://somee.com/DOKA/DOU/DOUserDefault.aspx");
                string ans = obj.RequestGet(listBox1);

                File.AppendAllText("ERRORS/dashBoard2.html", ans);

                var match = Regex.Match(ans,
                    "<a href=\"(DOMP/DOWebSites/DOWebSitesDefault.aspx.*?)\"");
                if (!match.Success) return null;
                var url = match.Groups[1].Value;
                url = "https://somee.com/DOKA/DOU/" + url;

                // websites
                obj.URL = new Uri(url);
                ans = obj.RequestGet(listBox1);


                File.AppendAllText("ERRORS/websitesPage2.html", ans);


                match = Regex.Match(ans,
                    "href=\"(DOSiteDefault.aspx.*?)\"");
                if (!match.Success) return null;
                url = "https://somee.com/DOKA/DOU/DOMP/DOWebSites/" + match.Groups[1].Value;

                // website
                obj.URL = new Uri(url);
                ans = obj.RequestGet(listBox1);
                File.AppendAllText("ERRORS/createSitePage2.html", ans);
                ip = Regex.Match(ans, "href=\"ftp://(\\d{2,3}.\\d{2,3}.\\d{2,3}.\\d{2,3})").Groups[1].Value;

                #endregion

                listBox1.Items.Add("----------end ip------------");

                return ip;
            }
            catch (WebException)
            {
                _errCountIp++;
                if (_errCountIp > 3)
                {
                    _errCountIp = 0;
                    return null;
                }
                return GetIpToFtp(obj);
            }
            catch (Exception)
            {

            }

            return null;
        }


        #endregion


        #region CAPTCHA

        #region 2captcha.com

        private int _idCount;
        private void Captcha(string url, int num, string action = null, string score = null)
        {
            try
            {
                // CAPTCHA 
                #region captha

                if (radioButtn2CaptchReCaptchaV2.Checked)
                {

                    #region filters


                    if (string.IsNullOrEmpty(url))
                    {
                        listBox1.Items.Add("Url  : ".ToUpper() + _sitekey);

                        return;
                    }

                    if (string.IsNullOrEmpty(_sitekey))
                    {
                        listBox1.Items.Add("siteKey  : ".ToUpper() + _sitekey);

                        return;
                    }

                    #endregion


                    listBox1.Items.Add(" TIMER  CAPTCHA: " + num + " ACTION: " + action + " SCORE: " + score);


                    #region РЕШЕНИЕ RECAPTHC VERSION 2.0

                    string requestUrl = null;
                    if (checkBXCapchaProxy.Checked)// by proxy
                    {
                        requestUrl =
                            string.Format(
                                "https://2captcha.com/in.php?key={0}&method=userrecaptcha&googlekey={1}&pageurl={2}", Key, _sitekey, url);


                    }
                    else
                    {
                        if (checkBoxCaptchaV3.Checked)
                        {
                            if (string.IsNullOrEmpty(action)) return;
                            if (string.IsNullOrEmpty(score)) return;

                            requestUrl =
                               string.Format(
                                   "https://2captcha.com/in.php?key={0}&method=userrecaptcha&version=v3&action={1}&min_score={2}&googlekey={3}&pageurl={4}&proxy=" + _currentProxy,
                                   Key, action, score, _sitekey, url);

                        }
                        else
                        {

                            requestUrl =
                                string.Format(
                                    "https://2captcha.com/in.php?key={0}&method=userrecaptcha&googlekey={1}&pageurl={2}",
                                    Key, _sitekey, url);
                        }

                    }

                    textBox1.Text = _currentProxy + "  " + requestUrl;


                    _idCaptch = null;
                    _isCaptchaOk = false;
                    _resultCaptch = null;

                    // получить ID 

                    #region ID CAPTCHA AND RESULT IT

                    ///*
                    int number = 0;
                    string captchID = null;

                    if (GetID(requestUrl, out captchID))
                    {
                        _idCaptch = captchID;
                        listBox1.Items.Add("    ID:" + _idCaptch);

                        // таймер для получения результата каптчи
                        TimerN(() =>
                        {

                            try
                            {

                                number++;

                                // Дать время работнику на решение каптчи
                                if (number >= 2)
                                {
                                    #region

                                    label2.Text = "Captcha " + number;

                                    string urlGet =
                                        "https://2captcha.com/res.php?key=" +
                                        Key +
                                        "&action=get&id=" +
                                        captchID;

                                    // taskinfo=1&json=1&

                                    WebRequest req = WebRequest.Create(urlGet);

                                    req.Proxy = null;

                                    string answer = null;
                                    using (WebResponse resp = req.GetResponse())
                                    {
                                        using (StreamReader answerStream = new StreamReader(resp.GetResponseStream()))
                                        {
                                            answer = answerStream.ReadToEnd();
                                        }
                                    }


                                    if (answer.Length < 3)
                                    {
                                        listBox1.Items.Add(
                                            "CAPTCHA длина меньше 3: " +
                                            answer);
                                    }
                                    else
                                    {
                                        if (answer.Substring(0, 3) ==
                                            "OK|")
                                        {

                                            listBox1.Items.Add("CAPTCHA: " + answer.ToUpper());

                                            _isCaptchaOk = true;

                                            _resultCaptch = answer.Remove(0, 3);

                                            listBox1.Items.Add(
                                                "OK| CAPTCHA id " +
                                                Thread.CurrentThread
                                                    .ManagedThreadId);

                                            return true;

                                        }
                                        else if (answer != "CAPCHA_NOT_READY")
                                        {

                                            /* ПЕРЕЗАГРУЗКА */
                                            listBox1.Items.Add("ERROR:CAPTCHA : " + answer);

                                            StatusCaptcha(CaptchaInfo.reportbad);

                                            return true;
                                        }

                                        listBox1.Items.Add(
                                            "[!!!ЭТО НЕ OK| CAPTCHA]  ..." +
                                            answer);

                                    }

                                    #endregion
                                }
                            }
                            catch (WebException exw)
                            {
                                File.AppendAllText("ERRORS\\errors.txt",
                                    "0)Ошибка: " + exw.GetBaseException().ToString() +
                                    "::" + DateTime.Now.ToShortTimeString() + "\r\n");
                                listBox1.Items.Add("E1:" + exw.Message);
                                return false;
                            }
                            catch (Exception ex)
                            {
                                File.AppendAllText("ERRORS\\errors.txt",
                                    "1)Ошибка: " + ex.Message +
                                    "::" + DateTime.Now.ToShortTimeString() + "\r\n");
                                listBox1.Items.Add("E2:" + ex.Message);
                                return true;
                            }


                            return false;
                        }, 10000);


                    }
                    else
                    {
                        listBox1.Items.Add(
                            "ID: Каптча не решена. Что то не так.  ");

                        #region ERROR ID

                        TimerN(() =>
                        {


                            if (_idCount >= 3)
                            {
                                File.AppendAllText("ERRORS\\errors.txt",
                                           "Ошибка: ID: Каптча не решена. Что то не так. ::" + DateTime.Now.ToShortTimeString() + "\r\n");

                                return true;
                            }

                            _idCount++;

                            Captcha(url, num);

                            return true;
                        }, 10000);


                        #endregion

                    }

                    // */

                    #endregion



                    #endregion


                }

                #endregion
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.GetBaseException().ToString());
            }
        }

        //  id для получения результата каптчи
        private bool GetID(string url, out string result)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                req.Proxy = null;

                using (WebResponse resp = req.GetResponse())
                using (StreamReader read = new StreamReader(resp.GetResponseStream()))
                {
                    string response = read.ReadToEnd();
                    listBox1.Items.Add("ID: " + response);
                    if (response.Length < 3)
                    {
                        result = response;
                        return false;
                    }
                    else
                    {
                        if (response.Substring(0, 3) == "OK|")
                        {
                            string captchaID = response.Remove(0, 3);

                            result = captchaID;

                            return true;
                        }
                    }
                }
            }
            catch (WebException exw)
            {
                File.AppendAllText("ERRORS\\errors.txt", "Ошибка: " + exw.GetBaseException() + "\r\n");
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString(), " GetID ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            result = null;
            return false;
        }

        #endregion


        #region anti-captcha.com


        private async Task<CaptchaResult> AntiCaptcha(string fileName, int delay)
        {
            try
            {
                _idCaptch = null;

                #region header

                string base64 = null;
                using (var image = Image.FromFile(fileName))
                {
                    using (var m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        var imageBytes = m.ToArray();

                        base64 = Convert.ToBase64String(imageBytes);
                    }
                }

                JObject jobj = new JObject();
                jobj.Add("clientKey", Key_anti_captcha);
                JObject jobj2 = new JObject();
                jobj2.Add("type", "ImageToTextTask");
                jobj2.Add("body", base64.Replace("\r", "").Replace("\n", ""));
                jobj2.Add("math", false);
                jobj2.Add("phrase", false);
                jobj2.Add("case", true);
                jobj2.Add("minLength", "6");
                jobj2.Add("maxLength", "6");
                jobj.Add("task", jobj2);
                jobj.Add("languagePool", "en");


                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback +=
                   (sender, certificate, chain, sslPolicyErrors) => true;


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.anti-captcha.com/createTask");
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = System.Net.CredentialCache.DefaultCredentials;
                request.Proxy = null;

                Stream stream = request.GetRequestStream();
                var bts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(jobj, Formatting.Indented));
                stream.Write(bts, 0, bts.Length);
                stream.Close();

                #endregion

                using (WebResponse response = request.GetResponse())
                {
                    Stream strm = response.GetResponseStream();
                    string inJson = null;
                    using (StreamReader read = new StreamReader(strm))
                    {
                        inJson = read.ReadToEnd();
                    }
                    strm.Close();

                    listBox1.Items.Add("Ответ :" + inJson);

                    var zin = JsonConvert.DeserializeObject<AntiCaptchaResponse>(inJson);

                    if (zin.errorId != 0)// если ответ от сервера не успешный
                    {
                        return new CaptchaResult(false, zin.taskId.ToString());
                    }

                    _idCaptch = zin.taskId.ToString();

                    await Task.Delay(delay * 1000);

                    return await AntiCaptchaAnswer(zin.taskId);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }

            Task<CaptchaResult> t = null;

            return await t;
        }

        [Serializable]
        private struct AntiCaptchaResponse
        {
            public int errorId;
            public int taskId;
        }


        private async Task<CaptchaResult> AntiCaptchaAnswer(int solveId)
        {
            int z = 0;

            WebClient wc = new WebClient();
            wc.Proxy = null;

            while (true)
            {
                byte[] resJson = null;

                try
                {
                    string url = string.Format("https://api.anti-captcha.com/getTaskResult");

                    JObject json = new JObject();
                    json.Add("clientKey", Key_anti_captcha);
                    json.Add("taskId", solveId);
                    var bts = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(json, Formatting.Indented));
                    resJson = await wc.UploadDataTaskAsync(url, "POST", bts);
                }
                catch (WebException wex)
                {
                    z++;
                    if (z >= 3)
                    {
                        return new CaptchaResult(false, "мы не получили ответ.");
                    }

                    continue;
                }


                dynamic res = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(resJson));

                if (res.errorId.ToString().Equals("0"))
                {
                    if (res.status.ToString().Equals("processing"))
                    {
                        await Task.Delay(5 * 1000);

                        listBox1.Items.Add("    Waiting... " + res.status);

                        continue;
                    }

                }
                else
                {
                    return new CaptchaResult(false, res.errorCode.ToString());
                }

                textBox1.Text = res.ToString();

                return new CaptchaResult(true, res.solution.text.ToString());
            }
        }





        #endregion

        private string StatusCaptcha(params CaptchaInfo[] actions)
        {
            try
            {
                Connect obj = new Connect();
                obj.IsProxy = false;
                obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                obj.IsKeepAlive = false;
                obj.IsRedirect = false;
                obj.IsDebbug = true;
                obj.IsExtendDebbug = true;

                obj.UserAgent = _ua;
                obj.Accept = "text/html, application/xhtml+xml, */*";


                string url = null;
                string ans = null;

                foreach (CaptchaInfo action in actions)
                {
                    switch (Enum.GetName(typeof(CaptchaInfo), action))
                    {
                        case "getbalance":

                            #region запросить текущий баланс

                            url =
                                string.Format(
                                    "https://2captcha.com/res.php?key={0}&action={1}", Key, action);

                            obj.URL = new Uri(url);
                            ans = obj.RequestGet(listBox1);

                            this.Text = Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName) +
                                    " Balance: " + ans + "$";
                            break;

                        #endregion

                        default:

                            label2.Text = "Отчет отправлен на 2captcha.com : " + action;

                            if (string.IsNullOrEmpty(_idCaptch))
                            {
                                listBox1.Items.Add("Info: Отчет не может быть отправлен, т.к. Id каптчи не был указан.");
                                break;
                            }


                            url =
                                string.Format(
                                    "https://2captcha.com/res.php?key={0}&action={1}&id={2}", Key, action, _idCaptch);


                            obj.URL = new Uri(url);
                            ans = obj.RequestGet(listBox1);

                            if (Enum.GetName(typeof(CaptchaInfo), action) == "get2")
                            {
                                listBox1.Items.Add("Cумма затраченная на последнию каптчу: " + ans + "$. id: " + _idCaptch);
                                File.AppendAllText("ERRORS/errorCaptcha.txt", "Cумма затраченная на последнию каптчу: " + ans + "$. id: " + _idCaptch + Environment.NewLine);
                            }
                            else
                            {
                                listBox1.Items.Add("Ошбка отравлена на 2captcha.com : " + ans + "$. id: " + _idCaptch);
                                File.AppendAllText("ERRORS/errorCaptcha.txt", ans + Environment.NewLine);

                                return ans;
                            }

                            break;
                    }

                }


            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }

            return null;
        }

        [Flags]
        private enum CaptchaInfo
        {
            NONE,
            reportbad,
            getbalance,
            get2, //стоимость решения отправленной капчи и ответ на нее

            ALL = reportbad | getbalance | get2
        }

        public class CaptchaResult
        {
            public bool Success;
            public string Response;

            public CaptchaResult(bool success, string response)
            {
                Success = success;
                Response = response;
            }
        }

        #region Captcha Image

        public async Task<CaptchaResult> UploadCaptchaFile(string fileName, int delay)
        {
            try
            {
                _idCaptch = null;

                NameValueCollection post = new NameValueCollection();
                post.Add("key", Key);
                post.Add("method", "post");
                post.Add("regsense", "1");
                post.Add("phrase", "0");
                post.Add("numeric", "4");
                post.Add("lang", "en");
                post.Add("min_len", "6");
                post.Add("max_len", "6");
                post.Add("json", "1");


                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback +=
                   (sender, certificate, chain, sslPolicyErrors) => true;


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://2captcha.com/in.php");
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.Method = "POST";
                request.KeepAlive = true;
                request.Credentials = System.Net.CredentialCache.DefaultCredentials;
                request.Proxy = null;

                Stream stream = request.GetRequestStream();

                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                foreach (string key in post.Keys)
                {
                    stream.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", key, post[key]);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    stream.Write(formitembytes, 0, formitembytes.Length);
                }
                stream.Write(boundarybytes, 0, boundarybytes.Length);

                string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", "file", fileName, "image/jpeg");
                byte[] headerbytes = Encoding.UTF8.GetBytes(header);
                stream.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                stream.Write(trailer, 0, trailer.Length);
                stream.Close();

                using (WebResponse response = request.GetResponse())
                {
                    Stream rStream = response.GetResponseStream();
                    string inJson = null;
                    using (StreamReader read = new StreamReader(rStream))
                    {
                        inJson = read.ReadToEnd();
                    }

                    listBox1.Items.Add("Ответ от in.php:" + inJson);

                    TwoCaptchaResponse zin = JsonConvert.DeserializeObject<TwoCaptchaResponse>(inJson);

                    if (zin.Status == 0)
                    {
                        return new CaptchaResult(false, zin.Request);
                    }

                    _idCaptch = zin.Request;

                    await Task.Delay(delay * 1000);

                    return await GetResponse(zin.Request);

                }
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }

            Task<CaptchaResult> t = null;

            return await t;
        }



        private async Task<CaptchaResult> GetResponse(string solveId)
        {
            int z = 0;

            WebClient wc = new WebClient();
            wc.Proxy = null;

            while (true)
            {
                byte[] resJson = null;

                try
                {
                    string url = string.Format("https://2captcha.com/res.php?key={0}&id={1}&action=get&json=1", Key,
                        solveId);

                    textBox1.Text = url;

                    resJson = await wc.DownloadDataTaskAsync(url);
                }
                catch (WebException wex)
                {
                    z++;
                    if (z >= 3)
                    {
                        return new CaptchaResult(false, "мы не получили ответ.");
                    }

                    continue;
                }


                var res = JsonConvert.DeserializeObject<TwoCaptchaResponse>(Encoding.UTF8.GetString(resJson));

                if (res.Status == 0)
                {
                    if (res.Request == "CAPCHA_NOT_READY")
                    {
                        await Task.Delay(5 * 1000);

                        listBox1.Items.Add("    Waiting... " + res.Request);

                        continue;
                    }

                    return new CaptchaResult(false, res.Request);

                }

                return new CaptchaResult(true, res.Request);
            }
        }



        [Serializable]
        private struct TwoCaptchaResponse
        {
            public int Status;
            public string Request;
        }

        #endregion



        #endregion




        #region PROXY


        private bool SetProxy()
        {
            try
            {
                // указываем прокси
                if (!File.Exists(_proxyFile))
                {
                    Task.Factory.StartNew(() =>
                    {
                        MessageBox.Show(
                            "У вас отсутствует файл прокси. Нажмите кнопку Get Proxy и перезапустите программу. ID thread:" + Thread.CurrentThread.ManagedThreadId,
                            "Note",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });

                    return false;
                }

                string[] massProxy = File.ReadAllLines(_proxyFile);


                if (massProxy.Length == 0)
                {
                    Task.Factory.StartNew(() =>
                    {
                        MessageBox.Show(
                            "У вас закончились прокси. Нажмите кнопку Get Proxy и перезапустите программу. ID thread:" + Thread.CurrentThread.ManagedThreadId, "Note",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });

                    return false;
                }

                // взять случайный ip
                _currentProxy = massProxy[new Random().Next(massProxy.Length)].Trim();

                while (string.IsNullOrEmpty(_currentProxy)) // если пустая строчка
                {
                    listBox1.Items.Add("_currentProxy = null " + _currentProxy);
                    _currentProxy = massProxy[new Random().Next(massProxy.Length)].Trim();
                }


                var arr = _currentProxy.Split(new string[] { ":", ";" }, StringSplitOptions.RemoveEmptyEntries);


                _currentProxy = arr[0] + ":" + arr[1];
                _anonymity = null;
                if (arr.Length == 4)
                {
                    if (checkBoxProxyPass.Checked)
                    {
                        _proxyLogin = arr[2].Trim();
                        _proxyPass = arr[3].Trim();

                        if (_proxyLogin == null) return false;
                        if (_proxyPass == null) return false;

                        listBox1.Items.Add("HTTP:" + _currentProxy + " :" + _proxyLogin + ":" + _proxyPass);

                        return true;
                    }
                    else
                    {
                        _typeProxy = arr[2];

                        if (_typeProxy.Contains(","))
                        {
                            _typeProxy =
                                _typeProxy.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                        }
                        _anonymity = arr[3];

                        listBox1.Items.Add("HTTP:" + _currentProxy + ":Type:" + _typeProxy + ":Priority: " + _anonymity);

                        if (!string.IsNullOrEmpty(_typeProxy))
                        {
                            listBox1.Items.Add("TYPE PROXY ." + _typeProxy);
                            if (_typeProxy.Contains("SOCKS"))
                            {
                                listBox1.Items.Add("=========================SOCKS PROXY DELETED.");
                                buttonDeleteProxy.PerformClick();

                                SetProxy(); return true;
                            }

                        }



                    }
                }
                else
                {
                    listBox1.Items.Add("HTTP:" + _currentProxy);
                }

                return true;
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return false;
        }


        private WebBrowserDocumentCompletedEventHandler _hnd;
        private WebBrowserDocumentCompletedEventHandler _hnd2;
        private WebBrowserDocumentCompletedEventHandler _hnd3;
        private void buttonProxy_Click(object sender, EventArgs e)
        {
            try
            {
                if (!checkBoxTab2All.Checked)
                {
                    if (File.Exists(_proxyFile))
                    {
                        if (!checkBoxTab2Proxy.Checked) File.Delete(_proxyFile);
                    }
                }

                if (string.IsNullOrEmpty(textBoxProxyPath.Text)) return;

                _isPagination = false;
                _isReady = false;

                WebBrowser br = webBrowser1;
                br.ScriptErrorsSuppressed = true;

                if (radioButtonHidemy.Checked)
                {
                    _hnd = new WebBrowserDocumentCompletedEventHandler(Compl);
                    br.DocumentCompleted += _hnd;
                    br.Navigate(textBoxProxyPath.Text);
                }

                if (radioButtonProxynova.Checked)
                {
                    _hnd2 = new WebBrowserDocumentCompletedEventHandler(Compl2);
                    br.DocumentCompleted += _hnd2;
                    br.Navigate(textBoxProxyPath.Text);
                }

                if (radioButtonFreeproxy.Checked)
                {
                    Task.Factory.StartNew(() => Compl3(textBoxProxyPath.Text.Trim()));
                }

                if (radioButtonProxyservers.Checked)
                {
                    Task.Factory.StartNew(() => Compl4(textBoxProxyPath.Text.Trim()));
                }
            }
            catch (Exception eg)
            {
                MessageBox.Show(eg.GetBaseException().ToString());
            }
        }



        private bool _isPagination;
        private bool _isReady;
        private void Compl(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                WebBrowser br = sender as WebBrowser;
                if (!br.IsBusy & br.ReadyState == WebBrowserReadyState.Complete)
                {

                    if (!_isReady)
                    {
                        _isReady = true;

                        TimerN(() =>
                        {
                            listBox1.Items.Add("    TIMER 1 GO");

                            var h2 = br.Document.All;
                            foreach (HtmlElement el in h2)
                            {
                                //listBox1.Items.Add("<H2>"+el.InnerText);
                                if (string.IsNullOrEmpty(el.InnerText)) continue;
                                if (el.InnerText.Contains("Онлайн-база прокси-листов"))
                                {
                                    // File.AppendAllText("hidemy.html", br.DocumentText);
                                    listBox1.Items.Add("R E D Y");

                                    #region Спарсить прокси и сохранить в файл

                                    var table = br.Document.GetElementsByTagName("table")[0];

                                    HtmlElementCollection tr = table.GetElementsByTagName("TR");

                                    foreach (HtmlElement elem in tr)
                                    {
                                        if (Regex.IsMatch(elem.InnerText, "\\d{1,3}\\."))
                                        {

                                            string ip = elem.Children[0].InnerText.Trim();
                                            string port = elem.Children[1].InnerText.Trim();
                                            string type = elem.Children[4].InnerText.Trim();
                                            string anon = elem.Children[5].InnerText.Trim();

                                            listBox1.Items.Add("Address: " + ip + ":: " + port + "::" + type + "::" + anon);
                                            //записать в файл
                                            File.AppendAllText(_proxyFile,
                                                ip + ":" + port + ":" + type + ":" + anon + "\r\n");


                                        }
                                    }


                                    #endregion



                                    #region 2 page

                                    if (!_isPagination)
                                    {
                                        listBox1.Items.Add("P A G I N A T I O N");


                                        HtmlElement divs = GetElem(br, "div", "pagination");

                                        listBox1.Items.Add("D I V S " + divs.FirstChild.Children.Count);

                                        foreach (HtmlElement elm in divs.FirstChild.Children)
                                        {
                                            if (elm.InnerText != null)
                                            {

                                                if (elm.InnerText == "2")
                                                {
                                                    _isPagination = true;
                                                    _isReady = false;

                                                    listBox1.Items.Add("Pagination " + elm.TagName + " :: " +
                                                                       elm.InnerText);

                                                    listBox1.Items.Add("URL " + elm.FirstChild.GetAttribute("href"));

                                                    br.DocumentCompleted -= _hnd;
                                                    br.DocumentCompleted += _hnd;
                                                    br.Navigate(elm.FirstChild.GetAttribute("href"));


                                                    break;
                                                }
                                            }
                                        }

                                        br = null;
                                        if (br != null) br.Dispose();


                                        return true;
                                    }

                                    #endregion


                                    return true;
                                }
                            }
                            listBox1.Items.Add("waiting...");

                            return false;
                        }, 2000);



                        // br.DocumentCompleted -= _hnd;
                    }
                }
            }
            catch (Exception eg)
            {
                MessageBox.Show(eg.GetBaseException().ToString());
            }
        }


        private void Compl2(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                WebBrowser br = sender as WebBrowser;
                if (!br.IsBusy & br.ReadyState == WebBrowserReadyState.Complete)
                {

                    if (!_isReady)
                    {
                        _isReady = true;

                        if (br.DocumentTitle.Contains("Proxy"))
                        {
                            // File.AppendAllText("hidemy.html", br.DocumentText);
                            listBox1.Items.Add("R E D Y " + br.DocumentTitle);

                            #region Спарсить прокси и сохранить в файл

                            var table = br.Document.GetElementsByTagName("table")[0];

                            HtmlElementCollection tr = table.GetElementsByTagName("TR");

                            foreach (HtmlElement elem in tr)
                            {
                                if (string.IsNullOrEmpty(elem.InnerText)) continue;
                                if (Regex.IsMatch(elem.InnerText, "\\d{1,3}\\."))
                                {

                                    string ip = elem.Children[0].InnerText.Trim();
                                    ip = Regex.Replace(ip,
                                        "document.write\\(.*?\\);", string.Empty);
                                    string port = elem.Children[1].InnerText.Trim();
                                    string type = "No";
                                    string anon = elem.Children[6].InnerText.Trim();

                                    listBox1.Items.Add("Address: " + ip + ":: " + port + "::" + type + "::" + anon);
                                    //записать в файл
                                    File.AppendAllText(_proxyFile,
                                        ip + ":" + port + ":" + type + ":" + anon + "\r\n");

                                }
                            }

                            #endregion


                        }

                    }
                }
            }
            catch (Exception eg)
            {
                MessageBox.Show(eg.GetBaseException().ToString());
            }
        }


        private void Compl3(string url)
        {
            try
            {

                Connect obj = new Connect(url);
                obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                if (checkBoxTab2Proxy.Checked)
                {
                    obj.IsProxy = true;
                    obj.IpPortOfProxy = "190.112.193.209:1212";

                    if (checkBoxProxyPass.Checked)
                    {
                        obj.IsByPass = true;
                        obj.ProxyLogin = "user-54775";
                        obj.ProxyPass = "berukasy";
                    }
                }

                obj.IsDebbug = true;
                obj.IsExtendDebbug = true;

                obj.IsKeepAlive = true;
                obj.IsRedirect = false;
                obj.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0";
                obj.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                NameValueCollection coll = new NameValueCollection();
                coll.Add("DNT", "1");
                coll.Add("Upgrade-Insecure-Requests", "1");
                obj.NameValColl = coll;

                string ans = null;
                ans = obj.RequestGet(listBox1);

                listBox1.Items.Add(" ");

                if (string.IsNullOrEmpty(ans)) return;

                label2.BackColor = Color.Yellow;
                if (File.Exists(_proxyFile)) File.Delete(_proxyFile);

                //paginat
                string div = Regex.Match(ans, "<div class=\"paginator\">(.*?)</div>").Groups[1].Value;

                var hrefAll = Regex.Matches(div, "href=\"(.*?)\">\\d{1,2}</a>");

                foreach (Match m in hrefAll)
                {
                    listBox1.Items.Add("URL: " + m.Value);
                }

                listBox1.Items.Add(" ");

                SaveIp(ans);



                foreach (Match z in hrefAll)
                {
                    string ur = "http://free-proxy.cz" + z.Groups[1].Value.Trim();

                    obj.URL = new Uri(ur);

                    ans = null;
                    ans = obj.RequestGet(listBox1);

                    listBox1.Items.Add(" ");

                    SaveIp(ans);

                }
                if (File.Exists("example.html")) File.Delete("example.html");
                File.AppendAllText("example.html", ans + Environment.NewLine);
            }
            catch (WebException we)
            {
                label2.BackColor = Color.FromArgb(new Random().Next(256), new Random().Next(256), new Random().Next(50, 256));
                buttonDeleteProxy.PerformClick();
                listBox1.Items.Add(we.Message);
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
        }

        private void SaveIp(string ans)
        {

            #region ip port

            var tr = Regex.Matches(ans, "<tr>(.*?)</tr>");

            foreach (Match m in tr)
            {
                if (m.Value.Contains("Base64.decode"))
                {
                    string base64 = Regex.Match(m.Value, "Base64\\.decode\\(\"(.*?)\"\\)").Groups[1].Value;

                    byte[] bt = Convert.FromBase64String(base64);

                    string ip = Encoding.UTF8.GetString(bt).Trim();

                    string port =
                        Regex.Match(m.Value, "<span class=\"fport\" style=\''>(.*?)</span>").Groups[1].Value.Trim();


                    string type =
                        Regex.Match(m.Value, "<td><small>(.*?)</small></td>").Groups[1].Value.Trim();


                    string anon =
                        Regex.Matches(m.Value, "<td class=\"small\"><small>(.*?)</small></td>")[2].Groups[1].Value.Trim();

                    listBox1.Items.Add("Proxy : " + ip + ":" + port + ":" + type + ":" + anon);

                    //записать в файл
                    File.AppendAllText(_proxyFile,
                        ip + ":" + port + ":" + type + ":" + anon + "\r\n");

                }
            }

            #endregion

        }


        private void Compl4(string url)
        {
            try
            {
                Connect obj = new Connect(url);
                obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                obj.IsDebbug = true;
                obj.IsExtendDebbug = true;

                obj.IsKeepAlive = true;
                obj.IsRedirect = false;
                obj.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0";
                obj.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                NameValueCollection coll = new NameValueCollection();
                coll.Add("Accept-Encoding", "gzip, deflate");
                obj.NameValColl = coll;

                obj.IpPortOfProxy = "165.231.95.34:12345";
                obj.IsByPass = true;
                obj.ProxyLogin = "shantell";
                obj.ProxyPass = "1rambo1";

                string ans = null;
                ans = obj.RequestGet(listBox1);

                listBox1.Items.Add(" ");

                if (string.IsNullOrEmpty(ans)) return;

                //paginat
                string div = Regex.Match(ans, "<ul class=\"pagination\">(.*?)</ul>").Groups[1].Value;

                var hrefAll = Regex.Matches(div, "<a href=\"(.*?)\">\\d{1,2}</a>");

                foreach (Match m in hrefAll)
                {
                    listBox1.Items.Add("URL: " + m.Value);
                }

                listBox1.Items.Add(" ");

                SaveIp2(ans);



                if (File.Exists("example2.html")) File.Delete("example2.html");
                File.AppendAllText("example2.html", ans + Environment.NewLine);
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
        }

        private void SaveIp2(string ans)
        {
            try
            {
                #region ip port

                ans = ans.Replace("\r", string.Empty).Replace("\n", string.Empty);

                var tr = Regex.Matches(ans, "<tr valign=\"top\">(.*?)</tr>");

                listBox1.Items.Add("Count: " + tr.Count);

                foreach (Match m in tr)
                {
                    if (Regex.IsMatch(m.Value, "\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}"))
                    {
                        string ip = Regex.Match(m.Value, "\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}").Value;

                        string hex =
                            Regex.Match(m.Value, "<span class=\"port\" data-port=\"(.*?)\">").Groups[1].Value.Trim();

                        string chash = Regex.Match(ans, "var chash = '(.*?)';").Groups[1].Value.Trim();

                        string port = Decode(hex, chash);

                        var typeAnon = Regex.Matches(m.Value, "<td>(\\w[^\\d]{3,10})</td>");
                        string type = null; string anon = null;
                        if (typeAnon.Count == 2)
                        {
                            type = typeAnon[0].Groups[1].Value;
                            anon = typeAnon[1].Groups[1].Value;
                        }
                        listBox1.Items.Add("Proxy : " + ip + ":" + port + ":" + type + ":" + anon);

                        //записать в файл
                        File.AppendAllText(_proxyFile,
                            ip + ":" + port + ":" + type + ":" + anon + "\r\n");

                    }
                }

                #endregion
            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }

        private string Decode(string word, string sum)
        {
            try
            {
                //listBox1.Items.Add(word);
                // listBox1.Items.Add(sum);

                List<int> a = new List<int>();

                int n = 0;

                for (int o = 0; o < word.Length - 1; o += 2, n++)
                {
                    string hex = word.Substring(o, 2);

                    int num = Convert.ToInt32(hex, 16);

                    a.Add(num);

                    //listBox1.Items.Add(":hex: "+hex+":int:"+num);
                }

                List<int> r = new List<int>();

                //for (o = 0; o < e.length; o++) r[o] = e.charCodeAt(o);

                for (int i = 0; i < sum.Length; i++)
                {
                    r.Add((int)sum[i]);

                    //listBox1.Items.Add(i + ")::" + (int)sum[i]);
                }

                // for (o = 0; o < a.length; o++) a[o] = a[o] ^ r[o % r.length];

                for (int i = 0; i < a.Count; i++)// 0 of 4
                {
                    var res = a[i];

                    var middle = i % r.Count;// 0/32
                    var res2 = r[middle];

                    //  listBox1.Items.Add("res:"+res + "== "+i+"/"+r.Count+"="+middle+":res2 ="+res2+":a count "+a.Count+":r count "+r.Count);

                    var all = res ^ res2;

                    a[i] = all;
                    // listBox1.Items.Add(all);
                }

                //listBox1.Items.Add("");
                //for (o = 0; o < a.length; o++) a[o] = String.fromCharCode(a[o]);
                string ans = null;
                for (int i = 0; i < a.Count; i++)
                {
                    var ch = Convert.ToChar(a[i]);
                    ans += ch;
                }


                //listBox1.Items.Add(ans);
                return ans;
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return null;
        }


        private void radioButtonHidemy_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProxyPath.Text = "https://hidemy.name/ru/proxy-list/?country=US&maxtime=1000&type=hs&anon=1234#list";
        }

        private void radioButtonProxynova_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProxyPath.Text = "https://www.proxynova.com/proxy-server-list/country-us/";
        }

        private void radioButtonFreeproxy_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProxyPath.Text = "http://free-proxy.cz/en/proxylist/country/US/https/ping/all";
        }

        private void radioButtonProxyservers_CheckedChanged(object sender, EventArgs e)
        {
            textBoxProxyPath.Text = "https://de.proxyservers.pro/proxy/list/country/DE/order/updated/order_dir/desc/page/1";
        }





        private HtmlElement GetElem(WebBrowser br, string tag, string classNm)
        {
            var tags = br.Document.GetElementsByTagName(tag);

            foreach (HtmlElement elem in tags)
            {
                if (elem.GetAttribute("className") == classNm)
                {
                    return elem;
                }
            }
            return null;

        }

        private void buttonDeleteProxy_Click(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                try
                {
                    // УДАЛИТЬ ПРОКСИ НЕВАЛИД
                    if (!string.IsNullOrEmpty(_proxyFile))
                    {
                        listBox1.Items.Add("Прокси удален " + _currentProxy);
                        OverwriteProxy(_proxyFile);
                    }
                    _currentProxy = null;
                    _proxyLogin = null;
                    _proxyPass = null;
                }
                catch (Exception en)
                {
                    MessageBox.Show(en.GetBaseException().ToString());
                }
            }));
        }



        private void saveAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Save();
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
        }

        #endregion


        #region SAVE / STOP / ПЕРЕЗАПИСЬ ФАЙЛА С КЛЮЧАМИ ДОМЕННЫХ ИМЕН


        private void ClearErrorsDirectory()
        {
            // clear ERRORS directory
            string[] files = Directory.GetFiles(Environment.CurrentDirectory + "/ERRORS");
            foreach (string f in files)
            {
                File.Delete(f);
            }
        }


        // SAVE ACCAUNT'S DATA
        private string _html;
        private void Save()
        {
            try
            {


            }
            catch (Exception en)
            {
                MessageBox.Show(en.GetBaseException().ToString());
            }
        }



        private void RemSingleName(string name, string path, int z = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }

                if (!File.Exists(path))
                {
                    throw new Exception("Path of email is not exist. Your error path: " + path);
                    return;
                }

                string[] trm = File.ReadAllLines(path);

                using (StreamWriter read = new StreamWriter(path))
                {
                    for (int i = 0; i < trm.Length; i++)
                    {
                        var s = trm[i].Trim(); // имя из файла

                        if (s.Contains(name))
                        {
                            label3.BackColor = Color.GreenYellow;
                            label3.Text = ((z > 0) ? z + ") " + name : name);

                            continue;
                        }

                        if (i == trm.Length - 1)
                        {
                            read.Write(s);
                            break;
                        }

                        // записать в файл
                        read.Write(s + "\r\n");
                    }

                }


            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }



        // перезаписать файл с прокси, если текущий прокси не валидный
        private void OverwriteProxy(string file)
        {
            try
            {

                // удалить прокси из файла
                if (!File.Exists(file)) return;
                if (_currentProxy == null) return;

                var mass = File.ReadAllLines(file);

                if (mass.Length == 0)
                {
                    listBox1.Items.Add("You have not proxy.");
                    return;
                }

                using (StreamWriter wr = new StreamWriter(file))
                {

                    for (int i = 0; i < mass.Length; i++)
                    {
                        string pz = mass[i];

                        if (pz.Trim().Contains(_currentProxy.Trim()))
                        {
                            continue;
                        }
                        else
                        {

                            if (i == mass.Length - 1)
                            {
                                wr.Write(pz);
                            }
                            else
                            {
                                wr.Write(pz + "\r\n");
                            }
                        }


                    }
                }


            }
            catch (Exception en)
            {
                MessageBox.Show(en.GetBaseException().ToString());
            }
        }

        private void DirSearch(string searchRootDirectory, string searchFileName)
        {
            try
            {
                foreach (string directory in Directory.GetDirectories(searchRootDirectory))
                {
                    foreach (string file in Directory.GetFiles(directory, searchFileName))
                    {
                        listBox1.Items.Add("file: " + file);
                    }
                    DirSearch(directory, searchFileName);
                }
            }
            catch (System.Exception excpt)
            {
                listBox1.Items.Add("error: " + excpt.Message);
            }
        }

        private void RemoveDirsAndFiles(string catalog)
        {
            try
            {
                if (!Directory.Exists(catalog))
                {
                    listBox1.Items.Add("Catalog not exist");
                    return;
                }

                foreach (string file in Directory.GetFiles(catalog))
                {
                    File.Delete(file);
                }

                foreach (string directory in Directory.GetDirectories(catalog))
                {
                    Directory.Delete(directory, true);
                }
            }
            catch (System.Exception excpt)
            {
                listBox1.Items.Add("Error: RemoveDirsAndFiles: " + excpt.GetBaseException());
            }
        }

        // этот метод для работы с формой
        private StringBuilder PostRequest(string ans, out HtmlNode form, string login = null, string pass = null)
        {
            form = null;
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(ans);
                var root = doc.DocumentNode;
                form = root.SelectSingleNode("//form");
                if (form == null) return null;
                var inputs = form.Descendants("input");
                var select = form.Descendants("select");
                var merge = inputs.Concat(select);
                StringBuilder param = new StringBuilder();

                foreach (HtmlNode input in merge)
                {
                    listBox1.Items.Add(input.OuterHtml);

                    string nameA = input.GetAttributeValue("name", null);
                    string valueB = input.GetAttributeValue("value", null);
                    string typeC = input.GetAttributeValue("type", null);
                    if (string.IsNullOrEmpty(nameA)) continue;
                    if (string.IsNullOrEmpty(valueB)) valueB = string.Empty;

                    if (nameA.Equals("Name")) valueB = login;
                    if (nameA.Equals("Login"))
                    {
                        if (string.IsNullOrEmpty(valueB)) valueB = login;
                    }
                    if (nameA.Equals("Password")) valueB = pass;

                    if (typeC != null)
                    {
                        if (typeC == "password") valueB = pass;
                    }

                    param.AppendFormat("{0}={1}&", Uri.EscapeDataString(nameA), Uri.EscapeDataString(valueB));

                }

                return param;
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return null;
        }

        private void GetUrlFromPage(string ans, ref string url, string tagName, string contain = null, bool isInnerTxt = false)
        {
            try
            {
                url = null;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(ans);
                var root = doc.DocumentNode;
                var nodes = root.Descendants(tagName);
                if (nodes != null & nodes.Count() != 0)
                {
                    foreach (HtmlNode nd in nodes)
                    {
                        url = nd.GetAttributeValue("action", null);
                        if (url == null) url = nd.GetAttributeValue("src", null);
                        if (url == null) url = nd.GetAttributeValue("href", null);
                        if (url == null) url = nd.GetAttributeValue("onClick", null);

                        if (url == null) continue;

                        if (!string.IsNullOrEmpty(contain))
                        {
                            if (!isInnerTxt) // ищем внутри содержимого атрибута
                            {
                                if (url.Contains(contain))
                                {
                                    break;
                                }
                            }
                            else// поиск ведем в тексте тега
                            {
                                //listBox1.Items.Add("text: "+nd.InnerText);
                                if (nd.InnerText.Contains(contain))
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
        }

        private List<string> GetUrlsFromPage(string ans, string tagName, string contain = null, bool isInnerTxt = false)
        {
            try
            {
                List<string> list = new List<string>();
                string url = null;

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(ans);
                var root = doc.DocumentNode;
                var nodes = root.Descendants(tagName);
                if (nodes != null & nodes.Count() != 0)
                {
                    foreach (HtmlNode nd in nodes)
                    {
                        url = nd.GetAttributeValue("action", null);
                        if (url == null) url = nd.GetAttributeValue("src", null);
                        if (url == null) url = nd.GetAttributeValue("href", null);
                        if (url == null) url = nd.GetAttributeValue("onClick", null);

                        if (url == null) continue;

                        if (!string.IsNullOrEmpty(contain))
                        {
                            if (!isInnerTxt) // ищем внутри содержимого атрибута
                            {
                                if (url.Contains(contain))
                                {
                                    list.Add(url);
                                }
                            }
                            else// поиск ведем в тексте тега
                            {
                                //listBox1.Items.Add("text: "+nd.InnerText);
                                if (nd.InnerText.Contains(contain))
                                {
                                    list.Add(url);
                                }
                            }
                        }
                        else
                        {
                            list.Add(url);
                        }
                    }
                }

                return ((list.Count == 0) ? null : list);

            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return null;
        }

        private string GetErrorFromPage(string ans, string tagName, string attributeName, string contain)
        {
            try
            {

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(ans);
                var root = doc.DocumentNode;
                var nodes = root.Descendants(tagName);
                if (nodes != null & nodes.Count() != 0)
                {
                    foreach (HtmlNode nd in nodes)
                    {
                        var attr = nd.GetAttributeValue(attributeName, null);
                        if (attr == null) continue;
                        if (attr.Equals(contain))
                        {
                            if (nd.GetAttributeValue("style", null) == null)
                            {
                                return nd.InnerText;
                            }
                        }


                    }
                }


            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return null;
        }

        private string GetInnerText(string ans, string tagName, string attributeName, string contain)
        {
            try
            {
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(ans);
                var root = doc.DocumentNode;
                var nodes = root.Descendants(tagName);
                if (nodes != null & nodes.Count() != 0)
                {
                    foreach (HtmlNode nd in nodes)
                    {
                        var attr = nd.GetAttributeValue(attributeName, null);
                        if (attr == null) continue;
                        if (attr.Equals(contain))
                        {
                            return nd.InnerText;

                        }


                    }
                }

            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return null;
        }



        private
        List<System.Windows.Forms.Timer> _timers = new List<System.Windows.Forms.Timer>();

        private void TimerN(_delegats function, int time, string tag = null)
        {
            try
            {
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Tag = tag;
                EventHandler Some = new EventHandler((o, e) =>
                {
                    try
                    {
                        bool result = function();

                        if (result)
                        {

                            timer.Stop();
                            timer.Dispose();
                        }
                    }
                    catch (Exception ef)
                    {
                        MessageBox.Show(ef.GetBaseException().ToString());
                    }

                });
                // добавить в список, чтобы потом его можно было остановить
                _timers.Add(timer);
                timer.Tick += Some;
                timer.Interval = time;
                timer.Start();
            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }

        }

        private void StopTimersN()
        {
            try
            {
                if (_timers.Count != 0)
                {
                    for (int i = 0; i < _timers.Count; i++)
                    {
                        System.Windows.Forms.Timer tm = _timers[i];

                        tm.Enabled = false;
                        tm.Stop();
                        tm.Dispose();
                    }

                    _timers.Clear();
                }
            }
            catch (Exception ea)
            {
                MessageBox.Show("StopTimersN(): " + ea.GetBaseException());
            }
        }


        private void radioButtonMymeriva_CheckedChanged(object sender, EventArgs e)
        {
            label6.Visible = true;
            label7.Visible = true;
            comboBoxIpServer.Visible = true;
            numericUpDown1.Visible = true;

            label8.Visible = false;
            label9.Visible = false;
            checkBoxDelDefault_asp.Visible = false;
            comboBoxOperatingSystem.Visible = false;
            comboBoxASPNetVersion.Visible = false;
        }

        private void radioButtonSomee_CheckedChanged(object sender, EventArgs e)
        {
            label6.Visible = false;
            label7.Visible = false;
            comboBoxIpServer.Visible = false;
            numericUpDown1.Visible = false;

            label8.Visible = true;
            label9.Visible = true;
            checkBoxDelDefault_asp.Visible = true;
            comboBoxOperatingSystem.Visible = true;
            comboBoxASPNetVersion.Visible = true;
        }

        private void comboBoxOperatingSystem_DropDown(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            Graphics g = combo.CreateGraphics();
            float longest = 0;
            for (int i = 0; i < combo.Items.Count; i++)
            {
                SizeF textLength = g.MeasureString(combo.Items[i].ToString(), combo.Font);

                if (textLength.Width > longest)
                    longest = textLength.Width;
            }

            if (longest > 0)
                combo.DropDownWidth = (int)longest;
        }


        private void checkBoxTempMail_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTempMail.Checked)
            {
                comboBoxEmail.Visible = false;
                label5.Visible = false;
                checkBoxTempMailSelfDmns.Visible = true;
            }
            else
            {
                comboBoxEmail.Visible = true;
                label5.Visible = true;
                checkBoxTempMailSelfDmns.Visible = false;
            }

        }


        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", Environment.CurrentDirectory);
        }

        private void clearAllDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            #region clear all data

            _userName = null;
            _linkTo = null;
            RemoveMail(_currentMail);
            _currentMail = null;

            if (checkBxPROXY.Checked)
            {
                buttonDeleteProxy.PerformClick();
            }



            #endregion


        }


        private void GetInfoOpenProperties(Uri ur)
        {
            Type t = ur.GetType();

            var props = t.GetProperties();

            foreach (PropertyInfo info in props)
            {
                object val = info.GetValue(ur);

                listBox1.Items.Add(info.Name + " : " + val + " : " + info.PropertyType);

                if (info.PropertyType == typeof(System.String[]))
                {
                    string[] arr = (string[])val;

                    foreach (var s in arr)
                    {
                        listBox1.Items.Add("\t" + s);
                    }

                }


            }
        }

        private int _countWeb;
        private delegate bool RunDelegate();
        private async Task<bool> WebRepeatIfError(RunDelegate run)
        {
            bool r = false;

            try
            {
                try
                {
                    r = run();

                    return r;
                }
                catch (WebException)
                {
                    r = false;
                }


                if (!r)
                {
                    _countWeb++;

                    if (_countWeb >= 4)
                    {
                        _countWeb = 0;
                        return false;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(15));
                    return await WebRepeatIfError(run);

                }

            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }

            return true;
        }



        #endregion


        private void SaveAllCookie(CookieContainer container, List<string> log)
        {

            #region Get all cookie

            Type t = container.GetType();

            Hashtable table = (Hashtable)t.InvokeMember("m_domainTable",
                BindingFlags.NonPublic |
                BindingFlags.GetField |
                BindingFlags.Instance,
                null,
                container,
                null);



            foreach (var tableKey in table.Keys)
            {
                String str_tableKey = (string)tableKey;

                if (str_tableKey[0] == '.')
                {
                    str_tableKey = str_tableKey.Substring(1);
                }

                SortedList list = (SortedList)table[tableKey].GetType().InvokeMember("m_list",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            table[tableKey],
                                                                            new object[] { });

                foreach (var listKey in list.Keys)
                {
                    String url = "https://" + str_tableKey + (string)listKey;
                    File.AppendAllText("ERRORS/cookie.txt", url + Environment.NewLine);
                }
            }

            File.AppendAllText("ERRORS/cookie.txt", Environment.NewLine);

            #endregion

        }

        private async void InitializeChrome()
        {
            try
            {
                /*

                if (_isFirstRunChrome)
                {
                    RemoveDirsAndFiles(Environment.CurrentDirectory + "\\myCache");

                    CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

                    var settings = new CefSettings()
                    {
                        CachePath = Environment.CurrentDirectory + "\\myCache",
                        UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.182 Safari/537.36"
                    };

                    settings.CefCommandLineArgs.Add("disable-web-security", "disable-web-security");
                    settings.CefCommandLineArgs.Add("window-size", "1920x1080");
                    settings.CefCommandLineArgs.Add("excludeSwitches", "enable-automation");
                    settings.CefCommandLineArgs.Add("no-proxy-server");

                    //settings.SetOffScreenRenderingBestPerformanceArgs();



                    Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

                    _isFirstRunChrome = false;


                    var brs = new BrowserSettings();

                    var rcs = new RequestContextSettings
                    {
                        CachePath = Environment.CurrentDirectory + "\\myCache",

                    };
                    var context = new RequestContext(rcs);


                    _chrome = new ChromiumWebBrowser("https://intoli.com/blog/making-chrome-headless-undetectable/chrome-headless-test.html", brs, context);
                    //_chrome = new ChromiumWebBrowser("https://temp-mail.org/ru/", brs, context);
                    Cef.UIThreadTaskFactory.StartNew(delegate
                    {
                        string error;
                        _chrome.RequestContext.SetPreference("enable_do_not_track", true, out error);

                    });

                    Cef.UIThreadTaskFactory.StartNew(delegate
                    {
                        var preferences = context.GetAllPreferences(true);

                        //Check do not track status
                        var doNotTrack = (bool)preferences["enable_do_not_track"];

                        listBox1.Items.Add("doNotTrack: " + doNotTrack);

                        foreach (KeyValuePair<string, object> pair in preferences)
                        {
                            // listBox1.Items.Add(pair.Key + " = " + pair.Value);
                        }
                    });


                    _chrome.FrameLoadStart += (s, argsi) =>
                    {
                        var b = (ChromiumWebBrowser)s;
                        if (argsi.Frame.IsMain)
                        {
                            b.SetZoomLevel(-2.0);

                        }
                    };


                }
                else
                {
                    if (!_chrome.IsBrowserInitialized) return;

                    Cef.GetGlobalCookieManager().DeleteCookies("", "");

                    _chrome.Load("https://temp-mail.org/ru/");

                }
                 */
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }

        }

        private async void InitializeSeleniumWebDriver()
        {
            try
            {
                ChromeOptions opt = new ChromeOptions();

                opt.AddArgument(
                    @"user-data-dir=C:\Users\LordNikon\AppData\Local\Google\Chrome\User Data");
                opt.AddArgument("--start-maximized");
                opt.AddArgument("--headless");
                opt.AddExcludedArgument("enable-automation");
                opt.AddAdditionalCapability("useAutomationExtension", false);
                opt.LeaveBrowserRunning = false;
                opt.AddArgument("ignore-certificate-errors");
                _ua = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36";
                opt.AddArgument("user-agent=" + _ua);

                _driver = new ChromeDriver(opt);
                _driver.Manage().Cookies.DeleteAllCookies();

                // IJavaScriptExecutor js = (IJavaScriptExecutor)_driver;
                //_ua = (string)js.ExecuteScript("return navigator.userAgent;");
                // listBox1.Items.Add("UA: " + _ua);
                //_cookies = (string)js.ExecuteScript("return document.cookie;");
                //listBox1.Items.Add("COOKIES: " + _cookies);
                // js.ExecuteScript("Object.defineProperty(navigator, 'languages', { get: () => ['en-US', 'en'], });");
                // js.ExecuteScript("Object.defineProperty(navigator, 'language', { get: () => 'en-US' });");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }


        Bitmap _bitMapCopy;
        private async void buttonExample_Click(object sender, EventArgs e)
        {
            try
            {
                if (_bitMapCopy != null) _bitMapCopy.Dispose();

                buttonSTART.Visible = true;

                var pathToImg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERRORS", "captcha.jpg");
                if (!File.Exists(pathToImg)) return;

                #region save image to byte array

                byte[] arr;
                using (FileStream fs = new FileStream(pathToImg, FileMode.Open, FileAccess.Read))
                {
                    arr = new byte[fs.Length];
                    using (MemoryStream mem = new MemoryStream(arr))
                    {
                        fs.CopyTo(mem);
                    }
                }

                using (MemoryStream mem = new MemoryStream(arr))
                {
                    _bitMapCopy = new Bitmap(mem, false);
                }

                pictureBox1.Image = _bitMapCopy;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                #endregion


                using (Bitmap image = new Bitmap(pathToImg))
                {
                    //++++ 
                    ResizeNearestNeighbor filter = new ResizeNearestNeighbor(image.Width * 2, image.Height * 2);
                    using (Bitmap clone = image.Clone(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                    {
                        using (Bitmap afterFilter = filter.Apply(clone))
                        {

                            labelAnsCaptcha.Text = MethodsCaptchaSolver.OCR(image);
                        }

                    }


                }


                return;

                //OCR 


                return;

                _currentMail = null;

                #region get e-mail
                /*
                TimerN(() =>
                {
                    if (!_chrome.IsBrowserInitialized) return false;
                    if (!_chrome.CanExecuteJavascriptInMainFrame) return false;
                    // if (_chrome.IsLoading) return false;

                    _chrome.EvaluateScriptAsync("(function(){" +
                                                         "var res = document.getElementById('mail').value;" +
                                                         "if(res != null){return res;}else{" +
                                                            "return 'Загрузка';" +
                                                         "}" +
                                                         "})();").ContinueWith((t) =>
                                                         {
                                                             string email = t.Result.Result.ToString();

                                                             if (!string.IsNullOrEmpty(email) & email.Contains("@"))
                                                             {
                                                                 listBox1.Items.Add(email);
                                                                 _currentMail = email;
                                                                 textBox2.Text = _currentMail;
                                                                 label2.BackColor = Color.Magenta;

                                                             }
                                                         });

                    if (!string.IsNullOrEmpty(_currentMail)) return true;
                    return false;
                }, 1000);


                #endregion

                return;


                #region click by letter

                var script1 = "(function(){ " +
                              "var elems = document.querySelectorAll(\"a[class='viewLink title-subject']\");  " +
                              "var el=null; " +
                              "elems.forEach(function(value){if(value.innerText.includes('Email verification')){el = value;}}); " +
                              "if(el != null){el.click();  return true;}else{return false;}" +
                              " })();";

                TimerN(() =>
                {
                    if (!_chrome.IsBrowserInitialized) return false;
                    if (!_chrome.CanExecuteJavascriptInMainFrame) return false;
                    if (_chrome.IsLoading) return false;

                    var res = _chrome.EvaluateScriptAsync(script1);


                    bool yes;
                    if (Boolean.TryParse(res.Result.Result.ToString(), out yes))
                    {
                        if (yes)
                        {
                            label2.BackColor = Color.Lime;
                            return true;
                        }
                    }

                    _chrome.Reload();

                    return false;
                }, 3000);

                #endregion

                #region get answer

                string script2 = "(function(){ " +
                                 "var res = document.getElementsByTagName('pre')[0];" +
                                 "if(res!= null){res.innerText.includes('please use the following validation code'); return res.innerText;}else{return '';} " +
                                 "})();";

                TimerN(() =>
                {
                    if (!_chrome.IsBrowserInitialized) return false;
                    if (!_chrome.CanExecuteJavascriptInMainFrame) return false;
                    if (_chrome.IsLoading) return false;

                    var res = _chrome.EvaluateScriptAsync(script2);

                    if (string.IsNullOrEmpty(res.Result.ToString())) return false;

                    string body = res.Result.Result.ToString();

                    if (!string.IsNullOrEmpty(body))
                    {
                        listBox1.Items.Add(body);

                        _linkTo +=
                               Regex.Match(body, "(\\d{6,12})")
                                   .Groups[1].Value.Trim();

                        if (string.IsNullOrEmpty(_linkTo)) return false;

                        _linkTo += ";";

                        listBox1.Items.Add("monkey: " + _linkTo);
                        return true;
                    }

                    return false;
                }, 3500);

                 */
                #endregion



            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }

        }



        #region E - MAIL

        private bool Email()
        {
            try
            {

                if (checkBoxTempMail.Checked)
                {

                    #region define data

                    Faker fake = new Faker();
                    Random rand = new Random();

                    string userName = fake.Internet.UserName().ToLower();
                    while (true)
                    {
                        if (userName.Length > 6 && userName.Length < 15)
                        {
                            break;
                        }

                        userName = fake.Internet.UserName().ToLower();
                    }
                    userName = userName.Replace("_", string.Empty).Replace(".", string.Empty) + rand.Next(1, 100);

                    #endregion

                    List<string> workedDmn = new List<string>();
                    string[] zones = { "com", "net", "org", "info", "biz" };
                    Random r = new Random();
                    string zone = zones[r.Next(zones.Length)];
                    Connect obj = new Connect(Uri.EscapeUriString("https://generator.email/search.php?key=" + zone));
                    obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
                    obj.IsKeepAlive = false;
                    obj.IsRedirect = true;
                    obj.UserAgent = _ua;

                    //obj.IsDebbug = true;
                    //obj.IsExtendDebbug = true;


                    if (!checkBoxTempMailSelfDmns.Checked)
                    {

                        #region не проверенные доменные имена

                        string ans = obj.RequestGet(listBox1);
                        List<string> list = JsonConvert.DeserializeObject<List<string>>(ans);
                        if (list == null || list.Count == 0) return false;

                        foreach (string s in list)
                        {
                            if (!zones.Any((a) => Regex.IsMatch(s, a + "$"))) continue;
                            //if (!Regex.IsMatch(s, "com$")) continue;
                            if (s.Length > 20 || s.Length < 10) continue;
                            if (s.Contains("kukold")) continue;
                            if (Regex.IsMatch(s, "\\d{5,100}")) continue;
                            if (s.Split('.').Length > 3) continue;

                            workedDmn.Add(s);
                            this.Invoke(new MethodInvoker(() => listBox1.Items.Add(s)));
                        }


                        #endregion

                    }
                    else
                    {
                        if (File.Exists("ERRORS/temp/tempMailDomains.txt"))
                        {
                            List<string> self = File.ReadAllLines("ERRORS/temp/tempMailDomains.txt").ToList();
                            workedDmn = self.OrderByDescending(a => Guid.NewGuid().ToString()).ToList();
                        }
                    }


                    if (File.Exists("ERRORS/temp/tempMailErr.txt"))
                    {
                        List<string> except = File.ReadAllLines("ERRORS/temp/tempMailErr.txt").ToList();
                        int z = except.Count;
                        int z2 = workedDmn.Count;

                        workedDmn = workedDmn.Except(except).ToList();

                        this.Invoke(new MethodInvoker(() => listBox1.Items.Add("after except: " + z2 + " except " + z + " = " + workedDmn.Count)));

                        //save

                    }

                    if (!workedDmn.Any()) return false;

                    do
                    {
                        string domain = workedDmn.OrderBy(x => Guid.NewGuid().ToString()).FirstOrDefault();

                        _currentMail = userName + "@" + domain;
                        this.Invoke(new MethodInvoker(() => textBox2.Text = "GetMyEmailOnet M1: https://generator.email/" + _currentMail + " :: temp mail :: " + _pass));

                        obj.URL = new Uri("https://generator.email/" + _currentMail);
                        obj.RequestGet(listBox1);

                        var arr = _currentMail.Split('@');
                        string to = string.Format("usr={0}&dmn={1}", arr[0], arr[1]);

                        // проверка на валидность почтового домена
                        obj.URL = new Uri("https://generator.email/check_adres_validation3.php");
                        obj.NameValColl = new NameValueCollection();
                        obj.NameValColl.Add("X-Requested-With", "XMLHttpRequest");
                        string json = obj.RequestPost(to, listBox1);

                        this.Invoke(new MethodInvoker(() => listBox1.Items.Add(_currentMail + " Valid: " + json)));
                        dynamic d = JsonConvert.DeserializeObject<dynamic>(json);

                        if (d.status.ToString().Equals("bad"))
                        {
                            #region получили ошибку о том что почта недоступна

                            this.Invoke(new MethodInvoker(() => listBox1.Items.Add("Valid: " + d.status)));

                            if (checkBoxTempMailSelfDmns.Checked) RemSingleName(domain, "ERRORS/temp/tempMailDomains.txt");

                            workedDmn.Remove(domain);

                            foreach (string s in workedDmn)
                            {
                                // listBox1.Items.Add(s);
                            }

                            if (!workedDmn.Any()) return false;

                            continue;

                            #endregion
                        }

                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(_currentMail);
                        }
                        catch
                        {
                            return false;
                        }

                        #region почта верифицирована - значит ее можно сохранить

                        if (!checkBoxTempMailSelfDmns.Checked)
                        {
                            if (File.Exists("ERRORS/temp/tempMailDomains.txt"))
                            {
                                List<string> self = File.ReadAllLines("ERRORS/temp/tempMailDomains.txt").ToList();

                                if (!self.Contains((domain)))
                                {
                                    File.AppendAllText("ERRORS/temp/tempMailDomains.txt", domain + Environment.NewLine);
                                }
                            }
                        }

                        #endregion


                        break;
                    } while (true);



                    return true;
                }

                #region постоянная почта

                if (!File.Exists(_mailFile))
                {
                    MessageBox.Show("У вас нет файла с почтами...");
                    return false;
                }
                // загрузить мыло из файла
                string[] mails = File.ReadAllLines(_mailFile);

                if (mails.Length == 0)
                {
                    MessageBox.Show("У вас закончилась почта.");
                    return false;
                }

                int next = 0;


                //string[] mym = mails.OrderByDescending(a => Guid.NewGuid().ToString()).ToArray();

                mails = mails[next].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                _currentMail = mails[0];
                _mailPass = mails[1];

                textBox2.Text = "GetMyEmailOnet M1: " + _currentMail + " :: " + _mailPass + " :: " + _pass;
                return true;

                #endregion
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
            return false;
        }



        private int _box;
        private async void EmailPop3(string server, string login, string pass, CancellationTokenSource cs = null)
        {

            try
            {
                string subject = null, pattern = null;

                if (radioButtonMymeriva.Checked)
                {
                    subject = "Dynamic DNS Account activation.";
                    pattern = "Verification code:\\s{0,3}(\\d{7,12})";

                }
                else if (radioButtonSomee.Checked)
                {
                    subject = "Email verification (Trial Version)";
                    pattern = "(\\d{6,12})";
                }


                string body = null;

                MailServer oServer = new MailServer(server, login, pass, ServerProtocol.Imap4);

                oServer.SSLConnection = true;
                oServer.Port = 993;//143; //993;

                MailClient oClient = new MailClient("TryIt");
                oClient.Connect(oServer);

                if (cs != null) cs.Cancel();

                //  listBox1.Items.Add("Email start. Current folder: " + oClient.SelectedFolder);

                bool isFind = false;
                foreach (Imap4Folder folder in oClient.Imap4Folders)
                {
                    if (_cancelEmail.IsCancellationRequested)
                    {
                        listBox1.Items.Add("Error: Get cancel token from email.");
                        return;
                    }

                    if (folder.Name == "Trash") continue;
                    oClient.SelectFolder(folder);
                    MailInfo[] infos = oClient.GetMailInfos();

                    // listBox1.Items.Add("Folder name: " + folder.Name + "::" + folder.LocalPath + "::" + infos.Length);

                    label3.Text = "Mails: " + folder.Name + " " + infos.Length + "; Tick:" + _box;
                    _box++;

                    for (int i = 0; i < infos.Length; i++)
                    {
                        MailInfo info = infos[i];

                        Mail email = oClient.GetMail(info);

                        listBox1.Items.Add("\tE-mail name: " + email.Subject);

                        if (email.Subject.Contains(subject))
                        {

                            #region delete e-mail

                            if (checkBoxDeleteEmail.Checked)
                            {
                                var folds = oClient.Imap4Folders.Where(z => z.Name == "Trash");

                                oClient.Move(info, folds.ElementAt(0));
                                oClient.Expunge();
                                listBox1.Items.Add("Письмо удалено.");
                                return;
                            }

                            #endregion

                            #region Read email

                            _box = 0;
                            label3.BackColor = Color.Yellow;

                            listBox1.Items.Add("E-mail: " + email.Subject);

                            //File.AppendAllText("ERRORS/Mail.html", email.Subject + "\r\n\r\n" + email.HtmlBody + Environment.NewLine);

                            body = email.HtmlBody;

                            _linkTo +=
                                Regex.Match(body, pattern)
                                    .Groups[1].Value.Trim();

                            if (string.IsNullOrEmpty(_linkTo)) return;

                            _linkTo += ";";

                            listBox1.Items.Add("monkey: " + _linkTo);

                            isFind = true;

                            #endregion
                        }


                    }//for


                }//for

                if (isFind)
                {
                    oClient.Quit();
                    return;
                }

                oClient.Quit();

                await Task.Delay(TimeSpan.FromSeconds(15));

                EmailPop3(server, login, pass);

            }
            catch (MailServerException em)
            {

                if (em.Message.Contains("A0002 NO [UNAVAILABLE]") || em.Message.Contains("A0002 NO [AUTHENTICATIONFAILED]"))
                {

                    // if (DialogResult.Yes == MessageBox.Show(em.Message + " :: Is delete account " + _currentMail + " ?", "", MessageBoxButtons.YesNo))
                    //  {
                    this.Invoke(new MethodInvoker(() =>
                    {
                        listBox1.Items.Add("(id thread: " + Thread.CurrentThread.ManagedThreadId + ") Error e-mail:: " + em.Message);

                        RemoveMail(_currentMail);
                        _currentMail = null;

                    }));

                    if (!Email()) return;
                    EmailPop3(server, _currentMail, _mailPass, cs);


                    // }

                    return;
                }
                MessageBox.Show("Вероятно выбран неверный сервер или неверный пароль. " + em.Message + " E-mail: " + login + " server: " + server);
            }
            catch (SocketException es)
            {
                listBox1.Items.Add("(id thread: " + Thread.CurrentThread.ManagedThreadId + ") Socket: " + es.Message);

                EmailPop3(server, login, pass, cs);
            }
            catch (Exception ez)
            {
                MessageBox.Show(ez.GetBaseException().ToString());
            }
        }



        private void RemoveMail(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }


                listBox1.Items.Add("Remove Mail:  " + name);

                string[] trm = File.ReadAllLines(_mailFile);

                using (StreamWriter read = new StreamWriter(_mailFile))
                {
                    for (int i = 0; i < trm.Length; i++)
                    {
                        var s = trm[i].Trim(); // строка из файла

                        if (s.Contains(name))
                        {
                            continue;
                        }

                        if (i == trm.Length - 1)
                        {
                            read.Write(s);
                            break;
                        }

                        // записать в файл
                        read.Write(s + "\r\n");
                    }

                }


            }
            catch (Exception ef)
            {
                MessageBox.Show(ef.GetBaseException().ToString());
            }
        }



        private void buttonDeleteMail_Click(object sender, EventArgs e)
        {
            try
            {
                buttonSTART.Visible = true;

                _emailError = 0;

                _linkTo = null;

                RemoveMail(_currentMail);
                _currentMail = null;
                _mailPass = null;
                _pass = null;

            }
            catch (Exception eg)
            {
                MessageBox.Show(eg.GetBaseException().ToString());
            }
        }

        private void buttonStartEmail_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            _linkTo = null;
            listBox1.Items.Add("LinkTo: " + _linkTo);

            #region E-mail

            _cancelEmail = new CancellationTokenSource();
            CancellationTokenSource cs = new CancellationTokenSource();
            cs.Cancel();

            if (checkBoxTempMail.Checked)
            {
                #region temp e-mail

                this.Invoke(new MethodInvoker(() =>
                {
                    Connect mail = new Connect();
                    mail.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                        SecurityProtocolType.Tls12;
                    mail.IsKeepAlive = true;
                    mail.IsRedirect = true;
                    mail.UserAgent = _ua;


                    #region Temp e-mail

                    TimerN(() =>
                    {

                        if (_cancelEmail.IsCancellationRequested)
                        {
                            listBox1.Items.Add("Error: Get cancel token from email.");
                            return true;
                        }

                        label3.Text = "Mails: " + _box;
                        _box++;

                        mail.URL = new Uri("https://generator.email/" + _currentMail);
                        string ansM = mail.RequestGet(listBox1);

                        if (ansM.Contains("Email verification"))
                        {
                            _box = 0;
                            label3.BackColor = Color.Yellow;

                            List<string> urls = GetUrlsFromPage(ansM, "a", "Email verification", true);
                            if (urls == null) return true;
                            foreach (string r in urls)
                            {

                                string tempUrl = "https://generator.email" + r;

                                listBox1.Items.Add("temp Url: " + tempUrl);

                                mail.URL = new Uri(tempUrl);
                                ansM = mail.RequestGet(listBox1);

                                var m = Regex.Match(ansM, "(?<=following\\ validation\\ code:)[\\w\\W]*?(?=Regards,)");
                                if (m.Success)
                                {
                                    _linkTo += m.Value.Trim() + ";";
                                    listBox1.Items.Add("monkey: " + _linkTo);

                                }

                            }

                            return true;
                        }

                        return false;
                    }, 5000);

                    #endregion



                }));

                #endregion
            }
            else
            {
                #region

                object combo = comboBoxEmail.SelectedItem;
                if (combo != null)
                {

                    if (string.IsNullOrEmpty(_currentMail))
                    {
                        listBox1.Items.Add("E-mail is empty.");
                        return;
                    }


                    Task.Factory.StartNew(() => EmailPop3(combo.ToString(), _currentMail, _mailPass, cs));
                }

                #endregion
            }

            #endregion

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (_cancelEmail == null) return;
            _cancelEmail.Cancel();
        }


        #endregion

        private void comboBoxIpServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxIpServer.SelectedIndex == -1) return;

            string[] mass = comboBoxIpServer.SelectedItem.ToString()
                       .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            if (mass.Length != 2) return;

            _ip = IPAddress.Parse(mass[0]);

        }


        #region Check accounts


        class Package
        {
            public string Company { get; set; }
            public double Weight { get; set; }
            public long TrackingNumber { get; set; }
        }



        private int _next = 0;
        private void buttonCheckAccs_Click(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.Clear();

                var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ERRORS", "temp");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                if (File.Exists(Path.Combine(logDir, "badDomains.txt"))) File.Delete(Path.Combine(logDir, "badDomains.txt"));

                List<string> lists = null;
                lists = richTextBox1.Lines.ToList();

                //await Task.Run(() => GetBadLinks(lists, logDir));


                BackgroundWorker wk = new BackgroundWorker();
                wk.WorkerReportsProgress = true;
                wk.WorkerSupportsCancellation = true;
                wk.DoWork += new DoWorkEventHandler((obj, arg) =>
                {

                    GetBadLinks(lists, logDir, wk);
                });
                wk.ProgressChanged += new ProgressChangedEventHandler((obj, arg) =>
                {
                    var list = arg.UserState as List<string>;
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            listBox2.Items.Add($"{arg.ProgressPercentage}) {item}");
                        }

                    }
                });
                wk.RunWorkerAsync();




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }


        private void GetBadLinks(List<string> lists, string logDir, BackgroundWorker wk)
        {
            try
            {


                if (radioButtonSomee.Checked)
                {
                    Connect obj = new Connect();
                    obj.IsProxy = false;

                    obj.ProtocolType = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 |
                                       SecurityProtocolType.Tls12;
                    obj.AutoDecompress = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    obj.IsKeepAlive = true;
                    obj.IsRedirect = false;

                    //obj.IsDebbug = true;
                    //obj.IsExtendDebbug = true;
                    List<string> log = new List<string>();
                    int count = 0;
                    while (true)
                    {
                        count++;
                        if (_next >= lists.Count)
                        {

                            wk.ReportProgress(count, log);
                            var res = MessageBox.Show(@"Файл создан ERRORS\badDomains.txt. Открыть каталог ?", null,
                                MessageBoxButtons.YesNo);
                            if (res == DialogResult.Yes)
                            {
                                Process.Start("explorer.exe", logDir);
                            }

                            return;
                        }

                        string[] splits = lists[_next].Split(';');

                        if (splits.Length < 4)
                        {
                            _next++;
                            log.Add($"items<4::{lists[_next]}");
                            continue;
                        }


                        try
                        {

                            string domain = splits[0];

                            obj.URL = new Uri("http://" + domain);
                            obj.RequestGet(log, null, true);
                            HttpStatusCode status = obj.Response.StatusCode;
                            if (status == HttpStatusCode.OK)
                            {
                                this.Invoke(new MethodInvoker(() => listBox2.Items.Add(_next + ") " + domain + ":: Ok")));
                            }
                            else
                            {
                                this.Invoke(
                                    new MethodInvoker(
                                        () => listBox2.Items.Add(_next + ") " + domain + ":: Bad: " + (int)status)));
                            }

                            _next++;
                        }
                        catch (WebException wex)
                        {
                            using (FileStream fs = File.Open(Path.Combine(logDir, "badDomains.txt"), FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                            {

                                using (StreamWriter wr = new StreamWriter(fs))
                                {
                                    wr.WriteLine(lists[_next]);
                                }
                            }

                            _next++;


                        }
                    }//while


                }


            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(() => MessageBox.Show(ex.GetBaseException().ToString())));
            }

        }


        #endregion




        #region context menu

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left) return;

                ContextMenu cm = new ContextMenu();//make a context menu instance
                MenuItem item = new MenuItem();//make a menuitem instance
                item.Text = "Копировать";
                item.Click += (a1, b2) => { Copy(); };
                cm.MenuItems.Add(item);
                item = new MenuItem();
                item.Text = "вставить";//give the item a header
                item.Click += (a1, b2) => { richTextBox1.Text = Clipboard.GetText(); };
                cm.MenuItems.Add(item);//add the item to the context menu
                item = new MenuItem();//recycle the menu item
                item.Text = "вырезать";//give the item a header
                item.Click += DoNothing;//give the item a click event handler
                cm.MenuItems.Add(item);//add the item to the context menu
                item = new MenuItem();//recycle item into a new menuitem
                item.Text = "удалить все";//give the item a header.
                item.Click += (objz, ereg) => { richTextBox1.Clear(); };//give the item a click event handler
                cm.MenuItems.Add(item);//add the item to the context menu
                //richTextBox1.ContextMenu = cm;//add the context menu to the sender
                cm.Show(richTextBox1, e.Location);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }
        }

        private void DoNothing(object sender, EventArgs e)
        {
            return;
        }


        private void Copy()
        {
            try
            {
                int index = richTextBox1.SelectionStart;
                int line = richTextBox1.GetLineFromCharIndex(index);
                string lineText = (richTextBox1.Lines.Length > 0) ? richTextBox1.Lines[line] : "";
                if (string.IsNullOrWhiteSpace(lineText)) return;
                Clipboard.SetText(lineText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().ToString());
            }

        }

        #endregion

        private void buttonExtract_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {

                    string zpth = @"ERRORS\doors.zip";
                    string extractDir = Path.Combine(Environment.CurrentDirectory, @"ERRORS\extract");


                    if (!File.Exists(@"ERRORS\badDomains.txt")) return;
                    if (!Directory.Exists(extractDir)) return;
                    if (!File.Exists(zpth)) return;

                    ZipFile.ExtractToDirectory(zpth, extractDir);
                    return;

                    using (ZipArchive zip = ZipFile.OpenRead(zpth))
                    {
                        listBox2.Items.Add("Entries:" + zip.Entries.Count);

                        foreach (ZipArchiveEntry entry in zip.Entries)
                        {
                            string[] count = entry.FullName.Split('/');

                            if (entry.FullName.EndsWith("/") && count.Length == 2)
                            {
                                listBox2.Items.Add(entry.Name + "::" + entry.FullName);
                                listBox2.Items.Add("to: " + Path.Combine(extractDir, entry.FullName));
                                entry.ExtractToFile(Path.Combine(extractDir, entry.Name));
                                break;
                            }
                        }
                    }




                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetBaseException().ToString());
                }
            });
        }


    }
}
