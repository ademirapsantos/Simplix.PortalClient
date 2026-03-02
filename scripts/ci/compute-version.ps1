param(
    [string]$BranchName,
    [string]$Sha,
    [int]$RunNumber = 0
)

$normalizedBranch = ($BranchName ?? "dev").ToLowerInvariant()
$shortSha = if ([string]::IsNullOrWhiteSpace($Sha)) { "local" } else { $Sha.Substring(0, [Math]::Min(7, $Sha.Length)).ToLowerInvariant() }

if ($RunNumber -le 0) {
    $RunNumber = 1
}

$version = switch ($normalizedBranch) {
    "main" { "1.0.$RunNumber" }
    "release" { "1.0.$RunNumber-rc.$shortSha" }
    default { "1.0.$RunNumber-dev.$shortSha" }
}

$assemblyVersion = "1.0.$RunNumber.0"

"version=$version"
"assembly_version=$assemblyVersion"
"file_version=$assemblyVersion"
"informational_version=$version+$shortSha"
"image_tag=$version"
"short_sha=$shortSha"
