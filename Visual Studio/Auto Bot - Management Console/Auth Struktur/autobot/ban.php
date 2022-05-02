<?php
$link = mysqli_connect("127.0.0.1", "admin_autobot","WjMKd1SmeCOsXkvZhd8VwyhKVrrq6H04rG4GclUgl7NR7T4eP7MCXluJ23S6AFsi");
$database = mysqli_select_db($link, "admin_autobot");

$user = $_GET['username'];
$ban_reason = $_GET['reason'];
$password = $_GET['password'];
$tables = "licences";

$sql = "SELECT * FROM ". $tables ." WHERE username = '". mysqli_real_escape_string($link,$user) ."'" ;
$result = $link->query($sql);

if ($result->num_rows > 0) {
    // Outputting the rows
    while($row = $result->fetch_assoc())
    {
        function Redirect($url, $permanent = false)
        {
            if (headers_sent() === false)
            {
                header('Location: ' . $url, true, ($permanent === true) ? 301 : 302);
            }
        exit();
        }
            
        if ($row['banned_reason'] == '')
        {
            $sql = "UPDATE ". $tables ." SET banned_reason='$ban_reason' WHERE username ='$user' AND password ='$password';";
            if(mysqli_query($link, $sql))
            {
                echo $row['banned_reason'];
                echo "User banned.";
            }
            else
            {
                echo "h4"; // Else errors
            }
        }
        else if ($row['banned_reason'] != '')
        {
            //echo "Already registered a password on the user.<br>";
        }
    }
}  
?>