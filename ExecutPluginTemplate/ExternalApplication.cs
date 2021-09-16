// This is a work based and developed after studies on the 
// Revit SDK Windows Forms-based example project named ModelessForm_ExternalEvent. 
// So, we came up with a solution that adapts the logic in the example 
// to a simple and useful WPF application project that can be used as a template.

using Autodesk.Revit.UI;
using ExecutPluginTemplate.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ExecutPluginTemplate
{
    /// <summary>
    /// This class implements the IExternalApplication interface. Here our external application is set. 
    /// Here we have the methods that determine what happens when you start and when you close your application.
    /// Here we create and set the placement of the button that will start our application in Revit.
    /// Also here is where we create the methods that will control the activity of our application window.
    /// </summary>
    public class ExternalApplication : IExternalApplication
    {
        internal static ExternalApplication thisApp = null; // instantiating the class, initially null, since the app initialization user interface is not set yet

        private MainWindow mWnd; // instantiating the MainWindow (modeless dialog)

        public Result OnShutdown(UIControlledApplication application)
        {
            /* Very important: always use the Dispose method in the shutdown to close the window and any other control/instance initialized by the application. */

            if (mWnd != null && mWnd.IsVisible)
            {
                mWnd.Dispose();
            }
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            /* This is where we create the button to launch the app and set its placement in the Revit ribbon tab. */

            //----Creating the UI ribbon tab           
            application.CreateRibbonTab("My Add-In Tab");

            //----Creating the UI button
            string path = Assembly.GetExecutingAssembly().Location; // this is the path to the dll
            PushButtonData button = new PushButtonData("ExternalApplication", "My WPF Add-In", path,
                "ExecutPluginTemplate.Commands.ExternalCommand"); // note that the button initializes the ExternalCommand class in its last argument

            //----Creating the UI panel using the ribbon tab as first argument
            RibbonPanel panel = application.CreateRibbonPanel("My Add-In Tab", "My Add-In Panel");

            //----Adding the button image           
            BitmapImage largeImage = ImageToBitmapImage(Properties.Resources.TLAddinIcon32_96); // see Auxiliary Methods at the end
            BitmapImage image = ImageToBitmapImage(Properties.Resources.TLAddinIcon16_96); // see Auxiliary Methods at the end

            //----Creating the button
            PushButton pushButton = panel.AddItem(button) as PushButton;
            pushButton.LargeImage = largeImage;
            pushButton.Image = image;
            pushButton.ToolTip = "This is a brief description of My WPF Add-In and its features";
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.Url, "replace this string with a valid URL for a tutorial or help content for your app");
            pushButton.SetContextualHelp(contextHelp);

            mWnd = null; // the MainWindow (modeless dialog) will not be initialized here, but only by the ShowWindow mehod below (called by the ExternalCommand class, started by the app button)
            thisApp = this; // replacing the null value of the application with the application itself since its ribbon elements are now set

            return Result.Succeeded;
        }


        //----METHODS TO BE CALLED BY OTHER CLASSES--------------------------------------------------
        public void ShowWindow(UIApplication uiApp)
        {
            /* This method will be called by the ExternalCommand (activated by the click on the ribbon button of the app). */

            //----Showing the window
            if (mWnd == null || mWnd.IsLoaded == false)
            {
                //----Creating the handler to manage the requests done by the commands in the MainWindow (modeless dialog)
                RequestHandler handler = new RequestHandler();

                //----Creating the external event to be raised as a request identified by the handler
                ExternalEvent exEvent = ExternalEvent.Create(handler);

                //----Initializing the MainWindow with the above instances as arguments (see the code behind of the Main Window and the MakeRequest method)       
                mWnd = new MainWindow(exEvent, handler);
                mWnd.Show();
            }
        }

        public void WakeWindowUp()
        {
            /* This method is used in the RequestHandler Execute method to keep the modeless dialog active after a request. */

            if (mWnd != null)
            {
                mWnd.WakeUp();
                mWnd.Activate();
            }
        }


        //----AUXILIARY METHODS--------------------------------------------------

        private BitmapImage ImageToBitmapImage(Image img)
        {
            /* This method, when called, will convert an image in your project to the the BitmapImage format required by the PushButton class. */

            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png); // set the input format here
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
