param (
  [switch]$Preview  # Enables preview mode
)

$addPreview = ""
if ($Preview) { $addPreview = "--preview" }

dotnet-script .\scripts\CleanProjects.csx $addPreview