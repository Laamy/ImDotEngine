if not exist "ImDotEngine\bin\x64\Debug\Data" (
	mkdir "ImDotEngine\bin\x64\Debug\Data"
)

for /d %%i in (ImDotEngine\Data\*) do (
	powershell.exe Compress-Archive -Path "%%i" -DestinationPath ""ImDotEngine\bin\x64\Debug\Data\%%~ni.zip" -Force
)