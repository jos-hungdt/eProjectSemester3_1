using eProjectSemester3_1.Application;
using eProjectSemester3_1.Application.Context;
using eProjectSemester3_1.Application.Services;
using System;

using Unity;
using Unity.Lifetime;

namespace eProjectSemester3_1
{
    public static class UnityExtensions
    {
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }

        public static void BindInRequestScope<T1>(this IUnityContainer container)
        {
            container.RegisterType<T1>(new HierarchicalLifetimeManager());
        }

    }

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();
            //container.

            container.BindInRequestScope<AppDbContext>(); 
            container.BindInRequestScope<UnitOfWorkManager>();

            container.BindInRequestScope<CacheService>();
            container.BindInRequestScope<LoggingService>();

            container.BindInRequestScope<MembershipService>();
            container.BindInRequestScope<CategoryService>();
            container.BindInRequestScope<NewsService>();
            container.BindInRequestScope<PostService>();

            container.BindInRequestScope<EstateService>();
            container.BindInRequestScope<EstateStyleService>();
            container.BindInRequestScope<EstateTypeService>();
            container.BindInRequestScope<PostEstateTypeService>();
            container.BindInRequestScope<BanksService>();
            container.BindInRequestScope<BankLoanService>();

        }
    }
} 