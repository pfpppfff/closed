@echo off
echo 正在构建S7NET项目...

REM 尝试使用Visual Studio 2022的MSBuild
if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    echo 使用Visual Studio 2022 MSBuild...
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" S7NET.csproj /p:Configuration=Debug /p:Platform=AnyCPU /verbosity:minimal
    goto :end
)

REM 尝试使用Visual Studio 2019的MSBuild
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    echo 使用Visual Studio 2019 MSBuild...
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" S7NET.csproj /p:Configuration=Debug /p:Platform=AnyCPU /verbosity:minimal
    goto :end
)

REM 尝试使用.NET Framework MSBuild
if exist "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" (
    echo 使用.NET Framework MSBuild...
    "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MSBuild.exe" S7NET.csproj /p:Configuration=Debug /p:Platform=AnyCPU /verbosity:minimal
    goto :end
)

echo 未找到MSBuild，请确保已安装Visual Studio或Build Tools
pause

:end
echo 构建完成
pause
