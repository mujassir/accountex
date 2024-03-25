﻿//using Hangfire;
//using Hangfire.SqlServer;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Hosting;

//namespace AccountEx.Web
//{

//    public class HangfireBootstrapper : IRegisteredObject
//    {
//        public static readonly HangfireBootstrapper Instance = new HangfireBootstrapper();

//        private readonly object _lockObject = new object();
//        private bool _started;

//        private BackgroundJobServer _backgroundJobServer;

//        private HangfireBootstrapper()
//        {
//        }

//        public void Start()
//        {
//            lock (_lockObject)
//            {
//                if (_started) return;
//                _started = true;

//                HostingEnvironment.RegisterObject(this);


//                GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireConnection",
//              new SqlServerStorageOptions
//              {
//                  QueuePollInterval = TimeSpan.FromSeconds(1),
//                  PrepareSchemaIfNecessary = false
//              });
//                _backgroundJobServer = new BackgroundJobServer();
//            }
//        }

//        public void Stop()
//        {
//            lock (_lockObject)
//            {
//                if (_backgroundJobServer != null)
//                {
//                    _backgroundJobServer.Dispose();
//                }

//                HostingEnvironment.UnregisterObject(this);
//            }
//        }

//        void IRegisteredObject.Stop(bool immediate)
//        {
//            Stop();
//        }
//    }
//}