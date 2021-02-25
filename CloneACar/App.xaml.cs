using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using CloneACar.LogicalHelpers;

// Theme generating
using ControlzEx.Theming;

namespace CloneACar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Startup override.
            base.OnStartup(e);

            // Sync up theme manager and make it thread safe.
            ThemeManager.Current.ThemeSyncMode = ThemeSyncMode.SyncWithAppMode;
            ThemeManager.Current.SyncTheme();

            // Make custom color bases for brushes based on app config.
            string[] MainColorString = AppConfigHelper.ReturnConfigItem("BaseColorScheme").Split(',');
            string[] ShowColorString = AppConfigHelper.ReturnConfigItem("ShowcaseColorScheme").Split(',');

            List<byte> MainColorValues = Array.ConvertAll(MainColorString, Value => byte.Parse(Value.Trim())).ToList();
            List<byte> ShowColorValues = Array.ConvertAll(ShowColorString, Value => byte.Parse(Value.Trim())).ToList();

            var BaseColor = Color.FromArgb(
                MainColorValues[0], MainColorValues[1],
                MainColorValues[2], MainColorValues[3]
            );

            var ShowcaseColor = Color.FromArgb(
                ShowColorValues[0], ShowColorValues[1],
                ShowColorValues[2], ShowColorValues[3]
            );
            var ShowcaseBrush = new SolidColorBrush(BaseColor);

            // Generate the custom schemes here.
            var CustomDarkColors = new Theme(
                "CustomDarkColors", "CloneACar - Dark", "Dark", 
                "Custom", BaseColor, ShowcaseBrush, true, false
            );

            var CustomLightColors = new Theme(
                "CustomLightColors", "CloneACar - Light", "Light",
                "Custom", BaseColor, ShowcaseBrush, true, false
            );

            // Add them to the theme manager.
            ThemeManager.Current.AddTheme(CustomDarkColors);
            ThemeManager.Current.AddTheme(CustomLightColors);

            // Apply dark theme as default.
            ThemeManager.Current.ChangeTheme(this, CustomDarkColors);
        }
    }
}
