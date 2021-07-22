#replace_string.ps1
    Param(
       [string]$filepath,
       [string]$find,
       [string]$replace
     )
    $content = Get-Content $filepath
    $content = $content.replace($find,$replace)
    $content | Set-Content $filepath