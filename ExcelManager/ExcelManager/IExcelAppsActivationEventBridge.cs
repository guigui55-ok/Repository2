using Microsoft.Office.Interop.Excel;
using System;

namespace ExcelUtility
{
    public interface IExcelAppsEventBridgeInterface
    {
        int Number { get; set; }

        WorkbookEvents_WindowActivateEventHandler Workbook_WindowActivateEvent { get; set; }
        AppEvents_WindowActivateEventHandler Application_WindowActivateEvent { get; set; }
        WorkbookEvents_DeactivateEventHandler Workbook_DeactivateEvent { get; set; }
        AppEvents_WindowDeactivateEventHandler Application_DeactivateEvent { get; set; }
        WorkbookEvents_SheetActivateEventHandler WorkSheet_ActivateEvent { get; set; }
        AppEvents_SheetSelectionChangeEventHandler Application_SheetSelectionChangeEvent { get; set; }
        AppEvents_SheetActivateEventHandler Application_SheetActivateEvent { get; set; }
        AppEvents_WorkbookOpenEventHandler Application_WorkbookOpenEvent { get; set; }
        AppEvents_WorkbookBeforeCloseEventHandler Application_WorkbookBeforeCloseEvent { get; set; }


        EventHandler Application_SheetSelectionChangeAfterEvent { get; set; }
        EventHandler Application_WindowActivateAfterEvent { get; set; }
        EventHandler Application_DeactivateAddEvent { get; set; }
        EventHandler WorkSheet_ActivateAfterEvent { get; set; }
        EventHandler Application_SheetActivateAfterEvent { get; set; }
        EventHandler Application_WorkbookOpenAfterEvent { get; set; }
        EventHandler Application_WorkbokBeforeCloseAddEvent { get; set; }
    }
}
