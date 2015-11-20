param($installPath, $toolsPath, $package, $project)

$helpfile = "MongoRepository.chm";
$helpfolder = "Documentation"
$helpsource = $installPath + "\" + $helpfile

$folder = $project.ProjectItems.Item($helpfolder)
If (-not $folder) {
	$folder = $project.ProjectItems.AddFolder($helpfolder)
}
$folder.ProjectItems.AddFromFile($helpsource)