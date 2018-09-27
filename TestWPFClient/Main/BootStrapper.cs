using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

using Prism.Modularity;
using Microsoft.Practices.Unity;
using Prism.Unity;
using Prism.Mvvm;

using Main.Views;
using Main;
//using ModuleMasterMaintenance;

namespace Main {
    class Bootstrapper : UnityBootstrapper {

        protected override DependencyObject CreateShell() {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell() {
            ViewModelLocator.SetAutoWireViewModel(this.Shell, true); 
            ((MainWindow)this.Shell).Show();
        }

        /// <summary>
        /// 使用するモジュール(プロジェクト)を登録
        /// </summary>
        protected override void ConfigureModuleCatalog() {
            var moduleCatalog = (ModuleCatalog)ModuleCatalog;
            //moduleCatalog.AddModule(typeof(ModuleName));
        }

        /// <summary>
        /// Viewを全てobject型としてコンテナに登録しておく(RegionManagerで使うため)
        /// </summary>
        protected override void ConfigureContainer() {
            base.ConfigureContainer();

            this.Container.RegisterTypes(
                AllClasses.FromLoadedAssemblies()
                    .Where(x => x.Namespace.EndsWith(".Views")),
                getFromTypes: _ => new[] { typeof(object) },
                getName: WithName.TypeName);

            // ViewModelLocatorでViewModelを生成する方法をUnityで行うようにする 
            ViewModelLocationProvider.SetDefaultViewModelFactory(t => this.Container.Resolve(t)); 
        }
    }
}
