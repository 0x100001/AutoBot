<?php
$link = mysqli_connect("127.0.0.1", "admin_autobot_streaming","32HZYYKKZFv1yf0WrMjKXYa1rQMTVaV44c6pETQM5650PogkMOaGFk8wQhApp04I");
$database = mysqli_select_db($link, "admin_autobot_streaming");

$currentDate = new DateTime();
            
$user = $_GET['username'];
$token_provided = $_GET['token'];
$tables = "users";

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
            
            $endDate = new DateTime($row['license_expire']);
            //$endDate = new DateTime("2021-06-20");
            
            if ($currentDate < $endDate)
            {
                echo "758c47547nc5847nc5874n"; //not expired
            }
            else
            {
                echo "489c7n495784c75n84"; //expired
            }
            
            echo "x8457n84x5n784x5"; //token correct
        }
        else
        {
            echo "8x7n5346x576n5"; //token incorrect
        }
        
        echo "g" . $row['group_id'];
    }
}  
?>