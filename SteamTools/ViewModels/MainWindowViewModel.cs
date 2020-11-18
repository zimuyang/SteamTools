﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using MetroTrilithon.Mvvm;
using Livet;
using SteamTools.Models;
using SteamTool.Core;
using SteamTools.Win32;
using System.Windows;
using System.Windows.Interop;
using SteamTools.Services;
using SteamTools.Properties;

namespace SteamTools.ViewModels
{
    public class MainWindowViewModel : MainWindowViewModelBase
    {
        // ----- Tab items
        public WelcomePageViewModel WelcomePage { get; }
        public CommunityProxyPageViewModel CommunityProxyPage { get; }
        public SteamAccountPageViewModel AccountPage { get; }
        public LocalAuthPageViewModel LocalAuthPage { get; }
        public SteamAppPageViewModel SteamAppPage { get; }
        public AchievementUnlockedPageViewModel AchievementUnlockedPage { get; }
        public ArchiSteamFarmPlusPageViewModel AsfPlusPage { get; }
        public SteamIdlePageViewModel SteamIdlePage { get; }
        public SettingsPageViewModel SettingsPage { get; }
        public OtherPlatformPageViewModel OtherPlatformPage { get; }
        public GameRelatedPageViewModel GameRelatedPage { get; }

        public IList<TabItemViewModel> TabItems { get; set; }
        public IList<TabItemViewModel> SystemTabItems { get; set; }

        #region SelectedItem 変更通知

        private TabItemViewModel _SelectedItem;

        public TabItemViewModel SelectedItem
        {
            get { return this._SelectedItem; }
            set
            {
                if (this._SelectedItem != value)
                {
                    this._SelectedItem = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        private bool _Visible;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (this._Visible != value)
                {
                    this._Visible = value;
                    ChangeWindowVisible();
                }
            }
        }

        public MainWindowViewModel()
        {
            this.TabItems = new List<TabItemViewModel>
            {
                (this.WelcomePage = new WelcomePageViewModel(this).AddTo(this)),
                (this.CommunityProxyPage = new CommunityProxyPageViewModel().AddTo(this)),
                (this.AccountPage = new SteamAccountPageViewModel().AddTo(this)),
                (this.SteamAppPage = new SteamAppPageViewModel().AddTo(this)),
                (this.LocalAuthPage = new LocalAuthPageViewModel().AddTo(this)),
                (this.AchievementUnlockedPage = new AchievementUnlockedPageViewModel().AddTo(this)),
                (this.AsfPlusPage = new ArchiSteamFarmPlusPageViewModel().AddTo(this)),
                (this.SteamIdlePage = new SteamIdlePageViewModel().AddTo(this)),
                (this.OtherPlatformPage = new OtherPlatformPageViewModel().AddTo(this)),
                (this.GameRelatedPage = new GameRelatedPageViewModel().AddTo(this)),
                //(this.Expeditions = new ExpeditionsViewModel(this.Fleets).AddTo(this)),
            };
            this.SystemTabItems = new List<TabItemViewModel>
            {
                SettingsPageViewModel.Instance,
                AboutPageModel.Instance,
				#region DEBUG
#if DEBUG
				new DebugPageViewModel().AddTo(this),
#endif
				#endregion
			};
            this.SelectedItem = this.TabItems.FirstOrDefault();
            this.Initialize();
        }

        public async new void Initialize()
        {
            base.Initialize();
            //await Task.Yield();
            await Task.Run(() =>
            {
                foreach (var item in this.TabItems)
                {
                    if (item == SteamAppPage)
                        continue;
                    item.Initialize();
                }
            }).ContinueWith(s => s.Dispose());
            StatusService.Current.Set(Resources.Ready);
        }

        /// <summary>
        /// 窗口显示关闭
        /// </summary>
        public void ChangeWindowVisible()
        {
            App.Current.MainWindow = App.Current.MainWindow ?? new MainWindow();
            if (Visible)
            {
                App.Current.MainWindow.WindowState = WindowState.Minimized;
                App.Current.MainWindow.Hide();
            }
            else
            {
                App.Current.MainWindow.Show();
                //App.Current.MainWindow.Focus();
                App.Current.MainWindow.WindowState = WindowState.Normal;
                FlashTaskBar.FlashWindow(new WindowInteropHelper(App.Current.MainWindow).Handle);
            }
        }
    }
}
