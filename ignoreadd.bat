set mypath=%~dp0%.p4ignore

IF NOT EXIST "%mypath%" color 04 && @echo. && @echo.&& echo ERROR: File does not exist && pause && goto eof

p4 set P4IGNORE="%mypath%"
@echo.
@echo.
@echo Sucsess!
@pause

:eof