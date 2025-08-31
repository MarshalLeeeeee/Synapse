@echo off
SETLOCAL

set "SCRIPT_DIR=%~dp0"

set "CLIENT_PATH=SynapseClient"
set "SERVER_PATH=SynapseServer"
set "COMMON_PATH=SynapseCommon"
set "COMMON_CODE_RELATIVE=Common"

:: 转换为绝对路径
call :MakeAbsolute "%SCRIPT_DIR%\%CLIENT_PATH%" CLIENT_ABS_PATH
call :MakeAbsolute "%SCRIPT_DIR%\%SERVER_PATH%" SERVER_ABS_PATH
call :MakeAbsolute "%SCRIPT_DIR%\%COMMON_PATH%\%COMMON_CODE_RELATIVE%" COMMON_ABS_PATH

echo CLIENT_ABS_PATH=%CLIENT_ABS_PATH%
echo SERVER_ABS_PATH=%SERVER_ABS_PATH%
echo COMMON_ABS_PATH=%COMMON_ABS_PATH%

if not exist "%CLIENT_ABS_PATH%\%COMMON_CODE_RELATIVE%" (
    echo Create link for ClientShared...
    mklink /J "%CLIENT_ABS_PATH%\%COMMON_CODE_RELATIVE%" "%COMMON_ABS_PATH%"
) else (
    echo ClientSharedExisted...
)

if not exist "%SERVER_ABS_PATH%\%COMMON_CODE_RELATIVE%" (
    echo Create link for ServerShared...
    mklink /J "%SERVER_ABS_PATH%\%COMMON_CODE_RELATIVE%" "%COMMON_ABS_PATH%"
) else (
    echo ServerSharedExisted...
)

echo.
echo Over...

pause
EXIT /B

:MakeAbsolute
SET "%~2=%~f1"
EXIT /B