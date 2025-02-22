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
$solutionName         = "ciat"
$launcherProjectName  = "ciatLauncher"
$commandsProjectName  = "ciatCommands"

Write-Host "Creating projects..."
Write-Host "Temporary folder created at: $tempDir"

# Create the solution in the temporary folder
Set-Location $tempDir
dotnet new sln -n $solutionName

# Create projects in the temporary folder
dotnet new console  -o $launcherProjectName --framework net9.0
dotnet new classlib -o $commandsProjectName --framework net9.0

# Define project files
$vsSolution             = "$solutionName.sln"
$launcherProjectCsproj  = "$launcherProjectName\$launcherProjectName.csproj"
$commandsProjectCsproj  = "$commandsProjectName\$commandsProjectName.csproj"

# Add projects to the solution
dotnet sln $vsSolution add $launcherProjectCsproj
dotnet sln $vsSolution add $commandsProjectCsproj

# Add NuGet packages with specific versions
dotnet add $launcherProjectCsproj package Microsoft.CodeAnalysis        --version 4.12.0
dotnet add $launcherProjectCsproj package Microsoft.CodeAnalysis.CSharp --version 4.12.0
dotnet add $launcherProjectCsproj package System.CommandLine            --version 2.0.0-beta4.22272.1

# Add references between projects
dotnet add $launcherProjectCsproj reference $commandsProjectCsproj

Write-Host "Projects created in the temporary folder."

# Copy solution and project files from the temporary folder to source\ without overwriting existing code
Set-Location $sourceDir
Copy-Item "$tempDir\$vsSolution"            "$sourceDir\$vsSolution"            -Force
Copy-Item "$tempDir\$launcherProjectCsproj" "$sourceDir\$launcherProjectCsproj" -Force
Copy-Item "$tempDir\$commandsProjectCsproj" "$sourceDir\$commandsProjectCsproj" -Force

Write-Host "Solution and project files copied to source\ without affecting existing code."


# Clean up the temporary folder
Write-Host "Deleting temporary folder..."
Remove-Item -Recurse -Force $tempDir
Write-Host "Temporary folder deleted."

Write-Host "Projects created successfully."