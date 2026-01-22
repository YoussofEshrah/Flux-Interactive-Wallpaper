using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop; // For WindowInteropHelper
using InteractiveWallpaper.Services; // Your service

namespace InteractiveWallpaper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        
        // Get our window handle
        var helper = new WindowInteropHelper(this);
        IntPtr handle = helper.Handle;
        
        // Set as wallpaper
        var wallpaperService = new WallpaperService();
        try
        {
            wallpaperService.SetAsWallpaper(handle);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to set as wallpaper: {ex.Message}");
        }
    }
}