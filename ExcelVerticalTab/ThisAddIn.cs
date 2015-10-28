﻿/*
- [ ] タブの同期
- [ ] リボンメニュー
- [ ] タブ移動
- [ ] コンテキストメニュー

*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using ExcelVerticalTab.Controls;
using Microsoft.Office.Tools;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace ExcelVerticalTab
{
    public partial class ThisAddIn
    {
        private VerticalTabHost ControlHost { get; set; }

        public ConcurrentDictionary<Excel.Workbook, PaneAndControl> Panes { get; } = new ConcurrentDictionary<Excel.Workbook, PaneAndControl>(); 

        public ConcurrentDictionary<Excel.Workbook, WorkbookHandler> Handlers { get; } = new ConcurrentDictionary<Excel.Workbook, WorkbookHandler>(); 
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        { 
            this.Application.WorkbookActivate += Application_WorkbookActivate;
            // クローズ時の破棄をどうするか
        }

        private PaneAndControl CreatePane(Excel.Workbook wb)
        {
            var control = new VerticalTabHost();
            control.Initialize();

            var pane = CustomTaskPanes.Add(control, "VTab");
            pane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionLeft;
            pane.Visible = true;

            return new PaneAndControl(pane, control);
        }

        private void Application_WorkbookActivate(Excel.Workbook Wb)
        {
            OnActivate(Wb);
        }

        public void OnActivate(Excel.Workbook wb)
        {
            var pane = Panes.GetOrAdd(wb, x => CreatePane(x));
            var handler = Handlers.GetOrAdd(wb, x => new WorkbookHandler(x));
            // タブの同期
            handler.SyncWorksheets();
            pane.Control.AssignWorkbookHandler(handler);
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        protected override Office.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Menu();
        }

        #region VSTO で生成されたコード

        /// <summary>
        /// デザイナーのサポートに必要なメソッドです。
        /// このメソッドの内容をコード エディターで変更しないでください。
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }

    public class PaneAndControl
    {
        public PaneAndControl(CustomTaskPane pane, VerticalTabHost control)
        {
            Pane = pane;
            Control = control;
        }

        public CustomTaskPane Pane { get; }
        public VerticalTabHost Control { get; }
    }

}
