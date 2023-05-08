
set ProjectDir=%1
set ModId=%2
set ConfigurationName=%3
set TargetPath=%4
set TargetDir=%5
set TargetName=%6

set StaticDir=%ProjectDir%..\%ModId%\
set CopyDir=%ProjectDir%..\%ConfigurationName%\BepInEx\plugins\%ModId%\

echo =============================================================================================================
echo START COPY 
echo -------------------------------------------------------------------------------------------------------------
echo * TargetName        = %TargetName%
echo * ConfigurationName = %ConfigurationName%
echo * CopyDir           = %CopyDir%

echo =============================================================================================================
echo COPY : STATIC FILES
echo d | xcopy /y /s %StaticDir% %CopyDir%

echo -------------------------------------------------------------------------------------------------------------
echo COPY : DLL
echo f | xcopy /y %TargetPath% "%CopyDir%%ModId%.dll"

if %ConfigurationName% == Debug echo -------------------------------------------------------------------------------------------------------------
if %ConfigurationName% == Debug echo COPY : PDB
if %ConfigurationName% == Debug echo f | xcopy /y "%TargetDir%%TargetName%.pdb" "%CopyDir%%ModId%.pdb"

echo -------------------------------------------------------------------------------------------------------------
echo COPY : LICENCE
xcopy /y "%ProjectDir%..\LICENSE" "%CopyDir%"

echo -------------------------------------------------------------------------------------------------------------
echo COPY : README
echo f | xcopy /y "%ProjectDir%..\README.md" "%CopyDir%README.txt"

echo =============================================================================================================