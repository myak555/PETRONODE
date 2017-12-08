'AutoPrint.vbs
'
'Demonstrates the basic concept of opening all
'WCL files contained in the input folder
'and printing them in two different scales on the default
'printer
'
'Author: Timo Korth
'==============================================

'Definition of input path
Const PATH = "C:\Automation\AutoPrint\Input\"

'Get WellCAD object
Set obWCAD = CreateObject("WellCAD.Application")
obWCAD.ShowWindow()

'Get access to the input folder
Dim FSO, obFolder, obFile
Set FSO = CreateObject("Scripting.FileSystemObject")
Set obFolder = FSO.GetFolder(PATH)
'Loop on files in the folder
For Each obFile In obFolder.Files
'***********************************

'Open WellCAD file
Set obBHole = obWCAD.OpenBorehole(PATH & obFile.Name)

'Disable page numbering
Set obPage = obBHole.Page
obPage.Numbering = 0

'Get Depth object
Set obDepth = obBHole.Depth

'Set depth scale to 1:100
obDepth.Scale = 100

'Send document to default printer
obBHole.DoPrint FALSE, obBHole.TopDepth, obBHole.BottomDepth, 1

'Set depth scale to 1:50
obDepth.Scale = 50

'Send document to default printer
obBHole.DoPrint FALSE, obBHole.TopDepth, obBHole.BottomDepth, 1

'Close document
obWCAD.CloseBorehole FALSE

'***********************************
Next