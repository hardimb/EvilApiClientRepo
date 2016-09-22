[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(EvilApiClient.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(EvilApiClient.Web.App_Start.NinjectWebCommon), "Stop")]

namespace EvilApiClient.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using EvilApiClient.Core.Repository;
    using EvilApiClient.Service;
    using Core.Common;
    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        /// <summary>
        {
            kernel.Bind<IEvilAPIRepository>().To<EvilUploadRepository>().WithConstructorArgument("objConfigData", GetConfigData());
            kernel.Bind<IUploadFileRepository>().To<UploadFileRepository>().WithConstructorArgument("configData", GetConfigData()).WithConstructorArgument("evilUploadRepository", ctx => ctx.Kernel.Get<IEvilAPIRepository>());

        }

        /// <summary>
        /// This method is used to fetch static data saved in app config file
        /// </summary>
        /// <returns></returns>
        private static ConfigData GetConfigData()
        {
            ConfigData configData = new ConfigData();
            configData.UploadUrl = System.Configuration.ConfigurationManager.AppSettings["UploadApiUrl"];
            configData.UploadAction = System.Configuration.ConfigurationManager.AppSettings["Action"];
            configData.CheckUrl = System.Configuration.ConfigurationManager.AppSettings["CheckCustomerApiUrl"];
            configData.ApiUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];

            return configData;
        }
    }
}
