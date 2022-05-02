<?php
$link = mysqli_connect("127.0.0.1", "admin_autobot","WjMKd1SmeCOsXkvZhd8VwyhKVrrq6H04rG4GclUgl7NR7T4eP7MCXluJ23S6AFsi");
$database = mysqli_select_db($link, "admin_autobot");

$user = $_GET['username'];
$password_provided = $_GET['password'];
$tables = "licences";
$mysql_host_request = $_GET['mh'];
$mysql_database_request = $_GET['md'];
$mysql_username_request = $_GET['mu'];
$mysql_password_request = $_GET['mp'];
$mysql_port_request = $_GET['mpp'];

$mysql_host = "";
$mysql_database = "";
$mysql_username = "";
$mysql_password = "";

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
        
        if ($password_provided == $row['password'])
        {
            if($row['banned_reason'] != "")
            {
                echo "x79347xm37489xm3"; // user banned
            }

            if($mysql_host_request == "1")
            {
                echo $row['mysql_host']; 
            }
            
            if($mysql_database_request == "1")
            {
                echo $row['mysql_database']; 
            }
            
            if($mysql_username_request == "1")
            {
                echo $row['mysql_username']; 
            }
            
            if($mysql_password_request == "1")
            {
                echo $row['mysql_password']; 
            }
			
			if($mysql_port_request == "1")
            {
                echo $row['mysql_port']; 
            }
        }
        else
        {
            echo "8x7n5346x576n5"; //password incorrect
        }
    }
}  
?>