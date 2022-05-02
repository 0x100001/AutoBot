<?php
$link = mysqli_connect("127.0.0.1", "admin_autobot_streaming","32HZYYKKZFv1yf0WrMjKXYa1rQMTVaV44c6pETQM5650PogkMOaGFk8wQhApp04I");
$database = mysqli_select_db($link, "admin_autobot_streaming");

$user = $_GET['username'];
$token_provided = $_GET['token'];
$hwid = $_GET['hwid'];
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
            echo "x8457n84x5n784x5"; //token correct
        }
        else
        {
            echo "78934758973x97mx58"; //token incorrect
        }
        
        echo "g" . $row['group_id'];

        if (strlen($row['hwid']) > 1)
        {
            if ($hwid != $row['hwid'])
            {
                echo "7864n36x4n736nx473n4"; // Wrong
            }
            else
            {
                echo "6736n7435b46x3b564"; // Correct
            }
        }
        else
        {
            $sql = "UPDATE ". $tables ." SET hwid='$hwid' WHERE username='$user'";
            if(mysqli_query($link, $sql))
            {
                echo $row['hwid'];
                echo "674x37n463x634x374=="; // HWID Set
            }
            else
            {
                echo "h4"; // Else errors
            }
        }
    }
}  
?>