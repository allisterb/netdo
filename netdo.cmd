@echo off
pushd
@setlocal
set ERROR_CODE=0

src\NetDo.Cli\bin\Debug\net10.0\NetDo.Cli.exe %*

:end
@endlocal
popd
exit /B %ERROR_CODE%