using System;
using SpotifyAPI;

namespace MusicStream
{
    public class MusicStreamManager
    {
        /* Credentials for SpotifyAPI */
        private string spotifyUsername = "mybleton";
        private string spotifyPassword = "ctms";
        private byte[] spotifyApiKey = {
	        0x01, 0x56, 0x23, 0x98, 0x1D, 0xE0, 0x4D, 0x8A, 0xF5, 0xEA, 0xA3, 0x60, 0xE1, 0xE3, 0xE8, 0xF2,
	        0x43, 0xF8, 0xA8, 0x21, 0x2A, 0x92, 0x78, 0xEC, 0xFA, 0xB7, 0xDA, 0x55, 0x80, 0x1D, 0x59, 0xBC,
	        0x38, 0x2D, 0xE8, 0xEE, 0x27, 0xD5, 0xF8, 0x69, 0x03, 0x6F, 0x51, 0x9E, 0x08, 0x0D, 0x1B, 0xC4,
	        0x6D, 0xA7, 0xF0, 0x73, 0x8D, 0x79, 0xB5, 0x9E, 0x61, 0x9F, 0x3A, 0x6B, 0x2E, 0xBF, 0x4C, 0x69,
	        0xE4, 0xD5, 0xA0, 0xF5, 0x7E, 0x01, 0x4C, 0x54, 0x4F, 0x74, 0xD5, 0xD2, 0x60, 0x44, 0xFD, 0xBB,
	        0xBC, 0xD0, 0x33, 0xE2, 0x71, 0xD3, 0xB1, 0x58, 0x6F, 0xA3, 0x73, 0xB6, 0x66, 0x0C, 0x4F, 0x29,
	        0x30, 0x87, 0x6B, 0x85, 0x1C, 0x7A, 0x3D, 0x90, 0x6A, 0x54, 0x1E, 0xC3, 0x45, 0x8A, 0x4E, 0xDA,
	        0x82, 0x92, 0xA9, 0x46, 0xCB, 0x7F, 0xFE, 0x1E, 0x82, 0x23, 0xB9, 0xDD, 0x09, 0x4D, 0x77, 0x4F,
	        0x2A, 0xBE, 0x26, 0xAA, 0xD7, 0x70, 0xB2, 0x97, 0x29, 0x08, 0x27, 0x58, 0x53, 0x48, 0xFE, 0x08,
	        0x18, 0xCB, 0x05, 0xB0, 0x5A, 0x19, 0xD9, 0x87, 0xCE, 0x5C, 0xC7, 0x62, 0x04, 0x05, 0x37, 0x24,
	        0xEC, 0x70, 0x48, 0xC2, 0x3D, 0x01, 0x8A, 0xE0, 0x71, 0xF3, 0x2C, 0xF2, 0xF7, 0xA1, 0x1A, 0x23,
	        0x46, 0x6D, 0xF1, 0x0A, 0x45, 0x13, 0x14, 0xC7, 0x40, 0xAA, 0x29, 0x49, 0xE2, 0x74, 0xDD, 0x20,
	        0xF7, 0x2E, 0x4E, 0x78, 0xEF, 0x8A, 0xE5, 0x78, 0x46, 0xDB, 0xC5, 0xD2, 0x5F, 0x51, 0xFB, 0x7B,
	        0x6C, 0xE2, 0x9A, 0x43, 0x7C, 0xA2, 0x4A, 0xC5, 0xA2, 0xB1, 0x41, 0xCF, 0xA9, 0x51, 0xED, 0x29,
	        0x81, 0x7D, 0x16, 0x9F, 0x85, 0xE8, 0x3E, 0x8D, 0x05, 0x6E, 0xCD, 0x5B, 0xEC, 0xA5, 0xAB, 0x98,
	        0x3C, 0xDA, 0x6D, 0x2F, 0x5F, 0x6A, 0x9D, 0x9C, 0x16, 0x73, 0x7D, 0x80, 0x5B, 0x87, 0x87, 0xEF,
	        0x72, 0xE6, 0xF1, 0x72, 0x0B, 0xA3, 0x29, 0xC7, 0x32, 0xE0, 0x2E, 0x4A, 0x2C, 0xC7, 0x94, 0x85,
	        0xF4, 0xC8, 0x4E, 0x3F, 0x31, 0x1E, 0x30, 0x46, 0xD6, 0x80, 0x29, 0x69, 0x0D, 0xB3, 0x41, 0xC2,
	        0x0C, 0xAF, 0xD3, 0x68, 0xE1, 0xEE, 0x52, 0x4C, 0x32, 0x32, 0xCA, 0xBD, 0x84, 0xA7, 0x53, 0x34,
	        0xC6, 0x28, 0xFF, 0x1B, 0x06, 0x89, 0xC4, 0xE5, 0xAC, 0x05, 0x06, 0xC6, 0x99, 0xD8, 0x1F, 0x9A,
	        0x9A,
        };

