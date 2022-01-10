using Autofac;
using FolderMemo.ServiceInterfaces;
using FolderMemo.Services;
using MVVMLib;
using System;
using System.Collections.Generic;
using System.Text;

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

            // Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<ChangeDirectoryDialogService>().Named<IOutputDialogService>("ChangeDirectoryDialogService").AsSelf();
            containerBuilder.RegisterType<SelectFileDialogService>().Named<IOutputDialogService>("SelectFileDialogService").AsSelf();
            containerBuilder.RegisterType<SelectFolderDialogService>().Named<IOutputDialogService>("SelectFolderDialogService").AsSelf();

            containerBuilder.RegisterInstance(SimpleMessenger.Default).As<IMessenger>();
            containerBuilder.RegisterType<SingleCommentViewModel>().AsSelf().SingleInstance();
            containerBuilder.RegisterType<BatchCommentViewModel>().AsSelf().SingleInstance();

            _container = containerBuilder.Build();
        }

        public SingleCommentViewModel SingleCommentVM
        {
            get
            {
                return _container.Resolve<SingleCommentViewModel>();
            }
        }

        public BatchCommentViewModel BatchCommentVM
        {
            get
            {
                return _container.Resolve<BatchCommentViewModel>();
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


        public IOutputDialogService SelectFolderDialogService
        {
            get
            {
                return _container.Resolve<SelectFolderDialogService>();
            }
        }

        public void Dispose()
        {
        }
    }
}
