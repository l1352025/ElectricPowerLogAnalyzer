@echo off

rem		set dst path
set dstpath=%1

rem 	delete plccom.log
if exist %dstpath%\plccomlog\plccom.log (
	del %dstpath%\plccomlog\plccom.log
)

rem 	merge all "plccom.log.sub_**.tar.gz" to plccom.log
for /l %%i in (0,1,9) do (
	if exist %dstpath%\plccomlog\plccom.log.sub_0%%i.tar.gz (
		7z x -aoa %dstpath%\plccomlog\plccom.log.sub_0%%i.tar.gz -o%dstpath%\plccomlog >nul
		7z x -aoa %dstpath%\plccomlog\plccom.log.sub_0%%i.tar -o%dstpath%\plccomlog >nul
		if exist %dstpath%\plccomlog\plccom.log (
			copy /b %dstpath%\plccomlog\plccom.log + %dstpath%\plccomlog\plccom.log.sub %dstpath%\plccomlog\plccom.log >nul
		) else (
			copy /b %dstpath%\plccomlog\plccom.log.sub %dstpath%\plccomlog\plccom.log >nul
		)
	)
)

if exist %dstpath%\plccom.log (
	if exist %dstpath%\plccomlog\plccom.log (
		copy /b %dstpath%\plccomlog\plccom.log + %dstpath%\plccom.log %dstpath%\plccomlog\plccom.log >nul
	) else (
		copy /b %dstpath%\plccom.log %dstpath%\plccomlog\plccom.log >nul
	)
)

echo.
if exist %dstpath%\plccomlog\plccom.log (
	echo merge plccom.log finished !
) else (
	echo merge failed : not find the directory '%dstpath%\plccomlog'
)
echo.



:clear
rem		delete all temp file
if exist %dstpath%\plccomlog\*.sub (
	del %dstpath%\plccomlog\*.sub >nul
)
if exist %dstpath%\plccomlog\*.sub (
	del %dstpath%\plccomlog\*.tar >nul
)