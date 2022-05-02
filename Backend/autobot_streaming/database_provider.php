<?php
$link = mysqli_connect("127.0.0.1", "admin_autobot_streaming","32HZYYKKZFv1yf0WrMjKXYa1rQMTVaV44c6pETQM5650PogkMOaGFk8wQhApp04I");
$database = mysqli_select_db($link, "admin_autobot_streaming");

$user = $_GET['username'];
$token_provided = $_GET['token'];
$tables = "users";
$mysql_host_request = $_GET['mh'];
$mysql_database_request = $_GET['md'];
$mysql_username_request = $_GET['mu'];
$mysql_password_request = $_GET['mp'];

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
        
        if ($token_provided == $row['token'])
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
        }
        else
        {
            echo "8x7n5346x576n5"; //token incorrect
        }
    }
}  
?>