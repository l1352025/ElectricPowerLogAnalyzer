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
		7z x -aoa %dstpath%\plccomlog\plccom.log.sub_0%%i.tar.gz -o%dstpath%\plccomlog
		7z x -aoa %dstpath%\plccomlog\plccom.log.sub_0%%i.tar -o%dstpath%\plccomlog
		if exist %dstpath%\plccomlog\plccom.log (
			copy /b %dstpath%\plccomlog\plccom.log + %dstpath%\plccomlog\plccom.log.sub %dstpath%\plccomlog\plccom.log
		) else (
			copy /b %dstpath%\plccomlog\plccom.log.sub %dstpath%\plccomlog\plccom.log
		)
	)
)

goto :clear

if exist %dstpath%\plccom.log (
	if exist %dstpath%\plccomlog\plccom.log (
		copy /b %dstpath%\plccomlog\plccom.log + %dstpath%\plccom.log %dstpath%\plccomlog\plccom.log
	) else (
		copy /b %dstpath%\plccom.log %dstpath%\plccomlog\plccom.log
	)
)

:clear
rem		delete all temp file
del %dstpath%\plccomlog\*.sub
del %dstpath%\plccomlog\*.tar