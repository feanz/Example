using Example.Data.Repositories.Interfaces;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using NHibernate;
using Ninject;
using Ninject.Web.Mvc;
using Web.Infrastructure.Logging;
using Example.Data.Repositories;
using Example.Data;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Web.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Web.App_Start.NinjectMVC3), "Stop")]

namespace Web.App_Start
{
    public static class NinjectMVC3 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
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
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>        
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ILogger>().To<ElmahLogger>();
            kernel.Bind<ISessionFactory>().ToConstant(NHibernateConfiguration.BuildSessionFactory());
            kernel.Bind<ISession>().ToMethod(x => x.Kernel.Get<ISessionFactory>().OpenSession()).InRequestScope();            
            kernel.Bind<IAccountRepository>().To<AccountRepository>();
            kernel.Bind<IBookmarkRepository>().To<CacheBookmarkRepository>();
        }        
    }
}