        /* Method for testing connection to SpotifyAPI */
        public void Play()
        {
            /* 
             * Just libspotify.net (libspotifydotnet.dll)
             * Not working
             */
 
            /*libspotify.sp_session_config config = new libspotify.sp_session_config();
            config.api_version = libspotify.SPOTIFY_API_VERSION;
            config.cache_location = "";
            config.settings_location = config.cache_location;
            config.application_key = "";
            config.user_agent = "Samsung SUR40 PixelSense ASE-CTMS";
            config.callbacks = "";
            config.userdata = "";
            config.compress_playlists = true;
            config.dont_save_metadata_for_playlists = true;
            config.initially_unload_playlists = true;
            libspotify.sp_session_create(config, )*/



            /* 
             * spotify.net with libSpotifyAPI (SpotifyAPI.dll & libspotifydotnet.dll)
             *      - SpotifyAPI compiled from https://github.com/devurandom-development/libSpotifyAPI
             *      - Source-Code for SpotifyAPI in app/libs/libSpotifyAPI/SpotifyAPI.sln
             * 
             * -Status: Getting AccessViolationException after creating SpotifySession
             *      System.AccessViolationException wurde nicht behandelt.
                      HResult=-2147467261
                      Message=Es wurde versucht, im geschützten Speicher zu lesen oder zu schreiben. Dies ist häufig ein Hinweis darauf, dass anderer Speicher beschädigt ist.
                      Source=libspotifydotnet
                      StackTrace:
                           bei libspotifydotnet.libspotify.sp_session_create(sp_session_config& config, IntPtr& sessionPtr)
                           bei SpotifyAPI.SpotifySession..ctor(String Username, String Password, SpotifySessionConfig Configuration)
                           bei MusicStream.MusicStreamManager.Play() in C:\Users\Lukas\Workspace\ASE\CTMS\app\MusicStream\MusicStreamManager.cs:Zeile 50.
                           bei Ctms.Applications.Controllers.PlaylistController.Play() in C:\Users\Lukas\Workspace\ASE\CTMS\app\Ctms.Applications\Controllers\PlaylistController.cs:Zeile 75.
                           bei System.Waf.Applications.DelegateCommand.<>c__DisplayClass4.<.ctor>b__0(Object p) in C:\Users\Lukas\Workspace\ASE\CTMS\app\libs\WpfApplicationFramework\Applications\DelegateCommand.cs:Zeile 38.
                           bei System.Waf.Applications.DelegateCommand.Execute(Object parameter) in C:\Users\Lukas\Workspace\ASE\CTMS\app\libs\WpfApplicationFramework\Applications\DelegateCommand.cs:Zeile 113.
                           bei MS.Internal.Commands.CommandHelpers.CriticalExecuteCommandSource(ICommandSource commandSource, Boolean userInitiated)
                           bei System.Windows.Controls.Primitives.ButtonBase.OnClick()
                           bei System.Windows.Controls.Button.OnClick()
                           bei System.Windows.Controls.Primitives.ButtonBase.OnMouseLeftButtonUp(MouseButtonEventArgs e)
                           bei System.Windows.UIElement.OnMouseLeftButtonUpThunk(Object sender, MouseButtonEventArgs e)
                           bei System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)
                           bei System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)
                           bei System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)
                           bei System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)
                           bei System.Windows.UIElement.ReRaiseEventAs(DependencyObject sender, RoutedEventArgs args, RoutedEvent newEvent)
                           bei System.Windows.UIElement.OnMouseUpThunk(Object sender, MouseButtonEventArgs e)
                           bei System.Windows.Input.MouseButtonEventArgs.InvokeEventHandler(Delegate genericHandler, Object genericTarget)
                           bei System.Windows.RoutedEventArgs.InvokeHandler(Delegate handler, Object target)
                           bei System.Windows.RoutedEventHandlerInfo.InvokeHandler(Object target, RoutedEventArgs routedEventArgs)
                           bei System.Windows.EventRoute.InvokeHandlersImpl(Object source, RoutedEventArgs args, Boolean reRaised)
                           bei System.Windows.UIElement.RaiseEventImpl(DependencyObject sender, RoutedEventArgs args)
                           bei System.Windows.UIElement.RaiseTrustedEvent(RoutedEventArgs args)
                           bei System.Windows.UIElement.RaiseEvent(RoutedEventArgs args, Boolean trusted)
                           bei System.Windows.Input.InputManager.ProcessStagingArea()
                           bei System.Windows.Input.InputManager.ProcessInput(InputEventArgs input)
                           bei System.Windows.Input.InputProviderSite.ReportInput(InputReport inputReport)
                           bei System.Windows.Interop.HwndMouseInputProvider.ReportInput(IntPtr hwnd, InputMode mode, Int32 timestamp, RawMouseActions actions, Int32 x, Int32 y, Int32 wheel)
                           bei System.Windows.Interop.HwndMouseInputProvider.FilterMessage(IntPtr hwnd, WindowMessage msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
                           bei System.Windows.Interop.HwndSource.InputFilterMessage(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
                           bei MS.Win32.HwndWrapper.WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, Boolean& handled)
                           bei MS.Win32.HwndSubclass.DispatcherCallbackOperation(Object o)
                           bei System.Windows.Threading.ExceptionWrapper.InternalRealCall(Delegate callback, Object args, Int32 numArgs)
                           bei MS.Internal.Threading.ExceptionFilterHelper.TryCatchWhen(Object source, Delegate method, Object args, Int32 numArgs, Delegate catchHandler)
                           bei System.Windows.Threading.Dispatcher.LegacyInvokeImpl(DispatcherPriority priority, TimeSpan timeout, Delegate method, Object args, Int32 numArgs)
                           bei MS.Win32.HwndSubclass.SubclassWndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam)
                           bei MS.Win32.UnsafeNativeMethods.DispatchMessage(MSG& msg)
                           bei System.Windows.Threading.Dispatcher.PushFrameImpl(DispatcherFrame frame)
                           bei System.Windows.Threading.Dispatcher.PushFrame(DispatcherFrame frame)
                           bei System.Windows.Threading.Dispatcher.Run()
                           bei System.Windows.Application.RunDispatcher(Object ignore)
                           bei System.Windows.Application.RunInternal(Window window)
                           bei System.Windows.Application.Run(Window window)
                           bei System.Windows.Application.Run()
                           bei Ctms.Presentation.App.Main() in C:\Users\Lukas\Workspace\ASE\CTMS\app\Ctms.Presentation\obj\x86\Debug\App.g.cs:Zeile 50.
                           bei System.AppDomain._nExecuteAssembly(RuntimeAssembly assembly, String[] args)
                           bei System.AppDomain.ExecuteAssembly(String assemblyFile, Evidence assemblySecurity, String[] args)
                           bei Microsoft.VisualStudio.HostingProcess.HostProc.RunUsersAssembly()
                           bei System.Threading.ThreadHelper.ThreadStart_Context(Object state)
                           bei System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
                           bei System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
                           bei System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
                           bei System.Threading.ThreadHelper.ThreadStart()
                      InnerException: 
             */

            //SpotifySessionConfig spotifySessionConfig = new SpotifySessionConfig(spotifyApiKey);
            //SpotifySession spotifySession = new SpotifySession(spotifyUsername, spotifyPassword, spotifySessionConfig);


        }
    }
}