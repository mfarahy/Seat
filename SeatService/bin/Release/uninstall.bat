@SET INSTALLUTILDIR=
@for /F "tokens=1,2*" %%i in ('reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" /v "InstallPath"') DO (
    if "%%i"=="InstallPath" (
        SET "INSTALLUTILDIR=%%k"
    )
)

%INSTALLUTILDIR%\installutil SeatService.exe /u