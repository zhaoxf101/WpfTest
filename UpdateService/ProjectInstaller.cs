using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace UpdateService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        public ServiceProcessInstaller ServiceProcessInstaller
        {
            get
            {
                return serviceProcessInstaller1;
            }
        }

        public ServiceInstaller ServiceInstaller
        {
            get
            { return serviceInstaller1; }
        }
    }
}
