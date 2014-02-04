using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace FileManager
{
    public static class ServiceLocator
    {
        private static readonly IServiceLocator msLocator = new MefServiceLocator();
        public static IServiceLocator Current
        {
            get
            {
                return msLocator;
            }
        }
    }
    public interface IServiceLocator
    {
        TService GetInstance<TService>();
        TService GetInstance<TService>(string key);
        IEnumerable<TService> GetAllInstances<TService>();
        void RegisterAssemblyTypes(Assembly assembly);
        void GetAssemblies(string path);
        void Register<T>(T obj);
    }


    public class MefServiceLocator : IServiceLocator
    {
        private readonly CompositionContainer mContainer;
        private readonly AggregateCatalog mCatalog;

        public MefServiceLocator()
            : this(Assembly.GetExecutingAssembly())
        {
        }

        public MefServiceLocator(params Assembly[] assemblies)
        {
            mCatalog = new AggregateCatalog();
            foreach (var assembly in assemblies)
            {
                mCatalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }
            mContainer = new CompositionContainer(mCatalog);
        }

        public void GetAssemblies(string path)
        {
          
        }

        public void Register<T>(T obj)
        {
            mContainer.ComposeExportedValue(obj);
        }

        public TService GetInstance<TService>()
        {
            try
            {
                return mContainer.GetExportedValue<TService>();
            }
            catch (Exception)
            {
                throw new Exception("Type was not found");
            }
        }

        public TService GetInstance<TService>(string key)
        {
            try
            {
                return mContainer.GetExportedValue<TService>(key);
            }
            catch (Exception)
            {
                throw new Exception("Type was not found");
            }
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            try
            {
                return mContainer.GetExportedValues<TService>();
            }
            catch (Exception exception)
            {
                throw new Exception("Type was not found");
            }
        }

        public void RegisterAssemblyTypes(Assembly assembly)
        {
            var assemblyCatalog = new AssemblyCatalog(assembly);
            mCatalog.Catalogs.Add(assemblyCatalog);
        }
    }
}