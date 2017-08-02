using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace UpdateService
{
    public partial class UpdateService : ServiceBase
    {
        TaskService _service;

        public UpdateService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _service = new TaskService();
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }

        public void Start(string[] args)
        {
            OnStart(args);
        }

    }
}
