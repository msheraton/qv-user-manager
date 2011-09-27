$Searcher = New-Object DirectoryServices.DirectorySearcher 
$Searcher.Searchroot = $args[0] 
$searcher.Filter = "(objectCategory=user)" 
$Searcher.PageSize = 1000
$Results=$Searcher.FindAll()
$Results|%{$_.GetDirectoryEntry().sAMAccountName}