param([string]$filename = "")

function test-url([string]$url)
{
    [Net.HttpWebRequest] $webRequest = [net.WebRequest]::Create($url);

    $webRequest.Timeout = 30000; ##30 Second timeout
    $webRequest.ReadWriteTimeout = 30000; ##30 Second timeout
    $webRequest.Method = "HEAD";

    Try
    {
        [Net.HttpWebResponse] $webResponse = $webRequest.GetResponse();
        
        return $true;
        
    } 
    Catch
    {
        return $false;
    }
    Finally
    {
        $webResponse.Close();        
    }    
}

if ($filename -eq "")
{
    Write-Host "Please specify a filename by using -filename"
}

foreach ($line in Get-Content $filename) 
{
    $url = $line.Trim();
    $isValid = test-url($url);
    Write-Host "$url,$isValid";    
}


