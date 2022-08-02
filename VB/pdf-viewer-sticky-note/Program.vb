Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraEditors

Namespace pdf_viewer_sticky_note

    Friend Module Program

        ''' <summary>
        ''' The main entry point for the application.
        ''' </summary>
        <STAThread>
        Sub Main()
            Call Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            DevExpress.Skins.SkinManager.EnableFormSkins()
            DevExpress.UserSkins.BonusSkins.Register()
            Call WindowsFormsSettings.SetPerMonitorDpiAware()
            Call Application.Run(New Form1())
        End Sub
    End Module
End Namespace
