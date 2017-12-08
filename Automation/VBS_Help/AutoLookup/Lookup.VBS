'Template.vbs
'
'This script must be started with the Demo.WCL file
'already open. Before the template is applied the log
'titles will be renamed to match the names required by
'the template. The Template.ini file is used as a lookup
'table to provide information of the required titles.
'
'Author: Timo Korth
'=======================================================

'Definition of the root directory
Const LOOKUPFILE = "C:\Automation\AutoLookup\Lookup.ini"
Const TEMPLATEFILE = "C:\Automation\AutoLookup\Template.wdt"

'Start WellCAD
Set obWCAD = CreateObject("WellCAD.Application")
obWCAD.ShowWindow()
'Get current borehole document
Set obBHole = obWCAD.GetBorehole()

'Start the loop to check each log title
NbLogs = obBHole.NbOfLogs-1
For i = 0 To NbLogs

  'Get the log and its name
  Set obLog = obBHole.Log(CInt(CStr(i))) 'ensure an integer is passed
  title = obLog.Name
  
  'Get access to the lookup file
  Set FSO = CreateObject("Scripting.FileSystemObject")
  Set obLookUp = FSO.OpenTextFile(LOOKUPFILE, 1)
  
  'Find log in lookup table 
  SectionFound = FALSE
  Do While Not obLookUp.AtEndOfStream
    line = obLookUp.ReadLine
    If Left(line,1) = "[" Then
      If Ucase(Mid(line,2,Len(title))) = Ucase(title) Then
        SectionFound = TRUE
      Else
        SectionFound = FALSE
      End If
    Else
      If SectionFound Then
        'Incoming title found, read alias entry
        If Ucase(Left(line, Len("Alias"))) = Ucase("Alias") Then
          l = Len("Alias")+1
          Do While l < Len(line)
            sign = Mid(line, l, 1)
            If sign = "=" Then
              'get new title
              newtitle = Right(line, Len(line)-l)
              'rename
              obLog.Name = newtitle
              'reset and continue
              l = Len(line)
              SectionFound = FALSE
            Else
             l = l+1
            End If
          Loop
        End If
      End If
    End If
  Loop
  
  'Close ini file
  obLookUp.Close

Next

'Apply the borehole document template
obBHole.ApplyTemplate TEMPLATEFILE, False, True












