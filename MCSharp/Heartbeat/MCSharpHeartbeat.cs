﻿using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MCSharp.Heartbeat
{
    public class MCSharpHeartbeat : Heartbeat
    {
        static BackgroundWorker worker;
        static MCSharpHeartbeat instance;

        public static void Init ()
        {
            instance = new MCSharpHeartbeat();
            worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
            worker.RunWorkerAsync();
        }
        static void worker_DoWork (object sender, DoWorkEventArgs e)
        {
            if (MinecraftHeartbeat.Hash != null)
            {
                instance.DoHeartBeat();
            }
            Thread.Sleep(instance.Timeout);
        }

        static void worker_RunWorkerCompleted (object sender, RunWorkerCompletedEventArgs e)
        {
            worker.RunWorkerAsync();
        }

        public MCSharpHeartbeat ()
        {
            _timeout = 60000;
            serverURL = "http://mcsharp.voziv.com/heartbeat.php";
            staticPostVars = "port=" + Properties.ServerPort +
            "&max=" + Properties.MaxPlayers +
            "&name=" + Uri.EscapeDataString(Properties.ServerName) +
            "&public=" + Properties.PublicServer +
            "&version=7";
        }

        void UpdateHeartBeatPostVars ()
        {
            postVars = staticPostVars;
            postVars += "&users=" + (Player.number);
            postVars += "&motd=" + Uri.EscapeDataString(Properties.ServerMOTD) +
                        "&hash=" + MinecraftHeartbeat.Hash +
                        "&data=" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public bool DoHeartBeat ()
        {
            bool success = false;
            byte[] formData = { };

            UpdateHeartBeatPostVars();
            try
            {
                request = (HttpWebRequest) WebRequest.Create(new Uri(serverURL));
                request.Timeout = 10000;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                formData = Encoding.ASCII.GetBytes(staticPostVars + postVars);
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
                    Logger.Log("Preparing mcsharp.voziv.com heartbeat.", LogType.Debug);
                    using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                    {

                        string line = responseReader.ReadLine();
                        // Heartbeat does nothing except check in
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                    Logger.Log("Timeout: mcsharp.voziv.com", LogType.Debug);
                    Logger.Log("MCSharp Heartbeat Timed out!", LogType.Error);
                }
                else
                {
                    using (WebResponse response = ex.Response)
                    {
                        using (StreamReader responseReader = new StreamReader(response.GetResponseStream()))
                        {

                            string line = responseReader.ReadLine();
                            Logger.Log(line, LogType.ErrorMessage);
                            Logger.Log(serverURL, LogType.ErrorMessage);
                        }
                    }
                    Logger.Log("Failed Heartbeat to mcsharp.voziv.com: The status was " + ex.Status.ToString(), LogType.Error);
                    Logger.Log(ex.Message, LogType.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error reporting to mcsharp.voziv.com", LogType.Error);
                Logger.Log(ex.Message, LogType.ErrorMessage);
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
