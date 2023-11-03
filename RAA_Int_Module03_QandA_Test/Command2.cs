#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAA_Int_Module03_QandA_Test
{
    [Transaction(TransactionMode.Manual)]
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 1. get the active view
            View view = doc.ActiveView;

            // 2. create list of categories that will be used for the filter
            List<ElementId> categories = new List<ElementId>();
            categories.Add(new ElementId(BuiltInCategory.OST_Walls));

            // 3a. create rule 1 - width greater than or equal to 200mm
            ElementId widthParamId = new ElementId(BuiltInParameter.WALL_ATTR_WIDTH_PARAM);
            FilterRule rule1 = ParameterFilterRuleFactory.CreateGreaterOrEqualRule(widthParamId, 0.5, 0);

            // 3b. create rule 2 - wall function equal 'exterior'
            ElementId exteriorParamId = new ElementId(BuiltInParameter.FUNCTION_PARAM);
            FilterRule rule2 = ParameterFilterRuleFactory.CreateEqualsRule(exteriorParamId, (int)WallFunction.Exterior);

            // 4. create LogicalAndFilter using filter rules
            ElementParameterFilter filter1 = new ElementParameterFilter(rule1);
            ElementParameterFilter filter2 = new ElementParameterFilter(rule2);
            LogicalAndFilter elemFilter = new LogicalAndFilter(filter1, filter2);

            // 4. ALT create LogicalOrFilter using filter rules
            LogicalOrFilter elemFilter2 = new LogicalOrFilter(filter1, filter2);

            using (Transaction t = new Transaction(doc, "Create and Apply Filter"))
            {
                t.Start();

                // 5. create parameter filter 
                ParameterFilterElement paramFilter = ParameterFilterElement.Create(doc, "Exterior Wall Thickness", categories);
                paramFilter.SetElementFilter(elemFilter2);

                // 6. set graphic overrides
                OverrideGraphicSettings overrides = new OverrideGraphicSettings();
                overrides.SetCutLineColor(new Color(255, 0, 0));

                // 7. apply filter to view and set visibility and overrides
                view.AddFilter(paramFilter.Id);
                view.SetFilterVisibility(paramFilter.Id, true);
                view.SetFilterOverrides(paramFilter.Id, overrides);
                t.Commit();
            }

            return Result.Succeeded;
        }
        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand2";
            string buttonTitle = "Button 2";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 2");

            return myButtonData1.Data;
        }
    }
}
