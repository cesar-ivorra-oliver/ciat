# Ensure the script is executed inside the 'source' directory
$expectedFolder = "source"
$currentFolder = Split-Path -Leaf (Get-Location)

if ($currentFolder -ne $expectedFolder) {
  Write-Host "Error: This script must be executed inside the '$expectedFolder\' directory." -ForegroundColor Red
  exit 1
}

# define directories
$sourceDir  = (Get-Location) # source directory
$tempDir    = New-Item -ItemType Directory -Path "_ciat_temp" -Force

# define project names
$solutionName     = "ciat"
$launcherProject  = "ciatLauncher"
$commandsProject  = "ciatCommands"

Write-Host "Creating projects..."
Write-Host "Temporary folder created at: $tempDir"

# Create the solution in the temporary folder
Set-Location $tempDir
dotnet new sln -n $solutionName

# Create projects in the temporary folder
dotnet new console  -o $launcherProject --framework net9.0
dotnet new classlib -o $commandsProject --framework net9.0

# Add projects to the solution
dotnet sln $solutionName.sln add "$launcherProject\$launcherProject.csproj"
dotnet sln $solutionName.sln add "$commandsProject\$commandsProject.csproj"

# Add ciatCommands reference to ciatLauncher
dotnet add "$launcherProject\$launcherProject.csproj" reference "$commandsProject\$commandsProject.csproj"

# Add NuGet packages with specific versions
dotnet add "$launcherProject\$launcherProject.csproj" package Microsoft.CodeAnalysis        --version 4.12.0
dotnet add "$launcherProject\$launcherProject.csproj" package Microsoft.CodeAnalysis.CSharp --version 4.12.0
dotnet add "$launcherProject\$launcherProject.csproj" package System.CommandLine            --version 2.0.0-beta4.22272.1

Write-Host "Projects created in the temporary folder."

# Copy solution and project files from the temporary folder to source\ without overwriting existing code
Set-Location $sourceDir
Copy-Item "$tempDir\$solutionName.sln"                        "$sourceDir\$solutionName.sln" -Force
Copy-Item "$tempDir\$launcherProject\$launcherProject.csproj" "$sourceDir\$launcherProject\$launcherProject.csproj" -Force
Copy-Item "$tempDir\$commandsProject\$commandsProject.csproj" "$sourceDir\$commandsProject\$commandsProject.csproj" -Force

Write-Host "Solution and project files copied to source\ without affecting existing code."


# Clean up the temporary folder
Write-Host "Deleting temporary folder..."
Remove-Item -Recurse -Force $tempDir
Write-Host "Temporary folder deleted."

Write-Host "Projects created successfully."