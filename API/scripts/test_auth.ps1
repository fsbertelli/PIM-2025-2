function Send($method, $url, $json) {
    Write-Output "\n---- $method $url ----"
    try {
        $resp = Invoke-RestMethod -Method $method -Uri $url -ContentType 'application/json' -Body $json -ErrorAction Stop
        Write-Output "Status: 200 OK"
        $resp | ConvertTo-Json -Depth 6 | Write-Output
    }
    catch {
        $ex = $_.Exception
        if ($ex.Response -ne $null) {
            $status = $ex.Response.StatusCode.value__
            $sr = New-Object System.IO.StreamReader($ex.Response.GetResponseStream())
            $body = $sr.ReadToEnd()
            Write-Output "Status: $status"
            Write-Output $body
        }
        else {
            Write-Output "Error: $($_.Exception.Message)"
        }
    }
}

# 1) PUT user 1
$putJson = '{"name":"Felipe Bertelli","email":"fs.bertelli@hotmail.com","password":"capoeirarosa","deptId":1,"userStatusId":1,"profileId":1}'
Send 'Put' 'http://localhost:5185/users/1' $putJson

# 2) POST /login
$loginJson = '{"email":"fs.bertelli@hotmail.com","password":"capoeirarosa"}'
Send 'Post' 'http://localhost:5185/login' $loginJson

# 3) Try create duplicate user
$createDup = '{"name":"Dup","email":"fs.bertelli@hotmail.com","password":"dup12345","deptId":1,"userStatusId":1,"profileId":1}'
Send 'Post' 'http://localhost:5185/users' $createDup

# 4) Create a second user
$create2 = '{"name":"User Two","email":"user2@example.com","password":"pass1234","deptId":1,"userStatusId":1,"profileId":1}'
Send 'Post' 'http://localhost:5185/users' $create2

# 5) Get all users to find user2 id
Write-Output "\n---- Get /users ----"
try {
    $all = Invoke-RestMethod -Uri 'http://localhost:5185/users' -Method Get -ErrorAction Stop
    $all | ConvertTo-Json -Depth 6 | Write-Output
    $user2 = $all | Where-Object { $_.email -eq 'user2@example.com' }
    if ($null -eq $user2) {
        Write-Output "user2 not found"
        exit 0
    }
    $user2Id = $user2.id
    Write-Output "user2 id: $user2Id"

    # 6) Try updating user2 email to existing (should conflict)
    $putConflict = '{"name":"User Two","email":"fs.bertelli@hotmail.com","password":"pass1234","deptId":1,"userStatusId":1,"profileId":1}'
    Send 'Put' ("http://localhost:5185/users/$user2Id") $putConflict
}
catch {
    Write-Output "Error fetching users: $($_.Exception.Message)"
}

