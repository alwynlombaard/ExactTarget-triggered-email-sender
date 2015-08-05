param([string]$task = "local")

$ApplicationName = "ExactTarget.TriggeredEmail"

$ChocolateyPackages = @(
)

$NugetPackages = @(
	"ExactTarget.TriggeredEmail"
)

$webClient = new-object net.webclient
$webClient.Headers.Add("Accept", "application/vnd.github.3.raw")
$webClient.Headers.Add("User-Agent", "JustGivingBuildScript")
$webClient.DownloadFile('https://api.github.com/repos/justgiving/GG.BuildScript/contents/bootstrap.ps1?access_token=e2f1fc95f4cac32894ef104d4d98ceb32780ab0f','bootstrap.ps1')

. .\bootstrap.ps1

Invoke-Build -Task $task -ApplicationName $ApplicationName -ChocolateyPackages $ChocolateyPackages -NugetPackages $NugetPackages
