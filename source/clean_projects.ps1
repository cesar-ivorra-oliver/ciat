param (
  [switch]$Preview  # Enables preview mode
)

# Ensure the script is executed inside the 'source' directory
$expectedFolder = "source"
$currentFolder = Split-Path -Leaf (Get-Location)

if ($currentFolder -ne $expectedFolder) {
  Write-Host "Error: This script must be executed inside the '$expectedFolder/' directory." -ForegroundColor Red
  exit 1
}

Write-Host "Starting cleanup process..."
if ($Preview) { Write-Host "[Preview Mode] No files will be deleted." -ForegroundColor Yellow }

# Function to delete or preview items (handles both files and folders)
function Remove-Or-Preview {
  param ($path)
  
  if (Test-Path $path) {
    $type = if ((Get-Item $path -Force).PSIsContainer) { "Folder" } else { "File" }

    if ($type -eq "Folder") {
      $type = "Folder"
    } else {
      $type = "File"
    }

    if ($Preview) {
      Write-Host "[Preview] Would delete ($type): $path" -ForegroundColor Cyan
    } else {
      Remove-Item $path -Recurse -Force
      Write-Host "Deleted ($type): $path" -ForegroundColor Green
    }
  }
}

# List of files to remove
$itemsToRemove = @(
  "ciat.sln", 
  "ciatLauncher/ciatLauncher.csproj", 
  "ciatCommands/ciatCommands.csproj"
)

# Remove files
foreach ($item in $itemsToRemove) {
  Remove-Or-Preview (Join-Path -Path (Get-Location) -ChildPath $item)
}

# List of folders to remove (search recursively)
$foldersToRemove = @(
  "bin",
  "obj",
  "Properties",
  ".vs"
)
Get-ChildItem -Path (Get-Location) -Recurse -Directory -Force | Where-Object { $_.Name -in $foldersToRemove } | ForEach-Object {
  Remove-Or-Preview $_.FullName
}

Write-Host "Cleanup completed."
if ($Preview) {
  Write-Host "[Preview Mode] No files were actually deleted." -ForegroundColor Yellow
}
