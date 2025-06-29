namespace SoukeyService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SokeyMinerServiceInstall = new System.ServiceProcess.ServiceProcessInstaller();
            this.sominerServerInstall = new System.ServiceProcess.ServiceInstaller();
            // 
            // SokeyMinerServiceInstall
            // 
            this.SokeyMinerServiceInstall.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.SokeyMinerServiceInstall.Password = null;
            this.SokeyMinerServiceInstall.Username = null;
            // 
            // sominerServerInstall
            // 
            this.sominerServerInstall.Description = "网络矿工数据采集服务器";
            this.sominerServerInstall.DisplayName = "NetminerGatherService";
            this.sominerServerInstall.ServiceName = "SMGatherService";
            this.sominerServerInstall.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.sominerServerInstall_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.SokeyMinerServiceInstall,
            this.sominerServerInstall});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller SokeyMinerServiceInstall;
        private System.ServiceProcess.ServiceInstaller sominerServerInstall;
    }
}