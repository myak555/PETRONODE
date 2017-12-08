'AutoBulkLoad.vbs
'
'This script triggers the import of all files from the specified input folder
'and applies a standard template afterwards. The results will be saved
'in the designated output folder.
'
'Author: Timo Korth
'==============================================

'Define constants
Const PATH = "C:\Automation\AutoBulkLoad\"
Const INPUTPATH = "C:\Automation\AutoBulkLoad\Input\"
Const OUTPUTPATH = "C:\Automation\AutoBulkLoad\Output\"

'Start WellCAD
Set WCAD = CreateObject("WellCAD.Application")
WCAD.ShowWindow()

'Get access to input folder and start to loop on
'all files in the folder
Dim FSO, obFolder, obFile
Set FSO = CreateObject("Scripting.FileSystemObject")
Set obFolder = FSO.GetFolder(INPUTPATH)
For Each obFile In obFolder.Files
'***********************************

'Import a LAS file, read processing parameter from ini file
Set obBHole = WCAD.FileImport(INPUTPATH & obFile.Name, FALSE, PATH & "AutoBulkLoad.ini")

'Apply a borehole document template
obBHole.ApplyTemplate PATH & "AutoBulkLoad.WDT", False, True

'Save it
strOutFile = Replace(obFile.Name, "las", "wcl")
obBhole.SaveAs OUTPUTPATH & strOutFile 

'***********************************
Next

'Prompt user to close docs
If MsgBox("Do you want to close the documents?",_
 vbYesNo + vbDefaultButton2, "Batch Process") = vbYes Then
WCAD.Quit()
End If










