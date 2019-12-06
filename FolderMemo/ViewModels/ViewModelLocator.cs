using Autofac;
using FolderMemo.ServiceInterfaces;
using FolderMemo.Services;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.Text;
using WpfApp1.ViewModels;

namespace FolderMemo.ViewModels
{
    public class ViewModelLocator : IDisposable
    {
        /// <summary>
        /// ViewModelLocator单例
        /// </summary>
        public static ViewModelLocator Instance
        {
            get;
            private set;
        }

        private IContainer _container;


        public ViewModelLocator()
        {
            ViewModelLocator.Instance = this;

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ChangeDirectoryDialogService>().Named<IOutputDialogService>("ChangeDirectoryDialogService").AsSelf();
            containerBuilder.RegisterType<SelectFileDialogService>().Named<IOutputDialogService>("SelectFileDialogService").AsSelf();

            containerBuilder.RegisterInstance(SimpleMessenger.Default).As<IMessenger>();
            containerBuilder.RegisterType<MainViewModel>().AsSelf().SingleInstance();

            _container = containerBuilder.Build();
        }

        public MainViewModel Main
        {
            get
            {
                return _container.Resolve<MainViewModel>();
            }
        }

        public IOutputDialogService ChangeDirectoryDialogService
        {
            get
            {
                return _container.Resolve<ChangeDirectoryDialogService>();
            }
        }


        public IOutputDialogService SelectFileDialogService
        {
            get
            {
                return _container.Resolve<SelectFileDialogService>();
            }
        }


        public void Dispose()
        {
        }
    }
}
