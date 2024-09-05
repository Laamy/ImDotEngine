if not exist "Data" (
	mkdir "Data"
)

for /d %%i in (..\..\..\Data\*) do (
	powershell.exe Compress-Archive -Path "%%i" -DestinationPath "Data\%%~ni.zip" -Force
)

for %%f in (Data\*.zip) do (
	if exist "Data\%%~nf.bundle" (
		del "Data\%%~nf.bundle"
	)

	ren "%%f" "%%~nf.bundle"
)