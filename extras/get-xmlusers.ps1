[xml]$userfile = Get-Content $args[0]
foreach($user in $userfile.Users.User) 
{
	write-host $user.Name 
}