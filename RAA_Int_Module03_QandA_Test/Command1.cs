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
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            View target_view = doc.ActiveView;
            Category bicat = Category.GetCategory(doc, BuiltInCategory.OST_Rooms);

            toggle_element_selection_handles(target_view, bicat, true);

            Reference roomRef = uiapp.ActiveUIDocument.Selection.PickObject(ObjectType.Element, "Select room");

            toggle_element_selection_handles(target_view, bicat, false);

            return Result.Succeeded;
        }
        public static void toggle_element_selection_handles(View targetView, Category bicat, bool state)
        {
            using (Transaction tx = new Transaction(targetView.Document))
            {
                tx.Start("Toggle handles");

                // if view has template, toggle temp VG overrides
                if (state)
                    targetView.EnableTemporaryViewPropertiesMode(targetView.Id);

                Category rr_cat = GetSubcategory(bicat, "Reference");
                targetView.SetCategoryHidden(rr_cat.Id, false);

                Category rr_int = GetSubcategory(bicat, "Interior Fill");
                if (rr_int == null)
                    rr_int = GetSubcategory(bicat, "Interior");

                targetView.SetCategoryHidden(rr_int.Id, false);

                // disable the temp VG overrides after making changes to categories
                if (!state)
                    targetView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryViewProperties);

                tx.Commit();
            }
        }

        private static Category GetSubcategory(Category cat, string subCatName)
        {
            foreach(Category subCat in cat.SubCategories)
            {
                if(subCat.Name.Equals(subCatName))
                    return subCat;
            }

            return null;
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
