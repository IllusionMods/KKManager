# Env setup ---------------
if ($PSScriptRoot -match '.+?\\bin\\?') {
    $dir = $PSScriptRoot + "\"
}
else {
    $dir = $PSScriptRoot + "\bin\"
}

$outfile = $dir + "\_release.zip"

Remove-Item $outfile -ErrorAction Ignore -Force

Remove-Item ($dir + "ModpackTool\Temp") -ErrorAction Ignore -Force -Recurse

Get-ChildItem $dir -Exclude *.log,*.pdb,*.xml,*.zip,*.7z,*.ps1,*.lnk,*.settings,FileContentsHashCache.bin,UpdateSourcesDeb*,cache | Compress-Archive -DestinationPath $outfile -Force -CompressionLevel Optimal