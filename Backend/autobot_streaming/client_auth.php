<?php
$link = mysqli_connect("localhost", "ni4155434_1sql1","O4YVP5Mza72Z5OkzYwEvAX2cVl2hpiCZVQHDaQEFskWPo7DFJKGjdzoyGQCp0yFN");
$database = mysqli_select_db($link, "ni4155434_1sql1");

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
            $sql = "UPDATE ". $tables ." SET hwid='$hwid' WHERE username_clean='$user'";
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