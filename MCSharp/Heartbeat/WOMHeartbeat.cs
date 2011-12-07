﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MCSharp.Heartbeat
{
    public class WOMHeartbeat : Heartbeat
    {
        static BackgroundWorker worker;
        static WOMHeartbeat instance;
        static string _hash = null;
        static string externalURL = "";
        public static string Hash { get { return _hash; } }

        public static void Init ()
        {
            if (instance == null)
            {
                instance = new WOMHeartbeat();
                worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(worker_DoWork);
                worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
                worker.RunWorkerAsync();
            }
        }
        static void worker_DoWork (object sender, DoWorkEventArgs e)
        {
            instance.DoHeartBeat();
            Thread.Sleep(instance.Timeout);
        }

        static void worker_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        public WOMHeartbeat ()
        {
            _timeout = 45000; // Beat every 45 seconds
            serverURL = "http://direct.worldofminecraft.com/hb.php";
            staticPostVars = "port=" + Properties.ServerPort +
                             "&max=" + Properties.MaxPlayers +
                             "&name=" + Uri.EscapeDataString(Properties.ServerName) +
                             "&public=" + Properties.PublicServer +
                             "&version=7&noforward=1";
        }

        void UpdateHeartBeatPostVars ()
        {
            postVars = staticPostVars;
            postVars += "&users=" + (Player.number);
            postVars += "&salt=" + Uri.EscapeDataString(Server.salt);
        }

        public bool DoHeartBeat ()
        {
            bool success = false;
            byte[] formData = { };

            UpdateHeartBeatPostVars();
            try
            {
                request = (HttpWebRequest) WebRequest.Create(new Uri(serverURL));
                request.Timeout = 20000;

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                formData = Encoding.ASCII.GetBytes(postVars);
                request.ContentLength = formData.Length;

                // CachePolicy is not Implemented in some versions of MONO. (1.9)
                try
                {
                    request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                }
                catch
                {
                    Logger.Log("Error Setting Heartbeat CachePolicy. If you are running MONO then this is OK.", LogType.Warning);
                }


                try
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(formData, 0, formData.Length);
                        requestStream.Close();
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.Timeout)
                    {
                        throw new WebException("Failed during request.GetRequestStream()", ex.InnerException, ex.Status, ex.Response);
                    }
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                    {
                        string line = responseReader.ReadToEnd().Trim();
                        File.WriteAllText("womurl.txt", line);
                        Logger.Log(line, LogType.Debug);
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    Logger.Log("Timeout: direct.worldofminecraft.com", LogType.Debug);
                    Logger.Log("WOM Heartbeat Timed out", LogType.Error);
                    Logger.Log(ex.Message, LogType.ErrorMessage);
                }
                else
                {
                    using (WebResponse response = ex.Response)
                    {
                        using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                        {
                            string line = responseReader.ReadLine();
                            Logger.Log(line, LogType.ErrorMessage);
                            Logger.Log(externalURL, LogType.ErrorMessage);
                        }
                    }
                    Logger.Log("Failed Heartbeat todirect.worldofminecraft.com: The status was " + ex.Status.ToString(), LogType.Error);
                    Logger.Log(ex.Message, LogType.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error reporting to direct.worldofminecraft.com", LogType.Error);
                Logger.Log(ex.Message, LogType.ErrorMessage);
                Logger.Log(serverURL, LogType.ErrorMessage);
                Logger.Log(postVars, LogType.ErrorMessage);
                success = false;
            }
            finally
            {
                request.Abort();
            }
            return success;
        }
    }
}
