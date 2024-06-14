﻿using CommonPluginsShared.Collections;
using CommonPluginsShared.Controls;
using CommonPluginsShared.Interfaces;
using Playnite.SDK;
using Playnite.SDK.Models;
using SuccessStory.Models;
using SuccessStory.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SuccessStory.Controls
{
    /// <summary>
    /// Logique d'interaction pour PluginCompactList.xaml
    /// </summary>
    public partial class PluginCompactList : PluginUserControlExtend
    {
        private SuccessStoryDatabase PluginDatabase => SuccessStory.PluginDatabase;
        internal override IPluginDatabase pluginDatabase => PluginDatabase;

        private PluginCompactListDataContext ControlDataContext = new PluginCompactListDataContext();
        internal override IDataContext controlDataContext
        {
            get => ControlDataContext;
            set => ControlDataContext = (PluginCompactListDataContext)controlDataContext;
        }


        public PluginCompactList()
        {
            InitializeComponent();
            DataContext = ControlDataContext;

            _ = Task.Run(() =>
            {
                // Wait extension database are loaded
                _ = System.Threading.SpinWait.SpinUntil(() => PluginDatabase.IsLoaded, -1);

                _ = Dispatcher?.BeginInvoke((Action)delegate
                {
                    PluginDatabase.PluginSettings.PropertyChanged += PluginSettings_PropertyChanged;
                    PluginDatabase.Database.ItemUpdated += Database_ItemUpdated;
                    PluginDatabase.Database.ItemCollectionChanged += Database_ItemCollectionChanged;
                    API.Instance.Database.Games.ItemUpdated += Games_ItemUpdated;

                    // Apply settings
                    PluginSettings_PropertyChanged(null, null);
                });
            });
        }


        public override void SetDefaultDataContext()
        {
            ControlDataContext.IsActivated = PluginDatabase.PluginSettings.Settings.EnableIntegrationCompact;
            ControlDataContext.Height = PluginDatabase.PluginSettings.Settings.IntegrationCompactHeight + 28;

            ControlDataContext.PictureHeight = PluginDatabase.PluginSettings.Settings.IntegrationCompactHeight;
            ControlDataContext.ItemsSource = new ObservableCollection<Achievements>();
        }


        public override void SetData(Game newContext, PluginDataBaseGameBase PluginGameData)
        {
            GameAchievements gameAchievements = (GameAchievements)PluginGameData;
            gameAchievements.orderAchievement = PluginDatabase.PluginSettings.Settings.IntegrationCompactOrderAchievement;
            ControlDataContext.ItemsSource = gameAchievements.OrderItems;
        }
    }


    public class PluginCompactListDataContext : ObservableObject, IDataContext
    {
        private bool isActivated;
        public bool IsActivated { get => isActivated; set => SetValue(ref isActivated, value); }

        private double height;
        public double Height { get => height; set => SetValue(ref height, value); }

        private double pictureHeight;
        public double PictureHeight { get => pictureHeight; set => SetValue(ref pictureHeight, value); }

        private ObservableCollection<Achievements> itemsSource;
        public ObservableCollection<Achievements> ItemsSource { get => itemsSource; set => SetValue(ref itemsSource, value); }
    }
}
