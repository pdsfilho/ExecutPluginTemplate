using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutPluginTemplate.Commands
{
    /// <summary>
    /// This is the class that will implement the IExternalEventHandler interface
    /// to handle all commands started by user action in the MainWindow (modeless dialog) 
    /// as Requests listed in an enumeration used by the Request class.
    /// Also here we will define all the methods that will build the application functionality using the Revit API.
    /// </summary>
    public class RequestHandler : IExternalEventHandler
    {
        /*  */

        public string GetName()
        {
            /* This method is needed to identify this event handler. */
            return "Request Handler";
        }

        delegate void Method(UIDocument uiDoc, Document doc); // first, a delegate to save us from repeating the UIApplication as argument for commands endless times

        public Request Request { get; } = new Request(); // instantiating the Request class, which will take the commands by the user by indentifying its RequestId 

        public void Execute(UIApplication app)
        {
            try
            {
                /* Based on the command started by the user in the MainWindow, 
                 these Switch cases will use the Take method from the Request class to execute the chosen command by its RequestId. */

                switch (Request.Take())
                {
                    case RequestId.None:
                        {
                            return;  // no request to handle
                        }
                    case RequestId.Command01:
                        {
                            GeneralMethod(app, Command01Method);
                            break;
                        }
                    case RequestId.Command02:
                        {
                            GeneralMethod(app, Command02Method);
                            break;
                        }
                    case RequestId.Command03:
                        {
                            GeneralMethod(app, Command03Method);
                            break;
                        }
                    case RequestId.Command04:
                        {
                            GeneralMethod(app, Command04Method);
                            break;
                        }
                    default:
                        {
                            TaskDialog.Show("Warning", "No valid request has been taken");
                            break;
                        }
                }
            }
            finally
            {
                ExternalApplication.thisApp.WakeWindowUp(); // keeping the dilaog active after a request
            }
            return;
        }


        //----MAIN METHOD----------------------------------------------------------------------------------------

        private void GeneralMethod(UIApplication app, Method method)
        {
            /* This method will be used in the Execute method above. 
             We just need to exchange the second argument instance (delegate) for each case in the Switch cases. */

            UIDocument uiDoc = app.ActiveUIDocument;

            Document doc = uiDoc.Document;

            method(uiDoc, doc); // using that useful Delegate Method we declared in the beginning to refer to each command method in each respective case
        }


        //----METHODS TO EXECUTE THROUGH DELEGATE----------------------------------------------------------------

        /* These are the methods that will be executed via Delegate based on the requests identified by the handler. 
         Essentially, these are the commands that use the Revit API tools in order to build each specific event triggered by the user.
         As this is something that is totally up to each developer, we will just use some very simple examples to deliver a fully executable template. 
         Just remember to overwrite them with your own methods to start playing around with your new multicommand Revit add-in. :) */

        private void Command01Method(UIDocument uiDoc, Document doc)
        {
            /* Just a simple command to return the number of currently selected itens in your Revit document. */
            Selection sel = uiDoc.Selection;
            ICollection<ElementId> ids = sel.GetElementIds();
            TaskDialog.Show("Command 01", "There are " + ids.Count + " elements selected in this project.");
        }

        private void Command02Method(UIDocument uiDoc, Document doc)
        {
            /* Just a simple command to return the number of elements that monitors some other element(s) in the model. */
            ICollection<ElementId> collector = new FilteredElementCollector(doc)
                   .WhereElementIsNotElementType().Where(x => x.IsMonitoringLocalElement()).Select(x => x.Id).Cast<ElementId>().ToList();

            TaskDialog.Show("Command 02", "There are " + collector.Count + " elements monitoring other elements.");
        }

        private void Command03Method(UIDocument uiDoc, Document doc)
        {
            /* Just a simple command to return the number of elements which are actually (not temporarily) hidden in the active view. */
            ICollection<ElementId> collector = new FilteredElementCollector(doc)
                   .WhereElementIsNotElementType().Where(x => x.IsHidden(doc.ActiveView)).Select(x => x.Id).Cast<ElementId>().ToList();
            TaskDialog.Show("Command 03", "There are " + collector.Count + " elements currently hidden in this view.");
        }

        private void Command04Method(UIDocument uiDoc, Document doc)
        {
            /* Just a simple command that will ask the user to select one element in the model and return the number of materials it uses. */
            TaskDialog.Show("Command 04", "Please, select one element in your model.");
            Reference pickedObject = uiDoc.Selection.PickObject(ObjectType.Element);
            if (pickedObject != null)
            {
                Element ele = doc.GetElement(pickedObject);
                ICollection<ElementId> materialIds = ele.GetMaterialIds(false);
                TaskDialog.Show("Command 04", "There are " + materialIds.Count + " materials used in this element.");
            }
        }
    }
}
