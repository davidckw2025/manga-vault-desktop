$ErrorActionPreference='Stop'
$root=$PSScriptRoot
$compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { $compiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe' }
if (-not (Test-Path -LiteralPath $compiler)) { throw 'C# compiler not found.' }
$output=Join-Path $root 'dist'
New-Item -ItemType Directory -Path $output -Force | Out-Null
$icon=Join-Path $output 'app.ico'
[IO.File]::WriteAllBytes($icon,[Convert]::FromBase64String([IO.File]::ReadAllText((Join-Path $root 'app.ico.base64'))))
$exe=Join-Path $output 'MangaVault.exe'
$html=Join-Path $root 'index.html'
$source=Join-Path $root 'ComicLauncher.cs'
& $compiler /nologo /target:winexe /optimize+ /reference:System.Windows.Forms.dll "/win32icon:$icon" "/out:$exe" "/resource:$html,ComicTool.html" $source
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
$check=Start-Process -FilePath $exe -ArgumentList '--self-test' -PassThru -Wait -WindowStyle Hidden
if ($check.ExitCode -ne 0) { throw 'Embedded UI validation failed.' }
Write-Output "Built: $exe"
