if($Args.length -ne 2){
	$help = @"
	
	This is a Subversion repository hook script.
	
	pre-commit REPOSPATH TRANSID
		
	where	
		REPOSPATH is the physical path to the Subversion repository.
		TRANSID is the current subversion transaction ID.
"@
	Write-Output $help
	return 1
}

$repository = $Args[0]
$transaction = $Args[1]
$errors = ""

# Loop through all the items in this commit
svnlook changed "$repository" --transaction $transaction | ForEach-Object {

	# Find first space instead of assuming column 1.
	# Allows handling of multi-character action flags.
	$pos = $_.IndexOf(" ")
	$filePath = $_.Substring($pos).trim()

	# Don't check folders.
	if( -Not $filePath.endsWith('/') ) {

		# Test for items being updated
		if($_[0] -eq 'U') {

			# Check whether item is locked.
			$lockInfoFound = $False
			svnlook lock "$repository" "$filePath" | ForEach-Object {
				$lockInfoFound = $True
			}
			if( -Not $lockInfoFound ) {
				$errors = "YES"
				[Console]::Error.WriteLine("File not Locked: {0}", $filePath)
			}
		}
		# Test for items being added
		elseif ($_[0] -eq 'A') {

			# Check whether added items have svn:needs-lock property.
			svnlook propget "$repository" svn:needs-lock "$filePath" --transaction $transaction

			if( $LastExitCode -ne 0 ) {
				$errors = "YES"
				}
		}
		# Test for items being deleted
		elseif ($_[0] -eq 'D') {

			# Checks for deletions go here.
			# Set $errors = "YES" to report failures.
		}
	}
}

if( $errors -ne "" ){
	exit 1
}