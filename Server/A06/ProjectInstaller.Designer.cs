namespace A06
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.A06ServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.A06ServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // A06ServiceProcessInstaller
            // 
            this.A06ServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.A06ServiceProcessInstaller.Password = null;
            this.A06ServiceProcessInstaller.Username = null;
            // 
            // A06ServiceInstaller
            // 
            this.A06ServiceInstaller.ServiceName = "A06Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.A06ServiceProcessInstaller,
            this.A06ServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller A06ServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller A06ServiceInstaller;
    }
}