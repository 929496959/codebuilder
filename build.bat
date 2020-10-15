C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe CodeBuilder.sln /t:Rebuild /t:Clean /p:Configuration=Release;TargetFrameworkVersion=v4.5;Platform=x64 /clp:ErrorsOnly
C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe CodeBuilder.sln /t:Rebuild /t:Clean /p:Configuration=Release;TargetFrameworkVersion=v4.5;Platform=x86 /clp:ErrorsOnly

copy /y codebuilder\bin\x86\release\codebuilder.exe release\codebuilder_x86.exe
copy /y codebuilder\bin\x64\release\codebuilder.exe release\codebuilder_x64.exe
copy /y codebuilder\bin\x86\release\codebuilder.exe.config release\codebuilder_x86.exe.config
copy /y codebuilder\bin\x64\release\codebuilder.exe.config release\codebuilder_x64.exe.config
copy /y codebuilder\bin\release\CodeBuilder.Core.dll release\CodeBuilder.Core.dll
copy /y codebuilder\bin\release\CodeBuilder.Database.dll release\CodeBuilder.Database.dll
copy /y codebuilder\bin\release\CodeBuilder.PowerDesigner.dll release\CodeBuilder.PowerDesigner.dll
copy /y codebuilder\bin\release\CodeBuilder.Tools.dll release\CodeBuilder.Tools.dll
copy /y codebuilder\bin\release\CodeBuilder.T4.dll release\CodeBuilder.T4.dll
copy /y codebuilder\bin\release\CodeBuilder.Razor.dll release\CodeBuilder.Razor.dll
pause