param (
  [switch]$Preview  # Enables preview mode
)

$addPreview = ""
if ($Preview) { $addPreview = "--preview" }

dotnet-script $PSScriptRoot\scripts\CleanProjects.csx $addPreview